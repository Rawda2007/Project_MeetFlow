using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.Extensions.Configuration;
using MeetFlow.BLL.Interfaces;

namespace MeetFlow.BLL.Services
{
    // Sends messages through Meta's WhatsApp Business Cloud API.
    // Setup: developers.facebook.com -> Create App (Business) -> Add "WhatsApp" product.
    // You'll get a Phone Number ID and a temporary Access Token (24h in test mode) —
    // put both in appsettings.json under "WhatsApp".
    //
    // IMPORTANT test-mode limitation: Meta only lets you message numbers you've added
    // and verified under WhatsApp > API Setup > "To" (max 5 numbers in test mode).
    // Messaging any other number will fail until the app goes through Meta's review.
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public WhatsAppService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task SendTaskAssignedMessageAsync(string phoneNumber, string taskTitle, DateTime? dueDate)
        {
            var accessToken = _config["WhatsApp:AccessToken"];
            var phoneNumberId = _config["WhatsApp:PhoneNumberId"];
            var apiVersion = _config["WhatsApp:ApiVersion"] ?? "v20.0";

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(phoneNumberId))
                throw new InvalidOperationException(
                    "WhatsApp settings are missing. Fill in WhatsApp:AccessToken and WhatsApp:PhoneNumberId in appsettings.json.");

            var url = $"https://graph.facebook.com/{apiVersion}/{phoneNumberId}/messages";

            var dueText = dueDate is null ? "no due date" : dueDate.Value.ToString("yyyy-MM-dd");
            var body = $"📌 New task assigned to you: *{taskTitle}*\nDue: {dueText}";

            var requestBody = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber, // international format, no leading '+', e.g. 201234567890
                type = "text",
                text = new { body }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(requestBody)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
