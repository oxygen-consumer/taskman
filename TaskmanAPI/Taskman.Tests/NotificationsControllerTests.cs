using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskmanAPI.Contexts;
using TaskmanAPI.Controllers;
using TaskmanAPI.Model;


public class NotificationsControllerTests 
{
    private DefaultContext _context;
    private NotificationsController _controller;

    public NotificationsControllerTests()
    {
        Setup();
    }

    private void Setup()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}")
            .Options;
        _context = new DefaultContext(options);

        // Seed the in-memory database
        SeedDatabase();

        // Create controller
        _controller = new NotificationsController(_context);
    }

    private void SeedDatabase()
    {
        var notifications = new List<Notification>
        {
            new Notification { Id = 1,UserId = "1",Title = "Titlu1", Content = "Test1" },
            new Notification { Id = 2, UserId = "2", Title = "Titlu2", Content = "Test2" }
        };

        _context.Notifications.AddRange(notifications);
        _context.SaveChanges();
    }

    //Test for GetNotifications method 
    [Fact]
    public async Task GetNotifications_ReturnsAllNotifications()
    {
        // Act
        var result = await _controller.GetNotifications();

        // Assert
        var okResult = Assert.IsType<ActionResult<IEnumerable<Notification>>>(result);
        var returnValue = Assert.IsType<List<Notification>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    //Tests for GetNotification method 
    [Fact]
    public async Task GetNotification_ReturnsNotification_WhenNotificationExists()
    {
        // Act
        var result = await _controller.GetNotification(1);

        // Assert
        var okResult = Assert.IsType<ActionResult<Notification>>(result);
        Assert.Equal(1, okResult.Value.Id);
    }

    [Fact]
    public async Task GetNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Act
        var result = await _controller.GetNotification(99);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    //Tester for PutNotification method
    [Fact]
    public async Task PutNotification_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var existingNotification = await _context.Notifications.FindAsync(1);
        existingNotification.Content = "Updated";

        // Act
        var result = await _controller.PutNotification(1, existingNotification);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify the update
        var updatedNotification = await _context.Notifications.FindAsync(1);
        Assert.Equal("Updated", updatedNotification.Content);
    }

    [Fact]
    public async Task PutNotification_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var notification = new Notification { Id = 1, Content = "Updated" };

        // Act
        var result = await _controller.PutNotification(2, notification);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task PutNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Arrange
        var notification = new Notification { Id = 10,UserId = "99",Title = "Titlu99", Content = "Updated" };

        // Act
        var result = await _controller.PutNotification(10, notification);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    
    //Test for PostNotification method
    [Fact]
    public async Task PostNotification_ReturnsCreatedAtAction_WhenPostIsSuccessful()
    {
        // Arrange
        var notification = new Notification { Id = 3,UserId = "3",Title = "Title3", Content = "New" };

        // Act
        var result = await _controller.PostNotification(notification);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Notification>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        Assert.Equal("GetNotification", createdAtActionResult.ActionName);
        Assert.Equal(notification.Id, ((Notification)createdAtActionResult.Value).Id);
    }

    //Test for DeleteNotification
    [Fact]
    public async Task DeleteNotification_ReturnsNoContent_WhenDeleteIsSuccessful()
    {
        // Act
        var result = await _controller.DeleteNotification(1);

        // Assert
        Assert.IsType<NoContentResult>(result); 
    }

    [Fact]
    public async Task DeleteNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteNotification(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    
}
