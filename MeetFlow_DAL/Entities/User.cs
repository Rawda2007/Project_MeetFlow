using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; } // international format, e.g. 201234567890 (needed for WhatsApp)

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MeetingNote> MeetingNotes { get; set; } = new List<MeetingNote>();

    public virtual ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<PasswordResetCode> PasswordResetCodes { get; set; } = new List<PasswordResetCode>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = new List<WorkspaceMember>();

    public virtual ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
}
