using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.ObjectMapping;
using Volo.Abp.AutoMapper;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace App;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpCachingModule))]
[DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
[DependsOn(typeof(AbpObjectMappingModule))]
[DependsOn(typeof(AbpAutoMapperModule))]
public class AppModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<AppModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        logger.LogInformation($"MySettingName => {configuration["MySettingName"]}");

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "162.HNSC.VOC.II";
        });

        Configure<RedisCacheOptions>(options =>
        {
            // options.Configuration = "192.168.1.165:30379";
            var redisConfig = ConfigurationOptions.Parse("192.168.1.165:30379");
            redisConfig.SocketManager = SocketManager.ThreadPool;
            redisConfig.SyncTimeout = 50000;
            options.ConfigurationOptions = redisConfig;
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            //Add all mappings defined in the assembly of the MyModule class
            options.AddMaps<AppModule>();
        });

        base.ConfigureServices(context);
    }
}
