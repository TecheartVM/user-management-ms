using MongoDB.Driver;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.DataAccess.MongoDb.Indexes;

internal static class UserIndexesExtension
{
    public static void AddUserIndexes(this IMongoCollection<User> collection)
    {
        var options = new CreateIndexOptions<User>() { Unique = true };

        collection.Indexes.CreateMany(new List<CreateIndexModel<User>>
        {
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                options),
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Phone),
                options)
        });
    }
}

