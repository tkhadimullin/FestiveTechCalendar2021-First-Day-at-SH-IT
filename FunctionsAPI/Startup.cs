
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(test_santa_api.Startup))]
namespace test_santa_api {
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<Child>(new Child {
                Id=1,
                City="Moscow",
                FirstName="Ivan",
                LastName="Ivanov",
                IsNaughty=false
            });
            builder.Services.AddScoped<SHDbContext>();
        }
    }
}