using MessagePipe;
using SaintsField.Playa;
using VContainer;
using VContainer.Unity;

namespace SoraTehk.Samples {
    public partial class CoreScope : LifetimeScope {
        [LayoutStart("Scene", ELayout.FoldoutBox)]
        public CoreEntryPoint CoreEntryPoint = null!;

        protected override void Configure(IContainerBuilder builder) {
            // MessagePipe
            MessagePipeOptions msgPipeCfg = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            builder.RegisterMessageBroker<int>(msgPipeCfg);

            // Entry point
            builder.RegisterComponent(CoreEntryPoint).AsImplementedInterfaces();
        }
    }
}