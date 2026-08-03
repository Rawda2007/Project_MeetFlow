using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class MeetingNote
{
    public int Id { get; set; }

    public int MeetingId { get; set; }

    public string Content { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Meeting Meeting { get; set; } = null!;
}
