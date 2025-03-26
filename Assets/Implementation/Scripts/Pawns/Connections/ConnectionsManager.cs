using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation  
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
            SignalBus.Subscribe<IPawnDraggedSignal>(OnPawnDragged);
            SignalBus.Subscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region Destructor

        ~ConnectionsManager() 
        {
            SignalBus.Unsubscribe<PawnConnectorActivate>(OnConnectorActivate);
            SignalBus.Unsubscribe<PawnConnectorDeactivate>(OnConnectorDeactivate);
            SignalBus.Unsubscribe<IPawnDraggedSignal>(OnPawnDragged);
            SignalBus.Unsubscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region IConnectionPooler Implementation

        public void ReturnToPool(Connection connection) 
        {
            if (connection is null) 
            {
                return;
            }
            if (ActiveConnections.Contains(connection)) 
            {
                ActiveConnections.Remove(connection);
            }
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
            if (StartConnector is null) 
            {
                StartConnector = signal.Connector;
            } 
            else 
            {
                var endConnector = signal.Connector;
                Create(StartConnector, endConnector);
                StartConnector = null;
            }
        }
        
        private void OnConnectorDeactivate(PawnConnectorDeactivate signal) 
        {
            var endConnector = signal.Connector;
            
            Create(StartConnector, endConnector);
            
            StartConnector = null;
        }

        private void Create(PawnConnector start, PawnConnector end) 
        {
            if (start is null || end is null) 
            {
                return;
            }
            if (start.Parent == end.Parent) 
            {
                return;
            }
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
            if (!ActiveConnections.Contains(connection)) 
            {
                ActiveConnections.Add(connection);
            }
            connection.SetPoints(new List<PawnConnector> { start, end });
        }

        private void OnPawnDragged(IPawnDraggedSignal signal) 
        {
            var toChanged = ActiveConnections.Where(c => c.Points.Any(p => p.Parent == signal.Pawn)).ToList();
            toChanged.ForEach(c => c.GenerateLineMesh());
        }

        private void OnPawnRemoved(IPawnRemovedSignal signal) 
        {
            var toPool = ActiveConnections.Where(c => c.Points.Any(p => p.Parent == signal.Pawn)).ToList();
            toPool.ForEach(ReturnToPool);
        }

        #endregion
    }
}