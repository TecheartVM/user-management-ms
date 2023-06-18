using System.Net;
using UserManagement.Domain.Entities;
using UserManagement.Tests.Integration.ControllerTests.Helpers;
using UserManagement.Tests.Unit.ControllerTests;
using Xunit;

namespace UserManagement.Tests.Integration.ControllerTests;

public class DeleteUserTests : UsersControllerIntegrationTestBase
{
    [Fact]
    public async Task DeleteUser_ReturnsOk_AsUser()
    {
        // Arrange
        var user = await AddNewUserAsync();
        string path = $"{_controllerPath}/{user.Id}";

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(HttpMethod.Delete, user, path);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        await AssertUserPresenceAsync(user.Id, false);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOk_AsAdmin()
    {
        // Arrange
        var toDelete = await AddNewUserAsync();
        string path = $"{_controllerPath}/{toDelete.Id}";

        var admin = new User
        {
            Name = "admin"
        };

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(HttpMethod.Delete, admin, path);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        await AssertUserPresenceAsync(toDelete.Id, false);
    }

    [Fact]
    public async Task DeleteUser_ReturnsUnauthorized()
    {
        // Arrange
        var user = await AddNewUserAsync();
        string path = $"{_controllerPath}/{user.Id}";

        // Act
        HttpResponseMessage response = await _client.DeleteAsync(path);

        // Assert
        AssertStatusCode(response, HttpStatusCode.Unauthorized);
        await AssertUserPresenceAsync(user.Id, true);
    }

    [Fact]
    public async Task DeleteUser_ReturnsForbidden()
    {
        // Arrange
        var executor = UserDataGenerator.GetRandomUser();
        var toDelete = await AddNewUserAsync();
        string path = $"{_controllerPath}/{toDelete.Id}";
        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(HttpMethod.Delete, executor, path);

        // Assert
        AssertStatusCode(response, HttpStatusCode.Forbidden);
        await AssertUserPresenceAsync(toDelete.Id, true);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound()
    {
        var toDelete = UserDataGenerator.GetRandomUser();
        string path = $"{_controllerPath}/{toDelete.Id}";

        var executor = new User
        {
            Name = "admin"
        };

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(HttpMethod.Delete, executor, path);

        // Assert
        AssertStatusCode(response, HttpStatusCode.NotFound);
    }
}