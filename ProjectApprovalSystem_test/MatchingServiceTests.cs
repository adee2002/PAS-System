using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Services;
using ProjectApprovalSystem.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using MatchModel = ProjectApprovalSystem.Models.Match;

namespace ProjectApprovalSystem_Tests
{
    public class MatchingServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ExpressInterestAsync_ValidProject_ReturnsTrueAndCreatesMatch()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var student = new ApplicationUser
            {
                Id = "student1",
                Email = "student@test.com",
                FirstName = "Test",
                LastName = "Student",
                EmailConfirmed = true
            };

            var project = new Project
            {
                Id = 1,
                Title = "Test Project",
                Abstract = "Test Abstract",
                TechnicalStack = "ASP.NET Core",
                ResearchAreaId = 1,
                StudentId = "student1",
                Status = ProjectStatus.Pending,
                SubmittedDate = DateTime.UtcNow
            };

            var researchArea = new ResearchArea
            {
                Id = 1,
                Name = "Artificial Intelligence",
                Description = "AI Testing"
            };

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            // Act
            var result = await service.ExpressInterestAsync(1, "supervisor1");

            // Assert
            Assert.True(result);

            // Use full namespace for Match model to avoid conflict with Moq.Match
            var match = await context.Matches.FirstOrDefaultAsync();
            Assert.NotNull(match);
            Assert.Equal(MatchStatus.Pending, match.Status);
            Assert.Equal(1, match.ProjectId);
            Assert.Equal("supervisor1", match.SupervisorId);

            var updatedProject = await context.Projects.FindAsync(1);
            Assert.Equal(ProjectStatus.UnderReview, updatedProject.Status);
        }

        [Fact]
        public async Task ExpressInterestAsync_AlreadyMatchedProject_ReturnsFalse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var project = new Project
            {
                Id = 1,
                Title = "Matched Project",
                Abstract = "Test",
                TechnicalStack = "Test",
                ResearchAreaId = 1,
                StudentId = "student1",
                Status = ProjectStatus.Matched
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();

            // Act
            var result = await service.ExpressInterestAsync(1, "supervisor1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmMatchAsync_ValidMatch_ConfirmsAndRevealsIdentity()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var student = new ApplicationUser
            {
                Id = "student1",
                Email = "student@test.com",
                FirstName = "Test",
                LastName = "Student"
            };

            var supervisor = new ApplicationUser
            {
                Id = "supervisor1",
                Email = "supervisor@test.com",
                FirstName = "Test",
                LastName = "Supervisor"
            };

            var project = new Project
            {
                Id = 1,
                Title = "Test Project",
                Abstract = "Test",
                TechnicalStack = "Test",
                ResearchAreaId = 1,
                StudentId = "student1",
                Status = ProjectStatus.UnderReview
            };

            // Use full namespace for Match model
            var match = new ProjectApprovalSystem.Models.Match
            {
                Id = 1,
                ProjectId = 1,
                StudentId = "student1",
                SupervisorId = "supervisor1",
                Status = MatchStatus.Pending,
                InterestDate = DateTime.UtcNow,
                IsIdentityRevealed = false
            };

            context.Users.AddRange(student, supervisor);
            context.Projects.Add(project);
            context.Matches.Add(match);
            await context.SaveChangesAsync();

            // Act
            var result = await service.ConfirmMatchAsync(1, "supervisor1");

            // Assert
            Assert.True(result);

            var updatedMatch = await context.Matches.FindAsync(1);
            Assert.Equal(MatchStatus.Confirmed, updatedMatch.Status);
            Assert.True(updatedMatch.IsIdentityRevealed);
            Assert.NotNull(updatedMatch.ConfirmedDate);

            var updatedProject = await context.Projects.FindAsync(1);
            Assert.Equal(ProjectStatus.Matched, updatedProject.Status);
            Assert.Equal("supervisor1", updatedProject.SupervisorId);
        }

        [Fact]
        public async Task ConfirmMatchAsync_NonExistentMatch_ReturnsFalse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            // Act
            var result = await service.ConfirmMatchAsync(999, "supervisor1");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAvailableProjectsForSupervisorAsync_ReturnsOnlyMatchingExpertise()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new MatchingService(context);

            var supervisor = new ApplicationUser
            {
                Id = "supervisor1",
                Email = "supervisor@test.com",
                FirstName = "Test",
                LastName = "Supervisor"
            };

            var researchArea1 = new ResearchArea { Id = 1, Name = "AI", Description = "AI" };
            var researchArea2 = new ResearchArea { Id = 2, Name = "Web", Description = "Web" };

            var project1 = new Project
            {
                Id = 1,
                Title = "AI Project",
                Abstract = "Test",
                TechnicalStack = "Test",
                ResearchAreaId = 1,
                StudentId = "student1",
                Status = ProjectStatus.Pending
            };

            var project2 = new Project
            {
                Id = 2,
                Title = "Web Project",
                Abstract = "Test",
                TechnicalStack = "Test",
                ResearchAreaId = 2,
                StudentId = "student2",
                Status = ProjectStatus.Pending
            };

            var expertise = new SupervisorExpertise
            {
                SupervisorId = "supervisor1",
                ResearchAreaId = 1
            };

            context.ResearchAreas.AddRange(researchArea1, researchArea2);
            context.Users.Add(supervisor);
            context.Projects.AddRange(project1, project2);
            context.SupervisorExpertises.Add(expertise);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAvailableProjectsForSupervisorAsync("supervisor1");

            // Assert
            Assert.Single(result);
            Assert.Equal("AI Project", result[0].Title);
        }
    }
}