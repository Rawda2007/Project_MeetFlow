namespace MeetFlow_DAL.Common
{
    // Lives in the DAL (not the BLL) so both the DAL repositories and the BLL services
    // can reference it without creating a circular project reference.
    public static class WorkspaceRoles
    {
        public const string Owner = "Owner";
        public const string Member = "Member";
    }
}
