using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskmanAPI.Contexts;
using TaskmanAPI.Controllers;
using TaskmanAPI.Exceptions;
using TaskmanAPI.Model;
using TaskmanAPI.Models;
using TaskmanAPI.Services;
using Xunit;
using TaskStatus = TaskmanAPI.Enums.TaskStatus;

public class ProjTasksControllerTests
{
    private DefaultContext _context;
    private ProjTasksController _controller;
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private Mock<ProjTasksService> _mockProjTasksService;
    private Mock<UserService> _mockUserService;

    public ProjTasksControllerTests()
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

        // Create mocks
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockProjTasksService = new Mock<ProjTasksService>(_context, _mockHttpContextAccessor.Object);
        _mockUserService = new Mock<UserService>(_context);

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
        _mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

        // Create controller
        _controller = new ProjTasksController(_context, _mockHttpContextAccessor.Object);
    }

    private void SeedDatabase()
    {
        var users = new List<User>
        {
            new User { Id = "user1", UserName = "Elena" },
            new User { Id = "user2", UserName = "Antonio" }
        };
        _context.Users.AddRange(users);

        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", Description = "desc1" },
            new Project { Id = 2, Name = "Project 2", Description = "desc2" }
        };
        _context.Projects.AddRange(projects);

        var tasks = new List<ProjTask>
        {
            new ProjTask { Id = 1, Title = "Task 1", ProjectId = 1, Description = "desc1", Status = TaskStatus.InProgress},
            new ProjTask { Id = 2, Title = "Task 2", ProjectId = 1, Description = "desc2", Status = TaskStatus.Open, ParentId = 1},
            new ProjTask { Id = 3, Title = "Task 3", ProjectId = 2, Description = "desc3", Status = TaskStatus.InProgress},
            new ProjTask { Id = 4, Title = "Task 4", ProjectId = 3, Description = "desc4", Status = TaskStatus.Open}

        };
        _context.ProjTasks.AddRange(tasks);
        
        var rolePerProjects = new List<RolePerProject>
        {
            new RolePerProject { UserId = "user1", ProjectId = 1, RoleName = "Owner"},
            new RolePerProject { UserId = "user1", ProjectId = 2, RoleName = "User"},
            new RolePerProject { UserId = "user2", ProjectId = 3, RoleName = "Admin"},
            new RolePerProject { UserId = "user2", ProjectId = 2, RoleName = "User"}

        };
        _context.RolePerProjects.AddRange(rolePerProjects);

        var usertasks = new List<UserTasks>
        {
            new UserTasks { TaskId = 1, UserId = "user1" },
            new UserTasks { TaskId = 2, UserId = "user1" },
           
        };
        _context.UserTasks.AddRange(usertasks);

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserTasks_ReturnsOkResult()
    {
        var projectId = 1;
        var result = await _controller.GetUserTasks(projectId);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<ProjTask>>(okResult.Value);
        Assert.Equal(1, returnValue.Count);
        
    }
    [Fact]
    public async Task GetUserTasks_WhenProjectDoesNotExist()
    {
        var projectId = 9;
        try
        {
            var result = await _controller.GetUserTasks(projectId);
            Assert.True(false,"The project  exist in a scenerio where it shouldn`t");
        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("Project does not exist",e.Message);
        }
        
        
    }

    [Fact]
    public async Task GetAllTasks_ReturnsOkResult()
    {
        var projectId = 1;
        var result = await _controller.GetAllTasks(projectId);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<ProjTask>>(okResult.Value);
        Assert.Equal(1, returnValue.Count);
    }

    [Fact]
    public async Task GetAllTasks_WhenTheProjectDoesNotExist()
    {
        var projectId = 9;
        try
        {
            var result = await _controller.GetAllTasks(projectId);
            Assert.True(false,"The project  exist in a scenerio where it shouldn`t");
        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("Project does not exist",e.Message);
        }
        
    }
    [Fact]
    public async Task GetProjTask_ReturnsOkResult_WhenTaskExistAndHaveAccess()
    {
        // Arrange
        var taskId = 1;
        var task = await _context.ProjTasks.FindAsync(taskId);
        var result = await _controller.GetProjTask(taskId);
        
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProjTask>(okResult.Value);
        Assert.Equal(task, returnValue);
    }
    [Fact]
    public async Task GetProjTask_WithoutAccess()
    {
        var taskId = 4;
        try
        {
            var task = await _context.ProjTasks.FindAsync(taskId);
            Assert.NotNull(task);
            
            var result = await _controller.GetProjTask(taskId);
            Assert.True(false);
        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("Task does not exist",e.Message);
        }
        
    }

    [Fact]
    public async Task GetProjTask_WhenProjectDoesNotExist()
    {
        var taskId = 9;
        try
        {
            var task = await _context.ProjTasks.FindAsync(taskId);
            Assert.Null(task);

            var result = await _controller.GetProjTask(taskId);
            Assert.True(false);
        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("Task does not exist", e.Message);
        }
    }

    [Fact]
    public async Task PutProjTask_ReturnsOkResult_WithEditedTask()
    {
        // Arrange
        var task = await _context.ProjTasks.FindAsync(1);
        task.Title = "Update";
       
        // Act
        var result = await _controller.PutProjTask(task);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProjTask>(okResult.Value);
        Assert.Equal("Update", returnValue.Title);
    }
    [Fact]
    public async Task PutProjTask_WithoutAccess()
    {
        try
        {
            // Arrange
            var task = await _context.ProjTasks.FindAsync(4);
            task.Title = "Update";
       
            // Act
            var result = await _controller.PutProjTask(task);
            Assert.True(false,"The example is not right");

        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("Project does not exist",e.Message);
        }
    }

    [Fact]
    public async Task PostProjTask_ReturnsOkResult_WithCreatedTask()
    {
        // Arrange
        var task = new ProjTask { Id = 5, Title = "Task 5", ProjectId = 2, Description = "desc5", Status = TaskStatus.Open };
        var result = await _controller.PostProjTask(task);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var found = await _context.ProjTasks.FindAsync(task.Id);
        Assert.NotNull(found);
    }

    [Fact]
    public async Task DeleteProjTask_ReturnsNoContentResult()
    {
        // Arrange
        var taskId = 3;
        // Act
        var result = await _controller.DeleteProjTask(taskId);
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task AssignUsers_ReturnsOkResult()
    {
        // Arrange
        var taskId = 3; // we give an open task to a user with access to project
        var username = "Antonio";
        var userId = "user2";
        var task = await _context.ProjTasks.FindAsync(taskId);
        
        // Act
        var result = await _controller.AssignUsers(taskId, username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProjTask>(okResult.Value);
        Assert.Equal(task, returnValue);
    }

    [Fact]
    public async Task RemoveUsers_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var username = "Elena";
        var userId = "user1";
        var task = await _context.ProjTasks.FindAsync(taskId);
        
        // Act
        var result = await _controller.RemoveUsers(taskId, username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProjTask>(okResult.Value);
        Assert.Equal(task, returnValue);
    }

    [Fact]
    public async Task ChangeStatus_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var status = "Done";
        
        // Act
        var result = await _controller.ChangeStatus(taskId, status);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ProjTask>(okResult.Value);
        Assert.Equal(TaskStatus.Done, returnValue.Status);
    }

    [Fact]
    public async Task GetSubtasks_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        // Act
        var result = await _controller.GetSubtasks(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<ProjTask>>(okResult.Value);
        Assert.Equal(1, returnValue.Count);
    }
}
