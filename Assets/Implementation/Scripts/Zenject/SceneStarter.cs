using Zenject;

namespace CrazyPawn.Implementation
{
    public class SceneStarter : IInitializable
    {
        #region Injected Fields

        [Inject] private IStateCompleter _stateCompleter;

        #endregion
        
        #region Events
        
        public void Initialize()
        {
            _stateCompleter.CompleteState(State.Init);
        }
        
        #endregion
    }
}