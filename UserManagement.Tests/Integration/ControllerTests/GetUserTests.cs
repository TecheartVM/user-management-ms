using System.Net.Http.Json;
using System.Net;
using UserManagement.Api.DataTransfer;
using UserManagement.Tests.Unit.ControllerTests;
using Xunit;

namespace UserManagement.Tests.Integration.ControllerTests;

public class GetUserTests : UsersControllerIntegrationTestBase
{
    [Fact]
    public async Task GetUser_ReturnsOkWithData()
    {
        // Arrange
        var user = await AddNewUserAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{_controllerPath}/{user.Id}");

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        AssertContentTypeJson(response);
        var receivedUser = await response.Content.ReadFromJsonAsync<UserInfoDto>();
        Assert.NotNull(receivedUser);
        Assert.Equal(user.Id, receivedUser.Id);
    }

    [Fact]
    public async Task GetUser_ReturnsBadRequest()
    {
        // Arrange
        await AddNewUserAsync();
        var invalidGuid = Guid.Empty;

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{_controllerPath}/{invalidGuid}");

        // Assert
        AssertStatusCode(response, HttpStatusCode.BadRequest);
        AssertContentTypeProblemJson(response);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound()
    {
        // Arrange
        var nonExistentGuid = Guid.NewGuid();

        // Act
        HttpResponseMessage response = await _client.GetAsync($"{_controllerPath}/{nonExistentGuid}");

        // Assert
        AssertStatusCode(response, HttpStatusCode.NotFound);
        AssertContentTypeProblemJson(response);
    }
}
