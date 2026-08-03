using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration;
using MeetFlow.BLL.DTOs.AiExtraction;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow.BLL.Services
{
    // Calls Google Gemini's generateContent endpoint directly — no separate Python/FastAPI
    // service needed. Set Gemini:ApiKey in appsettings.json (get one free at
    // https://aistudio.google.com/apikey).
    public class GeminiTaskExtractionService : IAiTaskExtractionService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GeminiTaskExtractionService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<List<ExtractedTaskDto>> ExtractTasksAsync(string meetingNotesText, List<WorkspaceMemberHintDto> workspaceMembers)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Gemini:ApiKey is not configured in appsettings.json.");

            var model = _config["Gemini:Model"] ?? "gemini-1.5-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            var prompt = BuildPrompt(meetingNotesText, workspaceMembers);

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var response = await _http.PostAsJsonAsync(url, requestBody);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadFromJsonAsync<GeminiResponse>(JsonOptions);
            var text = raw?.Candidates?.Count > 0
                ? raw.Candidates[0].Content?.Parts?.FirstOrDefault()?.Text
                : null;

            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("Gemini returned an empty response.");

            var tasks = ParseTasks(text);

            // Defense in depth: never trust an assigneeUserId the model invented.
            // If it's not one of the IDs we actually sent, treat it as unassigned.
            var validIds = workspaceMembers.Select(m => m.UserId).ToHashSet();
            foreach (var task in tasks)
            {
                if (task.AssigneeUserId is not null && !validIds.Contains(task.AssigneeUserId.Value))
                    task.AssigneeUserId = null;
            }

            return tasks;
        }

        private static string BuildPrompt(string notes, List<WorkspaceMemberHintDto> workspaceMembers)
        {
            var memberList = workspaceMembers.Count == 0
                ? "(no members available)"
                : string.Join("\n", workspaceMembers.Select(m => $"- userId {m.UserId}: {m.FullName}"));

            return $$"""
                You extract action items from meeting notes.

                Here are the real members of this workspace — match assignees against THIS list only,
                even if the notes use a different language, a nickname, or a partial name
                (e.g. Arabic "سارة" should match "Sara Ahmed" if that's the closest name below).
                If you are not reasonably confident which member is meant, or several members could
                match equally, use null instead of guessing:
                {{memberList}}

                Return ONLY a raw JSON array — no markdown, no code fences, no explanation.
                Each item must look exactly like this:
                {"title": "short task description", "assigneeUserId": <one of the userId values above, or null>, "dueDate": "YYYY-MM-DD or null", "priority": "Low, Medium, or High"}

                If the notes contain no clear action items, return an empty array: []

                Meeting notes:
                {{notes}}
                """;
        }

        private static List<ExtractedTaskDto> ParseTasks(string modelOutput)
        {
            // Models sometimes wrap JSON in ```json ... ``` even when told not to — strip it defensively.
            var cleaned = modelOutput.Trim();
            if (cleaned.StartsWith("```"))
            {
                var firstNewline = cleaned.IndexOf('\n');
                var lastFence = cleaned.LastIndexOf("```");
                if (firstNewline >= 0 && lastFence > firstNewline)
                    cleaned = cleaned[(firstNewline + 1)..lastFence].Trim();
            }

            try
            {
                return JsonSerializer.Deserialize<List<ExtractedTaskDto>>(cleaned, JsonOptions)
                    ?? new List<ExtractedTaskDto>();
            }
            catch (JsonException)
            {
                // If the model returned something unparseable, fail soft rather than crash the request.
                return new List<ExtractedTaskDto>();
            }
        }

        // ---------- Gemini response shape (only the fields we need) ----------

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<GeminiCandidate>? Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent? Content { get; set; }
        }

        private class GeminiContent
        {
            [JsonPropertyName("parts")]
            public List<GeminiPart>? Parts { get; set; }
        }

        private class GeminiPart
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }
    }
}
