using System;
using System.Collections.Generic;

namespace MeetFlow_DAL.Entities;

public partial class WorkspaceMember
{
    public int Id { get; set; }

    public int WorkspaceId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Workspace Workspace { get; set; } = null!;
}
