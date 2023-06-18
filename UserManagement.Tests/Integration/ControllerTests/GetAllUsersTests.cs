using System.Net;
using System.Net.Http.Json;
using UserManagement.Api.DataTransfer;
using UserManagement.Tests.Unit.ControllerTests;
using Xunit;

namespace UserManagement.Tests.Integration.ControllerTests;

public class GetAllUsersTests : UsersControllerIntegrationTestBase
{
    [Fact]
    public async Task GetAllUsers_ReturnsOkWithData()
    {
        int userCount = 2;

        // Arrange
        var users = await AddUsersAsync(userCount);

        // Act
        HttpResponseMessage response = await _client.GetAsync(_controllerPath);

        // Assert
        AssertStatusCode(response, HttpStatusCode.OK);
        AssertContentTypeJson(response);

        var receivedList = await response.Content.ReadFromJsonAsync<UserListDto>();
        Assert.NotNull(receivedList);
        Assert.Equal(userCount, receivedList.Count);

        int actualCount = receivedList.Users.Count();
        Assert.Equal(userCount, actualCount);
    }
}
