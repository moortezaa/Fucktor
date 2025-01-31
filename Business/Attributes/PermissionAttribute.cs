namespace Business.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAttribute : Attribute
    {
        public string Permission { get; }
        public bool IsAdmin { get; }

        public PermissionAttribute(string permission, bool isAdmin = false)
        {
            Permission = permission;
            IsAdmin = isAdmin;
        }
    }
}
