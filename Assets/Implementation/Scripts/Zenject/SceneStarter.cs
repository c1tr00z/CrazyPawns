using Zenject;

namespace CrazyPawn.Implementation
{
    public class SceneStarter : IInitializable
    {
        #region Injected Fields

        [Inject] private IStateCompleter StateCompleter;

        #endregion
        
        #region Events
        
        public void Initialize()
        {
            StateCompleter.CompleteState(State.Init);
        }
        
        #endregion
    }
}