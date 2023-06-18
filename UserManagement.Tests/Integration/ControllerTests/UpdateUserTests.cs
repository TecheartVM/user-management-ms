using System.Net.Http.Json;
using System.Net;
using UserManagement.Api.DataTransfer;
using UserManagement.Tests.Unit.ControllerTests;
using Xunit;
using UserManagement.Domain.Entities;
using UserManagement.Tests.Integration.ControllerTests.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserManagement.Tests.Integration.ControllerTests;

public class UpdateUserTests : UsersControllerIntegrationTestBase
{
    [Fact]
    public async Task UpdateUser_ReturnsOk_AsUser()
    {
        // Arrange
        var user = await AddNewUserAsync();
        var update = UserDataGenerator.CreateUser(user.Id);
        HttpContent content = JsonContent.Create(update);

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, user, _controllerPath, content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        await AssertUserPresenceAsync(update);
    }

    [Fact]
    public async Task UpdateUser_ReturnsOk_AsAdmin()
    {
        // Arrange
        var user = await AddNewUserAsync();
        var update = UserDataGenerator.CreateUser(user.Id);
        var admin = new User
        {
            Name = "admin"
        };
        HttpContent content = JsonContent.Create(update);

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, admin, _controllerPath, content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        await AssertUserPresenceAsync(update.Id);
    }

    [Fact]
    public async Task UpdateUser_ReturnsBadRequest()
    {
        // Arrange
        var user = UserDataGenerator.GetRandomUser();

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, user, _controllerPath, JsonContent.Create<UserPutDto>(null));

        // Assert
        AssertStatusCode(response, HttpStatusCode.BadRequest);
        AssertContentTypeProblemJson(response);
    }

    [Fact]
    public async Task DeleteUser_ReturnsForbidden()
    {
        // Arrange
        var executor = UserDataGenerator.GetRandomUser();
        var update = await AddNewUserAsync();
        update.Password = UserDataGenerator.GetRandomNumber(8);
        HttpContent content = JsonContent.Create(update);

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, executor, _controllerPath, content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.Forbidden);
        await AssertUserPresenceAsync(update, false);
    }

    [Fact]
    public async Task DeleteUser_ReturnsNotFound()
    {
        var update = UserDataGenerator.GetRandomUser();
        HttpContent content = JsonContent.Create(update);
        var executor = new User
        {
            Name = "admin"
        };

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, executor, _controllerPath, content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.NotFound);
        await AssertUserPresenceAsync(update, false);
        AssertContentTypeProblemJson(response);
    }

    [Fact]
    public async Task RegisterUser_ReturnsConflict()
    {
        // Arrange
        var duplicate =  await AddNewUserAsync();
        var toUpdate = await AddNewUserAsync();
        var update = new User
        {
            Email = duplicate.Email,
            Phone = duplicate.Phone,

            Id = toUpdate.Id,
            Name = toUpdate.Name,
            Password = toUpdate.Password
        };


        HttpContent content = JsonContent.Create(update);

        // Act
        HttpResponseMessage response = await SendWithJwtAuthAsync(
            HttpMethod.Put, toUpdate, _controllerPath, content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.Conflict);
        // 'application/json' content type is expected
        // because of using the ConflictObjectResult
        // which contains special custom error message
        AssertContentTypeJson(response);
        await AssertUserPresenceAsync(update, false);
    }
}