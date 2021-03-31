using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace MichiruLite.Services
{
    public class test
    {
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private IServiceProvider _services;
		private ILogger<test> _logger;

		public test(ILogger<test> logger,CommandService command,DiscordSocketClient client, IServiceProvider services)
		{
			_services = services;
			_commands = command;
			_client = client;
			_client.MessageReceived += HandleCommandAsync;
			_client.MessageDeleted += send;
			_logger = logger;
		}
		public async Task send(Cacheable<IMessage, ulong> a,ISocketMessageChannel b)
        {
            Console.WriteLine("asdasdasd");
        }
		public async Task InitializeAsync()
		{
			// Pass the service provider to the second parameter of
			// AddModulesAsync to inject dependencies to all modules 
			// that may require them.
			await _commands.AddModulesAsync(
				assembly: Assembly.GetEntryAssembly(),
				services: _services);
		}

		public async Task HandleCommandAsync(SocketMessage msg)
		{
			_logger.LogInformation(msg.Content);
			// Don't process the command if it was a system message
			var message = msg as SocketUserMessage;
			if (message == null) return;

			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;

			// Determine if the message is a command based on the prefix and make sure no bots trigger commands
			if (!(message.HasCharPrefix('!', ref argPos) ||
				message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
				message.Author.IsBot)
				return;

			// Create a WebSocket-based command context based on the message
			var context = new SocketCommandContext(_client, message);

			// Execute the command with the command context we just
			// created, along with the service provider for precondition checks.
			await _commands.ExecuteAsync(
				context: context,
				argPos: argPos,
				services: null);
		}
	}
}
