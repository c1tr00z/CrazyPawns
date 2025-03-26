namespace CrazyPawn.Implementation 
{
    public interface IConnectionPooler 
    {
        #region Methods

        public void ReturnToPool(Connection connection);

        #endregion
    }
}