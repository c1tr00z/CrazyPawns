using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public struct PawnConnectorActivate 
    {
        #region Public Fields

        public PawnConnector Connector;

        #endregion

        public static PawnConnectorActivate MakeNew(PawnConnector connector) 
        {
            return new PawnConnectorActivate 
            {
                Connector = connector
            };
        }
    }
    
}