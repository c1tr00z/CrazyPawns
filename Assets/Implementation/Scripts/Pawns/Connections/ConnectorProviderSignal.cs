namespace CrazyPawn.Implementation 
{
    public class ConnectorProviderSignal : IPawnConnectorActivate, IPawnConnectorDeactivate
    {
        #region Accessors

        public PawnConnector Connector { get; private set; }

        #endregion

        #region Class Implementation

        public void UpdateConnector(PawnConnector connector) 
        {
            Connector = connector;
        }

        #endregion
    }
}