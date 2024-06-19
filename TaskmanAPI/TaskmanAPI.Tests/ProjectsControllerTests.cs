using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

public class ProjectsControllerTests
{
    private DefaultContext _context;
    private ProjectsController _controller;

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
        var userClaim = new Claim(ClaimTypes.NameIdentifier, "user2");
        var identity = new ClaimsIdentity(new[] { userClaim }, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        var projects = new List<Project>
        {
            new() { Id = 1, Name = "Project 1", Description = "desc1" },
            new() { Id = 2, Name = "Project 2", Description = "desc2" },
            new() { Id = 3, Name = "Project 3", Description = "desc3"}
        };

        _context.Projects.AddRange(projects);

        // Adăugați role per proiect care sunt asociate utilizatorului
        var rolePerProjects = new List<RolePerProject>
        {
            new RolePerProject { UserId = "user1", ProjectId = 1, RoleName = "Owner"},
            new RolePerProject { UserId = "user1", ProjectId = 2, RoleName = "User"},
            new RolePerProject { UserId = "user2", ProjectId = 3, RoleName = "Admin"},
            new RolePerProject { UserId = "user2", ProjectId = 1, RoleName = "User"}

        };
        _context.RolePerProjects.AddRange(rolePerProjects);

        var users = new List<User>
        {
            new User() { Id = "user1", UserName = "Elena" },
            new User() {Id ="user2", UserName = "Antonio"}
        };
        _context.Users.AddRange(users);
        _context.SaveChanges();
    }
    //1) GetUserProjects tests
    [Fact]
    public async Task GetUserProjects_ReturnsAllProjects()
    {
        
        // Act
        var result = await _controller.GetUserProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Project>>(okResult.Value);
        //returnam toate proiectelela care are acces user1
        Assert.Equal(2, returnValue.Count);
    }
//2) GetProject Test
    [Fact]
    public async Task GetProject_WhenProjectExists_ButDontHaveAccess()
    {
        try
        {

            var project = await _context.Projects.FindAsync(3);
            Assert.NotNull(project);
            
            var result = await _controller.GetProject(3); //Project 3 exists but does not have access
            Assert.True(false,"We have access to the given project, so the test does not reach its goal");
        }
        catch (EntityNotFoundException ex)
        {
            Assert.Equal("Project does not exist", ex.Message);
        }
    }
    [Fact]
    public async Task GetProject_WhenProjectExists_AndHaveAccess()
    {
            // Act
            var result = await _controller.GetProject(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var project = Assert.IsType<Project>(okResult.Value);
            
            Assert.Equal(1,project.Id);
       
    }

    [Fact]
    public async Task GetProject_WhenProjectDoesNotExist()
    {
        // Arrange
        var projectId = 99;

        try
        {
            
            // Act
            var result = await _controller.GetProject(projectId);
            Assert.True(false,"The project exits and the user has access to it");
        }
        catch (EntityNotFoundException ex)
        {
            // Assert
            Assert.Equal("Project does not exist", ex.Message);
            
            var found =await _context.Projects.FindAsync(projectId);
            Assert.Null(found);// Value is not null = So the project exists and the test is not valid
        }
    }

//3) Edit
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
        var project = new Project { Id = 1, Name = "Updated", Description = "Description"};

        // Act
        var result = await _controller.Edit(2, project);

        // Asser
        Assert.IsType<ActionResult<Project>>(result);
    }

    [Fact]
    public async Task Edit_WhenProjectDoesNotExist_OrYouDontHaveAccess()
    {
        var projectId = 99;
        try
        {
            // Arrange
            var project = await _context.Projects.FindAsync(projectId);
            
            // Act
            if (project == null)
            {
                var new_project = new Project{Id=projectId, Name="New name", Description = "Description"};
                var result1 = await _controller.Edit(projectId, new_project);
            }
            else
            {
                var result = await _controller.Edit(projectId, project);
            }

            Assert.True(false,"The given project exist and the user has access");

        }
        catch (EntityNotFoundException e)
        {
            // Assert
            Assert.Equal("Project does not exist",e.Message);
        }
    }
    [Fact]
    public async Task Edit_WhenProjectExists_ButYouDontHavePrivilege()
    {
        try
        {
            var project = await _context.Projects.FindAsync(2);
            project.Name = "New project name";

            // Act
            var result = await _controller.Edit(2, project);
            Assert.True(false,"The given project have privileges so the example is not working");
            
        }
        catch (InsufficientMemoryException e)
        {
            // Assert
            Assert.Equal("You do not have the required privileges to edit this project",e.Message);
        }

       
    }
    //4) New
    [Fact]
    public async Task New_Returns_OkObjectResult_For_Valid_Project()
    {
        // Arrange
        var lista = new List<RolePerProject>
        {
            new RolePerProject(){ProjectId = 6,UserId = "user2",RoleName = "Owner"}
        };
        Assert.NotNull(lista);
        var validProject = new Project { Id = 6, Name = "Project 6", Description = "desc6",RolePerProjects = lista};
        // Act
        var result = await _controller.New(validProject) ;

        // Assert
        Assert.NotNull(result);

        var verify = await _context.Projects.FindAsync(6);
        Assert.NotNull(verify);
        
    }
    //5)Delete
    [Fact]
    public async Task Delete_Returns_NoContent()
    {
        // Arrange
        var projectId = 1;
        // You need to have access and privileges to delete a project
        // You are user1 and for project 1 you are the owner
       
        // Act
        var result = await _controller.Delete(projectId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public async Task Delete_WithoutAccess_OrTheProjectDoesNotExist()
    {
        // Arrange
        var projectId = 9;
        try
        {
            // Act
            var result = await _controller.Delete(projectId);
            // Assert
            Assert.True(false,"For this test you are not supposed to have access");
        }
        catch (EntityNotFoundException e)
        {
           Assert.Equal("Project does not exist",e.Message);
        }
    }
    [Fact]
    public async Task Delete_WithAccess_ButWithoutPrivileges()
    {
        // Arrange
        var projectId = 2;
        try
        {
            // Act
            var result = await _controller.Delete(projectId);
            // Assert
            Assert.True(false,"For this test you are not supposed to have access");
        }
        catch (InsufficientPrivilegesException e)
        {
            Assert.Equal("You do not have the required privileges to delete this project",e.Message);
        }
        catch (EntityNotFoundException e)
        {
            Assert.True(false,"The project does not have acces or doesn`t exist");
        }
    }
    
    //6)AddUser
    [Fact]
    public async Task AddUser_Returns_Ok()
    {
        // Arrange
        var projectId = 1; //You are Owner for project 1 you have access and privileges to add a new user
        var username = "Antonio";
        var userId = "user2";
        
        // Act
        var result = await _controller.AddUser(projectId, username);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    [Fact]
    public async Task AddUser_HasAccessButNotPrivileges()
    {
        try
        {
            // Arrange
            var projectId = 2;
            var username = "Antonio";
            var userId = "user2";

            // Act
            var result = await _controller.AddUser(projectId, username);

            // Assert
            Assert.True(false, "the example is not valid for this scenario");
        }
        catch (InsufficientPrivilegesException e)
        {
            Assert.Equal("You do not have the required privileges to add a user to this project", e.Message);
        }
        catch (Exception e)
        {
            Assert.True(false, "the example is not valid for this scenario"); 
        }
        
        
    }
   [Fact]
    public async Task AddUser_ButUserDoesNotExist()
    {
        try
        {
            // Arrange
            var projectId = 1;
            var username = "Arnold";
            var userId = "user5";

            // Act
            var result = await _controller.AddUser(projectId, username);

            // Assert
            Assert.True(false, "the example is not valid for this scenario");
        }
        catch (EntityNotFoundException e)
        {
            Assert.Equal("User does not exist", e.Message);
        }
        catch (Exception e)
        {
            Assert.True(false, "the example is not valid for this scenario"); 
        }
        
    }
    [Fact]
    public async Task AddUser_ButUserIsAlreadyInProject()
    {
        try
        {
            // Arrange
            var projectId = 1;
            var username = "Antonio";
            var userId = "user2";

            // Act
            var result = await _controller.AddUser(projectId, username);
            // Assert
            Assert.True(false, "the example is not valid for this scenario");
        }
        catch (EntityAlreadyExistsException e)
        {
            Assert.Equal("User is already part of the project", e.Message);
        }
        catch (Exception e)
        {
            Assert.True(false, "the example is not valid for this scenario"); 
        }
        
    }
    //7)RemoveUser
    [Fact]
    public async Task RemoveUser_Returns_Ok()
    {
        // Arrange
        var projectId = 1;
        var username = "Antonio";
        var userId = "user2";
        // Act
        var result = await _controller.RemoveUser(projectId, username);

        // Assert
        Assert.IsType<OkResult>(result);
    }
    
    //PromoteUser
    [Fact]
    public async Task PromoteUser_Returns_Ok()
    {
        // Arrange
        var projectId = 1;
        var username = "Antonio";
        var userId = "user2";
        
        // Act
        var result = await _controller.PromoteUser(projectId, username);

        // Assert
        Assert.IsType<OkResult>(result);
        var user = await _context.RolePerProjects.FindAsync(userId,projectId);
        Assert.Equal("Admin",user.RoleName);
    }
    
    [Fact]
    public async Task TransferOwnership_Returns_Ok()
    {
        // Arrange
        var projectId = 1;
        var username = "Antonio";
        var userId = "user2";
    
         // Act
        var result = await _controller.TransferOwnership(projectId, username);

        // Assert
        Assert.IsType<OkResult>(result);
        var user = await _context.RolePerProjects.FindAsync(userId,projectId);
        Assert.Equal("Owner",user.RoleName);

    }
    [Fact]
    public async Task DemoteUser_Returns_Ok()
    {
        // Arrange
        var projectId = 1;
        var username = "Antonio";
        var userId = "user2";
        
        // Act
        var result = await _controller.DemoteUser(projectId, username);

        // Assert
        Assert.IsType<OkResult>(result);
           }
    
    [Fact]
    public async Task GetProjectUsers_Returns_Ok_With_Users()
    {
        // Arrange
        var projectId = 1;
        // Act
        var result = await _controller.GetProjectUsers(projectId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        
    }
    
    [Fact]
    public async Task GetMyRole_Returns_Ok_With_Role()
    {
        // Arrange
        var projectId = 1;
        var role = "Owner";
        
        // Act
        var result = await _controller.GetMyRole(projectId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(role, okResult.Value);
    }



}