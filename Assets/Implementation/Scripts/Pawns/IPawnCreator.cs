namespace CrazyPawn.Implementation 
{
    public interface IPawnCreator 
    {
        #region Methods

        public Pawn Create();
        
        public void CreatePawns();

        #endregion
    }
}