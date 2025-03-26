namespace CrazyPawn.Implementation 
{
    public struct PawnConnectorDeactivate 
    {
        #region Public Fields

        public PawnConnector Connector;

        #endregion

        public static PawnConnectorDeactivate MakeNew(PawnConnector connector) 
        {
            return new PawnConnectorDeactivate 
            {
                Connector = connector
            };
        }
    }
}