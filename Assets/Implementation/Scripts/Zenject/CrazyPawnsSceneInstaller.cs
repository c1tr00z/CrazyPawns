using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsSceneInstaller : MonoInstaller {
        #region MonoInstaller Implementation

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<StateChangedSignal>().OptionalSubscriber();
            Container.Bind<IInitializable>().To<SceneStarter>().AsSingle();
            Container.Bind<PawnsManager>().AsSingle().NonLazy();
            Container.Bind(typeof(IStateCompleter), typeof(IStateProvider)).To<StateManager>().AsSingle();
        }

        #endregion
    }
}