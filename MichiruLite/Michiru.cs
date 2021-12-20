using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using MichiruLite.Modules.Audio;
using MichiruLite.Modules.Audio.Services;
using MichiruLite.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace MichiruLite
{
    internal class Michiru
    {
        private static IServiceProvider _serviceProvider;

        private static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            // Subscribe Services
            serviceCollection.AddSingleton<DiscordSocketClient>();
            serviceCollection.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                ThrowOnError = false
            }));
            serviceCollection.AddSingleton<CommandHandler>();
            serviceCollection.AddSingleton<AudioService>();
            serviceCollection.AddSingleton<LogService>();
            serviceCollection.AddSingleton<AuthorizationService>();
            serviceCollection.AddSingleton<AudioModule>();
            serviceCollection.AddSingleton<InteractiveService>();
            serviceCollection.AddSingleton<YoutubeService>();
            //serviceCollection.AddScoped<QueueService>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ConfigureServices();

            _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _serviceProvider.GetRequiredService<LogService>();
            _serviceProvider.GetRequiredService<AudioModule>();
            _serviceProvider.GetRequiredService<CommandHandler>();
            _serviceProvider.GetRequiredService<AuthorizationService>();

            await Task.Delay(-1);
        }
    }
}