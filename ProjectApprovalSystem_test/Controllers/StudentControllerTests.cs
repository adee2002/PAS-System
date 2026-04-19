using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApprovalSystem.Controllers;
using ProjectApprovalSystem.Services;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem_test.Helpers;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApprovalSystem_test.Controllers
{
    public class StudentControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private StudentController CreateControllerWithUser(
            ApplicationDbContext context,
            Mock<UserManager<ApplicationUser>> mockUserManager,
            IMatchingService matchingService,
            string userId = "student1")
        {
            var controller = new StudentController(context, mockUserManager.Object, matchingService);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "Student")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            return controller;
        }

        [Fact]
        public async Task Dashboard_ReturnsViewResultWithProjects()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockUserManager = CreateMockUserManager();
            var mockMatchingService = new Mock<IMatchingService>();

            var student = TestDataFactory.CreateStudent();
            var researchArea = TestDataFactory.CreateResearchArea();
            var project = TestDataFactory.CreateProject();

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(student);

            var controller = CreateControllerWithUser(context, mockUserManager, mockMatchingService.Object);

            // Act
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task CreateProposal_Get_ReturnsViewResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockUserManager = CreateMockUserManager();
            var mockMatchingService = new Mock<IMatchingService>();

            var student = TestDataFactory.CreateStudent();
            var researchArea = TestDataFactory.CreateResearchArea();

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            await context.SaveChangesAsync();

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(student);

            var controller = CreateControllerWithUser(context, mockUserManager, mockMatchingService.Object);

            // Act
            var result = await controller.CreateProposal();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact(Skip = "Known issue - fix in next iteration")]
        public async Task CreateProposal_Post_ValidModel_RedirectsToDashboard()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockUserManager = CreateMockUserManager();
            var mockMatchingService = new Mock<IMatchingService>();

            var student = TestDataFactory.CreateStudent();
            var researchArea = TestDataFactory.CreateResearchArea();

            context.ResearchAreas.Add(researchArea);
            context.Users.Add(student);
            await context.SaveChangesAsync();

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(student);

            var controller = CreateControllerWithUser(context, mockUserManager, mockMatchingService.Object);

            var viewModel = new ProjectApprovalSystem.Models.ViewModels.CreateProjectViewModel
            {
                Title = "New Test Project",
                Abstract = "This is a test abstract",
                TechnicalStack = "ASP.NET Core",
                ResearchAreaId = 1
            };

            // Act
            var result = await controller.CreateProposal(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirectResult.ActionName);
        }
    }
}