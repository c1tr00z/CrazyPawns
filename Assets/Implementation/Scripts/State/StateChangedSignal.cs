namespace CrazyPawn.Implementation
{
    public class StateChangedSignal 
    {

        #region Accessors

        public State State { get; }

        #endregion
        
        #region Constructors

        public StateChangedSignal(State newState) 
        {
            State = newState;
        }

        #endregion
    }
}