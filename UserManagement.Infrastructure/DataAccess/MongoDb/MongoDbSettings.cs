namespace UserManagement.Infrastructure.DataAccess.MongoDb;

public class MongoDbSettings
{
    public const string Section = "MongoDB";

    public string Connection { get; init; } = null!;

    public string Database { get; init; } = null!;

    public string UserCollection { get; init; } = null!;
}

