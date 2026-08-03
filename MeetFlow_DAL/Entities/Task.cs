using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class Task
{
    public int Id { get; set; }

    public int MeetingId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? AssignedTo { get; set; }

    public DateTime? DueDate { get; set; }

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Meeting Meeting { get; set; } = null!;
}
