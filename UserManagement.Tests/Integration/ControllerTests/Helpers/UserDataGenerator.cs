using System.Text;
using UserManagement.Domain.Entities;

namespace UserManagement.Tests.Integration.ControllerTests.Helpers;

internal static class UserDataGenerator
{
    public static User CreateUser(Guid id)
    {
        string name = GetRandomName();

        return new User
        {
            Id = id,
            Name = name,
            Email = $"{name}@test.com",
            Password = GetRandomNumber(8),
            Phone = GetRandomPhoneNumber()
        };
    }

    public static User GetRandomUser() =>
        CreateUser(Guid.NewGuid());

    public static string GetRandomName() =>
        $"user{GetRandomNumber(5)}";

    public static string GetRandomEmail() =>
        $"{GetRandomName()}@test.com";

    public static string GetRandomPhoneNumber() =>
        $"380{GetRandomNumber(9)}";

    public static string GetRandomNumber(int length)
    {
        if (length <= 0)
            return string.Empty;

        StringBuilder builder = new StringBuilder();

        Random random = SeedRandom();

        for (int i = 0; i < length; i++)
            builder.Append(random.Next(0, 9).ToString());

        return builder.ToString();
    }

    private static Random SeedRandom()
    {
        return new Random(Guid.NewGuid().GetHashCode());
    }
}
