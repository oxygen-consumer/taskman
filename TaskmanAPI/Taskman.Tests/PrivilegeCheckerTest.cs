// MyProject.Tests/PrivilegeCheckerTests.cs
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskmanAPI.Contexts;
using TaskmanAPI.Enums;
using TaskmanAPI.Models; 
using TaskmanAPI.Services;
using Xunit;

namespace MyProject.Tests
{
    public class PrivilegeCheckerTests : IDisposable
    {
        private readonly DefaultContext _context;
        private readonly ClaimsPrincipal _mockUser;
        private readonly PrivilegeChecker _privilegeChecker;

        public PrivilegeCheckerTests()
        {
            // Configurăm opțiunile pentru DbContextOptions
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Inițializăm contextul cu opțiunile definite
            _context = new DefaultContext(options);

            // Populăm contextul cu datele de test
            SeedDatabase();

            // Configurăm un mock pentru utilizator
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _mockUser = new ClaimsPrincipal(identity);

            // Inițializăm PrivilegeChecker cu contextul și utilizatorul simulat
            _privilegeChecker = new PrivilegeChecker(_context, _mockUser);
        }

        // Metodă pentru popularea bazei de date cu date de test
        private void SeedDatabase()
        {
            var rolePerProjects = new List<RolePerProject>
            {
                new RolePerProject { ProjectId = 1, UserId = "user1", RoleName = "User" }
            };
            _context.RolePerProjects.AddRange(rolePerProjects);
            _context.SaveChanges();
        }

        [Fact]
        public void HasAccessToProject_ReturnsTrue_WhenUserHasAccess()
        {
            // Act
            var result = _privilegeChecker.HasAccessToProject(1);

            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void HasAccessToProject_ReturnsFalse_WhenUserDoesNotHaveAccess()
        {
            // Arrange
            var rolePerProjects = new List<RolePerProject>();
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(rolePerProjects);
            _context.RolePerProjects = mockDbSet;

            // Act
            var result = _privilegeChecker.HasAccessToProject(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasPrivilege_ReturnsTrue_WhenUserHasSufficientRole()
        {
            // Arrange
            var rolePerProjects = new List<RolePerProject>
            {
                new RolePerProject { ProjectId = 1, UserId = "user1", RoleName = "Admin" }
            };
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(rolePerProjects);
            _context.RolePerProjects = mockDbSet;

            // Act
            var result = _privilegeChecker.HasPrivilege(1, Role.Admin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasPrivilege_ReturnsFalse_WhenUserHasInsufficientRole()
        {
            // Arrange
            var rolePerProjects = new List<RolePerProject>
            {
                new RolePerProject { ProjectId = 1, UserId = "user1", RoleName = "User" }
            };
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(rolePerProjects);
            _context.RolePerProjects = mockDbSet;

            // Act
            var result = _privilegeChecker.HasPrivilege(1, Role.Admin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasPrivilege_ReturnsFalse_WhenUserHasNoRole()
        {
            // Arrange
            var rolePerProjects = new List<RolePerProject>();
            var mockDbSet = DbSetMockHelper.CreateMockDbSet(rolePerProjects);
            _context.RolePerProjects = mockDbSet;

            // Act
            var result = _privilegeChecker.HasPrivilege(1, Role.User);

            // Assert
            Assert.False(result);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
