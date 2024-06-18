using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskmanAPI.Contexts;
using TaskmanAPI.Controllers;
using TaskmanAPI.Exceptions;
using TaskmanAPI.Models;

public class ProjectsControllerTests
{
    private DefaultContext _context;

    private ProjectsController _controller;
    //private IHttpContextAccessor _contextAccessor;

    public ProjectsControllerTests()
    {
        Setup();
    }

    private void Setup()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
            .Options;
        _context = new DefaultContext(options);

        if (_context == null) throw new Exception("_context is not initialized.");

        // Seed the in-memory database
        SeedDatabase();

        // Create a mock for IHttpContextAccessor
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        // Create a ClaimsPrincipal with the necessary claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "user1")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Create a mock HttpContext and set the User property
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(context => context.User).Returns(claimsPrincipal);

        // Set the HttpContext in the IHttpContextAccessor mock
        mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

        // Create controller
        _controller = new ProjectsController(_context, mockHttpContextAccessor.Object);
    }

    private void SeedDatabase()
    {
        var userClaim = new Claim(ClaimTypes.NameIdentifier, "user1");
        var identity = new ClaimsIdentity(new[] { userClaim }, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Project 1", Description = "desc1" },
            new() { Id = 2, Name = "Project 2", Description = "desc2" }
        };

        _context.Projects.AddRange(projects);

        // Adăugați role per proiect care sunt asociate utilizatorului
        var rolePerProjects = new List<RolePerProject>
        {
            new RolePerProject { UserId = "user1", ProjectId = 1, RoleName = "Admin"},
            new RolePerProject { UserId = "user1", ProjectId = 2 ,RoleName = "User"}
        };
        _context.RolePerProjects.AddRange(rolePerProjects);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserProjects_ReturnsAllProjects()
    {
        
        // Act
        var result = await _controller.GetUserProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Project>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProject_ReturnsProject_WhenProjectExists()
    {
        
        // Act
        var result = await _controller.GetProject(1);

        // Assert
        Assert.NotNull(result); // Verificăm că rezultatul nu este null

        if (result != null)
        {
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value); // Verificăm că valoarea rezultatului nu este null

            if (okResult.Value != null)
            {
                var project = Assert.IsType<Project>(okResult.Value);
                Assert.Equal(1, project.Id);
            }
        }
    }

    [Fact]
    public async Task GetProject_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectId = 99;

        try
        {
            // Act
            var result = await _controller.GetProject(projectId);

            // Dacă nu se aruncă excepția, asigură-te că este un rezultat de tip `NotFound`
            Assert.IsType<NotFoundResult>(result.Result);
        }
        catch (EntityNotFoundException ex)
        {
            // Assert
            Assert.Equal("Project does not exist", ex.Message);
        }
    }


    [Fact]
    public async Task Edit_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var existingProject = await _context.Projects.FindAsync(1);
        existingProject.Name = "Updated";

        // Act
        var result = await _controller.Edit(1, existingProject);

        // Assert
        Assert.IsType<ActionResult<Project>>(result);
        var updatedProject = await _context.Projects.FindAsync(1);
        Assert.Equal("Updated", updatedProject.Name);
    }

    [Fact]
    public async Task Edit_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Updated" };

        // Act
        var result = await _controller.Edit(2, project);

        // Asser
        Assert.IsType<ActionResult<Project>>(result);
    }

    [Fact]
    public async Task Edit_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        var project = new Project { Id = 10, Name = "Project 10" };

        // Act
        var result = await _controller.Edit(10, project);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}