using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class Decision
{
    public int Id { get; set; }

    public int MeetingId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Meeting Meeting { get; set; } = null!;
}
