using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Backend {
    public class Program {
        public static void Main (string[] args) {
            var host = CreateWebHostBuilder (args).Build ();

            // https://docs.microsoft.com/ru-ru/ef/core/miscellaneous/connection-resiliency
            using (var scope = host.Services.CreateScope ()) {
                var services = scope.ServiceProvider;

                // app must die if db can't migrate
                var context = services.GetRequiredService<UsersContext> ();
                var strategy = context.Database.CreateExecutionStrategy ();

                // example
                // strategy.Execute (() => {
                //     using (var transaction = context.Database.BeginTransaction ()) {
                //         // do work seed

                //         context.SaveChanges ();

                //         transaction.Commit ();
                //     }
                // });

                context.Database.Migrate ();

            }

            host.Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .ConfigureLogging ((hostingContext, logging) => {
                logging.ClearProviders ();

                logging.AddConfiguration (hostingContext.Configuration.GetSection ("Logging"));

                if (hostingContext.HostingEnvironment.IsDevelopment ()) {
                    logging.AddConsole ();
                    logging.AddDebug ();
                }
            })
            .UseStartup<Startup> ();
    }
}