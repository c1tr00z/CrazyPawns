namespace CrazyPawn.Implementation
{
    public class StateChangedSignal : IStateChangedSignal
    {

        #region Accessors

        public State State { get; private set; }

        #endregion
        
        #region Constructors

        public StateChangedSignal(State newState) 
        {
            State = newState;
        }

        #endregion

        #region Class Implementation

        public void UpdateState(State newState) 
        {
            State = newState;
        }

        #endregion
    }
}