using CrazyPawn.Implementation;
namespace Implementation.Scripts.Pawns.Connections 
{
    public interface IConnectionPooler 
    {
        #region Methods

        public void ReturnToPool(Connection connection);

        #endregion
    }
}