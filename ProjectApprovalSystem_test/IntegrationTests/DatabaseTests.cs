using Xunit;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem_test.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApprovalSystem_test.IntegrationTests
{
    public class DatabaseTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Database_CanAddAndRetrieveProject()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var student = TestDataFactory.CreateStudent();
            var researchArea = TestDataFactory.CreateResearchArea();
            var project = TestDataFactory.CreateProject();

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            context.Projects.Add(project);

            // Act
            await context.SaveChangesAsync();

            // Assert
            var retrievedProject = await context.Projects.FirstOrDefaultAsync(p => p.Id == 1);
            Assert.NotNull(retrievedProject);
            Assert.Equal("Test Project", retrievedProject.Title);
        }

        [Fact]
        public async Task Database_CanAddAndRetrieveMatch()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var student = TestDataFactory.CreateStudent();
            var supervisor = TestDataFactory.CreateSupervisor();
            var researchArea = TestDataFactory.CreateResearchArea();
            var project = TestDataFactory.CreateProject();
            var match = TestDataFactory.CreateMatch();

            context.ResearchAreas.Add(researchArea);
            context.Users.AddRange(student, supervisor);
            context.Projects.Add(project);
            context.Matches.Add(match);

            // Act
            await context.SaveChangesAsync();

            // Assert
            var retrievedMatch = await context.Matches
                .Include(m => m.Project)
                .Include(m => m.Student)
                .Include(m => m.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == 1);

            Assert.NotNull(retrievedMatch);
            Assert.Equal(MatchStatus.Pending, retrievedMatch.Status);
            Assert.NotNull(retrievedMatch.Project);
            Assert.NotNull(retrievedMatch.Student);
            Assert.NotNull(retrievedMatch.Supervisor);
        }

        [Fact]
        public async Task Database_CanUpdateProjectStatus()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var student = TestDataFactory.CreateStudent();
            var researchArea = TestDataFactory.CreateResearchArea();
            var project = TestDataFactory.CreateProject(status: ProjectStatus.Pending);

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            // Act
            var projectToUpdate = await context.Projects.FindAsync(1);
            projectToUpdate.Status = ProjectStatus.Matched;
            await context.SaveChangesAsync();

            // Assert
            var updatedProject = await context.Projects.FindAsync(1);
            Assert.Equal(ProjectStatus.Matched, updatedProject.Status);
        }

        [Fact]
        public async Task Database_CanAddSupervisorExpertise()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var supervisor = TestDataFactory.CreateSupervisor();
            var researchArea = TestDataFactory.CreateResearchArea();

            context.Users.Add(supervisor);
            context.ResearchAreas.Add(researchArea);
            await context.SaveChangesAsync();

            var expertise = new SupervisorExpertise
            {
                SupervisorId = "supervisor1",
                ResearchAreaId = 1
            };

            // Act
            context.SupervisorExpertises.Add(expertise);
            await context.SaveChangesAsync();

            // Assert
            var retrievedExpertise = await context.SupervisorExpertises
                .FirstOrDefaultAsync(e => e.SupervisorId == "supervisor1");

            Assert.NotNull(retrievedExpertise);
            Assert.Equal(1, retrievedExpertise.ResearchAreaId);
        }
    }
}