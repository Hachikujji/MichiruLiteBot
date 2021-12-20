using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MichiruLite
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                new Michiru().StartAsync().GetAwaiter().GetResult();
            }
            catch (FileNotFoundException e)
            {
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}