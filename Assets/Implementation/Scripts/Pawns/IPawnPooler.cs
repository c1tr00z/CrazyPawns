namespace CrazyPawn.Implementation 
{
    public interface IPawnPooler 
    {
        #region Methods

        public void ReturnToPool(Pawn pawn);

        #endregion
    }
}