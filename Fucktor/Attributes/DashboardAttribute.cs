namespace Fucktor.Attributes
{
    public class DashboardAttribute : Attribute
    {
        public string Icon { get; set; } = "home";
        public int Order { get; set; }
        public string? ParentAction { get; set; }
        public DashboardAttribute(string icon)
        {
            Icon = icon;
        }
    }
}
