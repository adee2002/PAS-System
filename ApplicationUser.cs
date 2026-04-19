using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ProjectApprovalSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole UserType { get; set; } = UserRole.Student;

        // Navigation properties
        public virtual ICollection<Project> StudentProjects { get; set; } = new List<Project>();
        public virtual ICollection<Project> SupervisedProjects { get; set; } = new List<Project>();
        public virtual ICollection<SupervisorExpertise> ExpertiseAreas { get; set; } = new List<SupervisorExpertise>();
        public virtual ICollection<Match> StudentMatches { get; set; } = new List<Match>();
        public virtual ICollection<Match> SupervisorMatches { get; set; } = new List<Match>();
    }

    public enum UserRole
    {
        Student = 1,
        Supervisor = 2,
        ModuleLeader = 3,
        SystemAdmin = 4
    }
}