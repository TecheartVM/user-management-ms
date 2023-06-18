using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.DataAccess.MongoDb.Indexes;
using UserManagement.Infrastructure.DataAccess.MongoDb.Mappers;
using UserManagement.Interfaces.Interfaces;

namespace UserManagement.Infrastructure.DataAccess.MongoDb;

public class MongoUserRepository : IUserRepository
{
    static MongoUserRepository()
    {
        // simply registering serializer does not handle querry filters
        // so we can't correctly search/edit/delete db entries by their GUID _id
        // without using the deprecated properties below
#pragma warning disable CS0618
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618

        BsonSerializer.RegisterSerializer<Guid>(new GuidSerializer(GuidRepresentation.Standard));
        new UserBsonMapper().RegisterMap();
    }

    private readonly IMongoCollection<User> _users;

    public MongoUserRepository(IOptions<MongoDbSettings> dbSettings)
    {
        MongoDbSettings settingsValues = dbSettings.Value;
        IMongoClient client = new MongoClient(settingsValues.Connection);
        IMongoDatabase db = client.GetDatabase(settingsValues.Database);
        _users = db.GetCollection<User>(settingsValues.UserCollection);

        _users.AddUserIndexes();
    }

    public async Task<User?> Add(User user)
    {
        if (user == null) return null;
        
        try
        {
            await _users.InsertOneAsync(user);
            return user;
        }
        catch(MongoWriteException e)
        {
            throw e.WrapException();
        }
    }

    public async Task<IEnumerable<User>> Get()
    {
        var all = await _users.FindAsync(u => true);
        var result = await all.ToListAsync();
        return result;
    }

    public async Task<User?> Get(Guid id)
    {
        var all = await _users.FindAsync(u => u.Id == id);
        var list = await all.ToListAsync();
        return list.FirstOrDefault();
    }

    public async Task<bool> Update(User user)
    {
        var updateDefinition = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Password, user.Password)
            .Set(u => u.Phone, user.Phone);

        try
        {
            var result = await _users.UpdateOneAsync(u => u.Id == user.Id, updateDefinition);
            return result.IsAcknowledged
                && result.IsModifiedCountAvailable
                && result.ModifiedCount > 0;
        }
        catch (MongoWriteException e)
        {
            throw e.WrapException();
        }
    }

    public async Task<bool> Delete(Guid id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.IsAcknowledged
            && result.DeletedCount > 0;
    }
}

