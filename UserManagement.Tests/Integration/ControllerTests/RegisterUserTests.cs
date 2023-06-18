using System.Net.Http.Json;
using System.Net;
using UserManagement.Api.DataTransfer;
using UserManagement.Tests.Unit.ControllerTests;
using Xunit;
using UserManagement.Tests.Integration.ControllerTests.Helpers;

namespace UserManagement.Tests.Integration.ControllerTests;

public class RegisterUserTests : UsersControllerIntegrationTestBase
{
    [Fact]
    public async Task RegisterUser_ReturnsOkWithData()
    {
        // Arrange
        var user = UserDataGenerator.GetRandomUser();
        var data = new
        {
            id = user.Id.ToString(),
            name = user.Name,
            email = user.Email,
            password = user.Password,
            phone = user.Phone
        };
        HttpContent content = JsonContent.Create(data);

        // Act
        HttpResponseMessage response = await _client.PostAsync($"{_controllerPath}/register", content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        AssertContentTypeJson(response);
        var receivedUser = await response.Content.ReadFromJsonAsync<UserInfoDto>();
        Assert.NotNull(receivedUser);
        Assert.Equal(user.Id, receivedUser.Id);
        await AssertUserPresenceAsync(user.Id);
    }

    [Fact]
    public async Task RegisterUser_ReturnsBadRequest()
    {
        // Act
        HttpResponseMessage response = await _client.PostAsync(
            $"{_controllerPath}/register", JsonContent.Create<UserPutDto>(null));

        // Assert
        AssertStatusCode(response, HttpStatusCode.BadRequest);
        AssertContentTypeProblemJson(response);
    }

    [Fact]
    public async Task RegisterUser_ReturnsConflict()
    {
        // Arrange
        var user = await AddNewUserAsync();
        var duplicate = UserDataGenerator.CreateUser(user.Id);
        HttpContent content = JsonContent.Create(duplicate);

        // Act
        HttpResponseMessage response = await _client.PostAsync($"{_controllerPath}/register", content);

        // Assert
        AssertStatusCode(response, HttpStatusCode.Conflict);
        // 'application/json' content type is expected
        // because of using the ConflictObjectResult
        // which contains special custom error message
        AssertContentTypeJson(response);
        await AssertUserPresenceAsync(duplicate, false);
    }
}
