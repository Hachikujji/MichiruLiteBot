using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MichiruLite
{
    class Program
    {
        static void Main(string[] args)
            => new Michiru().StartAsync().GetAwaiter().GetResult();
    }
}
