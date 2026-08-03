using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class Meeting
{
    public int Id { get; set; }

    public int WorkspaceId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime MeetingDate { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Decision> Decisions { get; set; } = new List<Decision>();

    public virtual ICollection<MeetingNote> MeetingNotes { get; set; } = new List<MeetingNote>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual Workspace Workspace { get; set; } = null!;
}
