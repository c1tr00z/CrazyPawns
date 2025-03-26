using System.Collections.Generic;
using Implementation.Scripts.Pawns.Connections;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation.Connections 
{
    public class ConnectionsManager : IConnectionPooler
    {
        #region Private Fields

        private SignalBus SignalBus;

        private PawnConnector StartConnector;

        private List<Connection> ActiveConnections = new();

        private Queue<Connection> Cached = new();

        private Transform PoolParent;

        #endregion

        #region Injected Fields

        [Inject] private ConnectionFactory ConnectionFactory;

        #endregion

        #region Accessors

        private Transform Pool => CommonUtils.GetCached(ref PoolParent, () => {
            var newPool = new GameObject("ConnectionsPool").transform;
            newPool.gameObject.SetActive(false);
            return newPool;
        });

        #endregion
        
        #region Constructors

        [Inject]
        public ConnectionsManager(SignalBus signalBus) 
        {
            SignalBus = signalBus;
            SignalBus.Subscribe<PawnConnectorActivate>(OnConnectorActivate);
            SignalBus.Subscribe<PawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        #endregion

        #region Destructor

        ~ConnectionsManager() 
        {
            SignalBus.Unsubscribe<PawnConnectorActivate>(OnConnectorActivate);
            SignalBus.Unsubscribe<PawnConnectorDeactivate>(OnConnectorDeactivate);
        }

        #endregion

        #region IConnectionPooler Implementation

        public void ReturnToPool(Connection connection) 
        {
            if (Cached.Contains(connection)) 
            {
                return;
            }
            Cached.Enqueue(connection);
            connection.transform.parent = Pool;
        }

        #endregion
        
        #region Class Implementation

        private void OnConnectorActivate(PawnConnectorActivate signal) 
        {
            StartConnector = signal.Connector;
        }
        
        private void OnConnectorDeactivate(PawnConnectorDeactivate signal) 
        {
            var endConnector = signal.Connector;
            if (StartConnector is not null && endConnector is not null) 
            {
                Create(StartConnector, endConnector);
            }
            
            StartConnector = null;
        }

        private void Create(PawnConnector start, PawnConnector end) 
        {
            Connection connection;
            if (Cached.Count > 0) 
            {
                connection = Cached.Dequeue();
                connection.transform.parent = null;
            } 
            else 
            {
                connection = ConnectionFactory.Create();
            }
            connection.SetPoints(new List<Transform> { start.transform, end.transform });
        }

        #endregion
    }
}