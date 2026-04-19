using ProjectApprovalSystem.Models;
using System;

namespace ProjectApprovalSystem_test.Helpers
{
    public static class TestDataFactory
    {
        public static ApplicationUser CreateStudent(string id = "student1", string email = "student@test.com")
        {
            return new ApplicationUser
            {
                Id = id,
                Email = email,
                UserName = email,
                FirstName = "Test",
                LastName = "Student",
                UserType = UserRole.Student,
                EmailConfirmed = true
            };
        }

        public static ApplicationUser CreateSupervisor(string id = "supervisor1", string email = "supervisor@test.com")
        {
            return new ApplicationUser
            {
                Id = id,
                Email = email,
                UserName = email,
                FirstName = "Test",
                LastName = "Supervisor",
                UserType = UserRole.Supervisor,
                EmailConfirmed = true
            };
        }

        public static Project CreateProject(
            int id = 1,
            string title = "Test Project",
            string studentId = "student1",
            ProjectStatus status = ProjectStatus.Pending)
        {
            return new Project
            {
                Id = id,
                Title = title,
                Abstract = "This is a test project abstract.",
                TechnicalStack = "ASP.NET Core, SQL Server",
                ResearchAreaId = 1,
                StudentId = studentId,
                Status = status,
                SubmittedDate = DateTime.UtcNow
            };
        }

        public static Match CreateMatch(
            int id = 1,
            int projectId = 1,
            string studentId = "student1",
            string supervisorId = "supervisor1",
            MatchStatus status = MatchStatus.Pending)
        {
            return new Match
            {
                Id = id,
                ProjectId = projectId,
                StudentId = studentId,
                SupervisorId = supervisorId,
                Status = status,
                InterestDate = DateTime.UtcNow,
                IsIdentityRevealed = false
            };
        }

        public static ResearchArea CreateResearchArea(int id = 1, string name = "Artificial Intelligence")
        {
            return new ResearchArea
            {
                Id = id,
                Name = name,
                Description = "AI and Machine Learning"
            };
        }
    }
}