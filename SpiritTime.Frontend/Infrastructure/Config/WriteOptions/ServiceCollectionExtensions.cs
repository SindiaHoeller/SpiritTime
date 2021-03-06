using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SpiritTime.Frontend.Infrastructure.Config.WriteOptions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureWritable<T>(
            this IServiceCollection services,
            IConfigurationSection   section,
            string                  file) where T : class, new()
        {
            Console.WriteLine("ConfigureWritable file: " + file);
            services.Configure<T>(section);
            services.AddTransient<IWritableOptions<T>>(provider =>
            {
                var configuration = (IConfigurationRoot)provider.GetService<IConfiguration>();
                var environment   = provider.GetService<IWebHostEnvironment>();
                var options       = provider.GetService<IOptionsMonitor<T>>();
                return new WritableOptions<T>(environment, options, configuration, section.Key, file);
            });
        }
    }
}