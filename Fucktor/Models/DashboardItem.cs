﻿namespace Fucktor.Models
{

    public class DashboardItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Display { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<DashboardItem> Childs { get; set; } = new List<DashboardItem>();
        public string Link { get; set; } = string.Empty;
        public string Icon { get; set; } = "home";
    }
}
