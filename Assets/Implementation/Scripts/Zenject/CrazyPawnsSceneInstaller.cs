using Zenject;
namespace CrazyPawn.Implementation 
{
    public class CrazyPawnsSceneInstaller : MonoInstaller 
    {
        #region MonoInstaller Implementation

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<StateChangedSignal>().OptionalSubscriber();
            Container.DeclareSignal<InputSignal>().OptionalSubscriber();
            Container.Bind<IInitializable>().To<SceneStarter>().AsSingle();
            Container.Bind<IAssetProvider>().To<AssetManager>().AsSingle();
            Container.Bind(typeof(IPawnCreator), typeof(IPawnPooler)).To<PawnsManager>().AsSingle().NonLazy();
            Container.BindFactory<Pawn, PawnFactory>().FromFactory<RealPawnsFactory>();
            Container.Bind(typeof(IStateCompleter), typeof(IStateProvider)).To<StateManager>().AsSingle();
        }

        #endregion
    }
}