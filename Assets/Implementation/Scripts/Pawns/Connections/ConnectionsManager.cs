using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation  
{
    public class ConnectionsManager : IConnectionPooler
    {
        #region Private Fields

        private SignalBus _signalBus;

        private PawnConnector _startConnector;

        private List<Connection> _activeConnections = new();

        private Queue<Connection> _cached = new();

        private Transform _poolParent;

        #endregion

        #region Injected Fields

        [Inject] private ConnectionFactory ConnectionFactory;

        #endregion

        #region Accessors

        private Transform Pool => CommonUtils.GetCached(ref _poolParent, () => {
            var newPool = new GameObject("ConnectionsPool").transform;
            newPool.gameObject.SetActive(false);
            return newPool;
        });

        #endregion
        
        #region Constructors

        [Inject]
        public ConnectionsManager(SignalBus signalBus) 
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Subscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
            _signalBus.Subscribe<IPawnDraggedSignal>(OnPawnDragged);
            _signalBus.Subscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region Destructor

        ~ConnectionsManager() 
        {
            _signalBus.Unsubscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Unsubscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
            _signalBus.Unsubscribe<IPawnDraggedSignal>(OnPawnDragged);
            _signalBus.Unsubscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region IConnectionPooler Implementation

        public void ReturnToPool(Connection connection) 
        {
            if (connection is null) 
            {
                return;
            }
            if (_activeConnections.Contains(connection)) 
            {
                _activeConnections.Remove(connection);
            }
            if (_cached.Contains(connection)) 
            {
                return;
            }
            _cached.Enqueue(connection);
            connection.transform.parent = Pool;
        }

        #endregion
        
        #region Class Implementation

        private void OnConnectorActivate(IPawnConnectorActivate signal) 
        {
            if (_startConnector is null) 
            {
                _startConnector = signal.Connector;
            } 
            else 
            {
                var endConnector = signal.Connector;
                Create(_startConnector, endConnector);
                _startConnector = null;
            }
        }
        
        private void OnConnectorDeactivate(IPawnConnectorDeactivate signal) 
        {
            var endConnector = signal.Connector;
            
            Create(_startConnector, endConnector);
            
            _startConnector = null;
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
            if (_cached.Count > 0) 
            {
                connection = _cached.Dequeue();
                connection.transform.parent = null;
            } 
            else 
            {
                connection = ConnectionFactory.Create();
            }
            if (!_activeConnections.Contains(connection)) 
            {
                _activeConnections.Add(connection);
            }
            connection.SetPoints(new List<PawnConnector> { start, end });
        }

        private void OnPawnDragged(IPawnDraggedSignal signal) 
        {
            var toChanged = _activeConnections.Where(c => c.Points.Any(p => p.Parent == signal.Pawn)).ToList();
            toChanged.ForEach(c => c.GenerateLineMesh());
        }

        private void OnPawnRemoved(IPawnRemovedSignal signal) 
        {
            var toPool = _activeConnections.Where(c => c.Points.Any(p => p.Parent == signal.Pawn)).ToList();
            toPool.ForEach(ReturnToPool);
        }

        #endregion
    }
}