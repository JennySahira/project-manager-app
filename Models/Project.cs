using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProjectManagerApp.Models
{
    public enum ProjectStatus
    {
        Started,
        Completed
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public ProjectStatus Status { get; set; }

        public string? IconFileName { get; set; }


        [BindNever]
        public string? UserId { get; set; } //Skapad med hjälp av ChatGpt

    }
}
