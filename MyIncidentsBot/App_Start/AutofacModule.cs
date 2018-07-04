using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using MyIncidentsBot.Dialogs;
using MyIncidentsBot.Helpers;
using MyIncidentsBot.Helpers.Contracts;
using MyIncidentsBot.Services;
using MyIncidentsBot.Services.Contracts;

namespace MyIncidentsBot.App_Start
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Bindings
            builder.RegisterType<IncidentsService>()
                .Keyed<IIncidentsService>(FiberModule.Key_DoNotSerialize)
                .As<IIncidentsService>()
                .InstancePerRequest();

            builder.RegisterType<ServiceNowClient>()
                .Keyed<IRestClient>(FiberModule.Key_DoNotSerialize)
                .As<IRestClient>()
                .InstancePerRequest();

            builder.RegisterType<LUISDialog>()
                .InstancePerDependency();

            builder.RegisterType<CommonResponsesDialog>()
                .InstancePerDependency();
        }
    }
}