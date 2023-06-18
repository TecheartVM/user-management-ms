using MongoDB.Bson.Serialization;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.DataAccess.MongoDb.Mappers;

internal class UserBsonMapper
{
    public void RegisterMap()
    {
        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.MapIdProperty(u => u.Id);
            cm.MapProperty(u => u.Name).SetElementName("name");
            cm.MapProperty(u => u.Email).SetElementName("email");
            cm.MapProperty(u => u.Password).SetElementName("password");
            cm.MapProperty(u => u.Phone).SetElementName("phone");
            cm.MapProperty(u => u.Created).SetElementName("registration_date");

            cm.MapCreator(u => new User
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Phone = u.Phone,
                Created = u.Created
            });
        });
    }
}

