using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectApprovalSystem.Controllers;
using ProjectApprovalSystem.Data;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using ProjectApprovalSystem.Services;
using ProjectApprovalSystem_test.Helpers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ProjectApprovalSystem_test.Controllers
{
    public class SupervisorControllerTests
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

        private SupervisorController CreateControllerWithUser(
            ApplicationDbContext context,
            Mock<UserManager<ApplicationUser>> mockUserManager,
            IMatchingService matchingService,
            string userId = "supervisor1",
            string role = "Supervisor")
        {
            var controller = new SupervisorController(context, mockUserManager.Object, matchingService);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
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
        public async Task Dashboard_ReturnsViewResultWithModel()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockUserManager = CreateMockUserManager();
            var mockMatchingService = new Mock<IMatchingService>();

            mockMatchingService.Setup(x => x.GetAvailableProjectsForSupervisorAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<BlindReviewViewModel>());

            var user = TestDataFactory.CreateSupervisor();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = CreateControllerWithUser(context, mockUserManager, mockMatchingService.Object);

            // Act
            var result = await controller.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task ManageExpertise_ReturnsViewResult()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockUserManager = CreateMockUserManager();
            var mockMatchingService = new Mock<IMatchingService>();

            var user = TestDataFactory.CreateSupervisor();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var controller = CreateControllerWithUser(context, mockUserManager, mockMatchingService.Object);

            // Act
            var result = await controller.ManageExpertise();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
        }
    }
}