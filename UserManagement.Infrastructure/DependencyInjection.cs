using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserManagement.Domain;
using UserManagement.Infrastructure.DataAccess.MongoDb;
using UserManagement.Interfaces.Interfaces;

namespace UserManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMongo(
            this IServiceCollection services,
            ConfigurationManager configurationManager)
        {
            services.AddScoped<IUserRepository, MongoUserRepository>();

            MongoDbSettings settings = new ();
            
            IConfigurationSection section = configurationManager.GetSection(MongoDbSettings.Section);
            if (!section.Exists())
                throw new Exception($"{MongoDbSettings.Section} section not found in configuration sources.");

            section.Bind(settings);

            services.AddSingleton(Options.Create(settings));

            return services;
        }
    }
}
