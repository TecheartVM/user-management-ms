using Xunit;
using Mongo2Go;
using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.DataAccess.MongoDb;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using UserManagement.Tests.Integration.ControllerTests.Helpers;
using UserManagement.Api.Authentication.Jwt;
using System.Net.Http.Headers;

namespace UserManagement.Tests.Unit.ControllerTests;

public class UsersControllerIntegrationTestBase : IDisposable
{
    protected const string _controllerPath = "api/v1/users";

    protected readonly string _dbName = "test_db";
    protected readonly string _userCollectionName = "test_users";

    protected readonly MongoDbRunner _dbRunner;
    protected readonly IMongoCollection<User> _usersCollection;
    protected readonly HttpClient _client;

    protected const string hostUrl = "http://localhost:5048";
    protected const string apiKey = "1FX3cJimjosUAE470Exu0I3yK4UtUAeBqcVchVzC";
    internal JwtAuthenticationManager _authManager = new JwtAuthenticationManager(
        new JwtSettings
        {
            TokenValidityTime = 15,
            Issuer = hostUrl,
            Audience = hostUrl,
            Key = apiKey
        });

    public UsersControllerIntegrationTestBase()
    {
        _dbRunner = MongoDbRunner.Start();

        MongoDbSettings dbSettings = new MongoDbSettings
        {
            Connection = _dbRunner.ConnectionString,
            Database = _dbName,
            UserCollection = _userCollectionName
        };

        // internal class 'Program' is available here
        // because of the 'InternalsVisibleTo' tag in the Api assembly
        var appFactory = new WebApplicationFactory<Program>();

        // needed to register serializers and mappers
        new MongoUserRepository(Options.Create(dbSettings));

        appFactory = appFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(Options.Create(dbSettings));

                //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //    .AddJwtBearer(options =>
                //    {
                //        options.TokenValidationParameters = new TokenValidationParameters
                //        {
                //            ValidateIssuer = false,
                //            ValidateAudience = false,
                //            ValidateLifetime = false,
                //            ValidateIssuerSigningKey = true,
                //            ValidIssuer = hostUrl,
                //            ValidAudience = hostUrl,
                //            IssuerSigningKey = new SymmetricSecurityKey(
                //                Encoding.UTF8.GetBytes(apiKey)),
                //            NameClaimType = "name"
                //        };
                //    });
            });
        });

        _client = appFactory.CreateClient();

        _usersCollection = new MongoClient(_dbRunner.ConnectionString)
            .GetDatabase(_dbName).GetCollection<User>(_userCollectionName);
    }

    // =========== Database filling methods ===========

    protected async Task<User> AddNewUserAsync() =>
        await AddUserAsync(UserDataGenerator.GetRandomUser());

    protected async Task<User> AddNewUserAsync(Guid id) =>
        await AddUserAsync(UserDataGenerator.CreateUser(id));

    protected async Task<User> AddUserAsync(User user)
    {
        await _usersCollection.InsertOneAsync(user);
        return user;
    }

    protected async Task<ICollection<User>> AddUsersAsync(int count)
    {
        var result = new List<User>(count);

        for (int i = 0; i < result.Capacity; i++)
            result.Add(UserDataGenerator.GetRandomUser());

        await _usersCollection.InsertManyAsync(result);

        return result;
    }

    // ================================================

    // ================ Assert methods ================

    protected void AssertStatusCode(HttpResponseMessage message, HttpStatusCode code) =>
        Assert.Equal(code, message.StatusCode);

    protected async Task AssertUserPresenceAsync(Guid userId, bool expectation = true)
    {
        var search = (await _usersCollection.FindAsync(u => u.Id == userId)).ToList();

        User? user = search.Any() ? search.First() : null;
        bool found = user != null;
        string userMessage =
            $"Test database {(expectation ? "does not contain" : "already contains")} user with this id";
        Assert.True(found == expectation, userMessage);
    }

    protected async Task AssertUserPresenceAsync(User user, bool expectation = true)
    {
        var search = (await _usersCollection.FindAsync(u => u.Id == user.Id)).ToList();

        User? found = search.Any() ? search.First() : null;
        bool flag = found != null;

        if(flag)
        {
            flag = string.Equals(user.Name, found!.Name)
                && string.Equals(user.Email, found!.Email)
                && string.Equals(user.Password, found!.Password)
                && string.Equals(user.Phone, found!.Phone);
        }

        string userMessage =
            $"Test database {(expectation ? "does not contain" : "contains")} such user, which is unexpected";
        Assert.True(flag == expectation, userMessage);
    }

    protected void AssertContentType(HttpResponseMessage message, string contentType)
    {
        Assert.Equal(
            contentType,
            message.Content?.Headers?.ContentType?.ToString());
    }

    protected void AssertContentTypeJson(HttpResponseMessage message)
    {
        AssertContentType(message, "application/json; charset=utf-8");
    }

    protected void AssertContentTypeProblemJson(HttpResponseMessage message)
    {
        AssertContentType(message, "application/problem+json; charset=utf-8");
    }

    // ================================================

    // =============== Request methods ================

    protected async Task<HttpResponseMessage> SendWithAuthAsync(
        HttpMethod method, AuthenticationHeaderValue authHeader,
        string path = _controllerPath, HttpContent? content = null)
    {
        HttpResponseMessage response;

        using (var requestMessage = new HttpRequestMessage(method, path))
        {
            requestMessage.Headers.Authorization = authHeader;
            if(content != null) requestMessage.Content = content;
            response = await _client.SendAsync(requestMessage);
        }

        return response;
    }

    protected async Task<HttpResponseMessage> SendWithJwtAuthAsync(
        HttpMethod method, User userToAuthenticate,
        string path = _controllerPath, HttpContent? content = null)
    {
        string token = _authManager.GenerateApiKey(userToAuthenticate)!;

        return await SendWithAuthAsync(
            method,
            new AuthenticationHeaderValue("Bearer", token),
            path,
            content);
    }

    // ================================================

    public void Dispose()
    {
        _dbRunner?.Dispose();
        _client?.Dispose();
    }
}
