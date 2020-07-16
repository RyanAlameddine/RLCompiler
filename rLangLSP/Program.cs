﻿using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using System.Threading.Tasks;

namespace rLangLSP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithLoggerFactory(new LoggerFactory())
                    .AddDefaultLoggingProvider()
                    //.WithServices(ConfigureServices)
                    .WithHandler<TextDocumentSyncHandler>()
                 );

            await server.WaitForExit;
        }

        //static void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddSingleton<BufferManager>();
        //}
    }
}
