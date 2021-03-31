using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MichiruLite.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MichiruLite
{
    class Michiru
    {
        private static IServiceProvider _serviceProvider;
        private static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            // Subscribe Services
            serviceCollection.AddSingleton<CommandHandler>();
            serviceCollection.AddSingleton<AudioService>();
            serviceCollection.AddSingleton<MessageService>();
            serviceCollection.AddSingleton<DiscordSocketClient>();
            serviceCollection.AddSingleton<AuthorizationService>();
            serviceCollection.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                ThrowOnError = false
            }));
            serviceCollection.AddLogging(configure => configure.AddConsole());

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ConfigureServices();

            _serviceProvider.GetRequiredService<AuthorizationService>();
            _serviceProvider.GetRequiredService<CommandHandler>();


            await Task.Delay(-1);

        }

    }
}
