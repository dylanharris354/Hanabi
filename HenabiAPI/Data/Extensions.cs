using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HenabiAPI.Data
{
    static class Extensions
    {
        public static void CreateDbIfNotExists(this IApplicationBuilder host)
        {
            {
                using (var scope = host.ApplicationServices.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<HenabiDBContext>();
                    context.Database.EnsureCreated();
                    DBInitiallizer.Initialize(context);
                }
            }
        }
    }
    }
