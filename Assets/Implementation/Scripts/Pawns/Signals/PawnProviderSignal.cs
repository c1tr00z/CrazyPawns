namespace CrazyPawn.Implementation 
{
    public class PawnProviderSignal : IPawnDragStartedSignal, IPawnDraggedSignal, IPawnDragFinishedSignal, IPawnRemovedSignal
    {
        
        #region IPawnDraggedSignal Implementation

        public Pawn Pawn { get; private set; }

        #endregion

        #region Class Implementation

        public void UpdatePawn(Pawn pawn) 
        {
            Pawn = pawn;
        }

        #endregion
    }
}