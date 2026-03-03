using SaintsField.Playa;
using VContainer;
using VContainer.Unity;

namespace SoraTehk.Samples {
    public partial class MainScope : LifetimeScope {
        [LayoutStart("Scene", ELayout.FoldoutBox)]
        public MainEntryPoint MainEntryPoint = null!;

        protected override void Configure(IContainerBuilder builder) {
            // Entry point
            builder.RegisterComponent(MainEntryPoint).AsImplementedInterfaces();
        }
    }
}