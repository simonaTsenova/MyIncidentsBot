using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSTeamsBot.Common.Settings;
using MSTeamsBot.Dialogs;
using MSTeamsBot.Helpers;
using MSTeamsBot.Helpers.Contracts;
using MSTeamsBot.Services;
using MSTeamsBot.Services.Contracts;
using System;

namespace MSTeamsBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);

            Conversation.UpdateContainer(b =>
            {
                b.Register(c => new MicrosoftAppCredentials(Configuration["MicrosoftAppId"], Configuration["MicrosoftAppPassword"])).SingleInstance();
            });

            services.AddMvc();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<IncidentsService>()
                .Keyed<IIncidentsService>(FiberModule.Key_DoNotSerialize)
                .As<IIncidentsService>();
            //.SingleInstance();
            //.InstancePerRequest();

            builder.RegisterType<MetaMessagingService>()
               .Keyed<IMetaMessagingService>(FiberModule.Key_DoNotSerialize)
               .As<IMetaMessagingService>()
               .SingleInstance();
            //.InstancePerRequest();

            builder.RegisterType<ServiceNowClient>()
                .Keyed<IRestClient>(FiberModule.Key_DoNotSerialize)
                .As<IRestClient>();
                //.InstancePerRequest();

            builder.RegisterType<LUISDialog>()
                .InstancePerDependency();

            builder.RegisterType<CommonResponsesDialog>()
                .InstancePerDependency();

            var container = builder.Build();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
