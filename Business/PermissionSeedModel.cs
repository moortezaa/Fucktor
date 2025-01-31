namespace Business
{
    internal class PermissionSeedModel
    {
        public string Title { get; set; }
        public string? Group { get; set; }
        public bool IsAdmin { get; set; }

        public PermissionSeedModel(string name, string? group, bool isAdmin)
        {
            Title = name;
            Group = group;
            IsAdmin = isAdmin;
        }
    }
}
