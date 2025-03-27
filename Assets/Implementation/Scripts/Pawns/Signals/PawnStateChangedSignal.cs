namespace CrazyPawn.Implementation 
{
    public class PawnStateChangedSignal 
    {
        #region Accessors

        public IPawnStateProvider StateProvider { get; }

        #endregion

        #region Constructors

        public PawnStateChangedSignal(IPawnStateProvider stateProvider) 
        {
            StateProvider = stateProvider;
        }

        #endregion
    }
}