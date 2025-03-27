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

        private Connection _staticConnections;
        
        private Connection _dynamicConnections;

        private Queue<Connection> _cached = new();

        private Transform _poolParent;

        private List<PawnConnector[]> _connectionsData = new();
        
        private List<PawnConnector[]> _staticData = new();
        
        private List<PawnConnector[]> _dynamicData = new();

        private Connection _connectionPrefab;
        
        #endregion

        #region Injected Fields

        [Inject] private IAssetProvider _assetProvider;

        [Inject] private CrazyPawnsImplSettings _implementationSettings;

        #endregion

        #region Accessors

        private Transform Pool => CommonUtils.GetCached(ref _poolParent, () => {
            var newPool = new GameObject("ConnectionsPool").transform;
            newPool.gameObject.SetActive(false);
            return newPool;
        });

        private Connection ConnectionPrefab => CommonUtils.GetCached(ref _connectionPrefab,
            () => _assetProvider.ProvideAssetByKey<Connection>(_implementationSettings.ConnectionPrefabResourceKey));

        private Connection StaticConnections => CommonUtils.GetCached(ref _staticConnections, () => {
            var newConnection = Object.Instantiate(ConnectionPrefab);
            newConnection.gameObject.name = "StaticConnections";
            newConnection.Init(_implementationSettings);
            return newConnection;
        });
        
        private Connection DynamicConnections => CommonUtils.GetCached(ref _dynamicConnections, () => {
            var newConnection = Object.Instantiate(ConnectionPrefab);
            newConnection.gameObject.name = "DynamicConnections";
            newConnection.Init(_implementationSettings);
            return newConnection;
        });

        #endregion
        
        #region Constructors

        [Inject]
        public ConnectionsManager(SignalBus signalBus) 
        {
            _signalBus = signalBus;
            
            _signalBus.Subscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Subscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
            
            _signalBus.Subscribe<IPawnDragStartedSignal>(OnPawnDragStarted);
            _signalBus.Subscribe<IPawnDraggedSignal>(OnPawnDragged);
            _signalBus.Subscribe<IPawnDragFinishedSignal>(OnPawnDragFinished);
            
            _signalBus.Subscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region Destructor

        ~ConnectionsManager() 
        {
            _signalBus.Unsubscribe<IPawnConnectorActivate>(OnConnectorActivate);
            _signalBus.Unsubscribe<IPawnConnectorDeactivate>(OnConnectorDeactivate);
            
            _signalBus.Unsubscribe<IPawnDragStartedSignal>(OnPawnDragStarted);
            _signalBus.Unsubscribe<IPawnDraggedSignal>(OnPawnDragged);
            _signalBus.Unsubscribe<IPawnDragFinishedSignal>(OnPawnDragFinished);
            
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

            _connectionsData.Add(new []{ start, end });
            _staticData.Clear();
            _staticData.AddRange(_connectionsData);

            RegenerateConnections();
        }

        private void RegenerateConnections() 
        {
            StaticConnections.SetPoints(_staticData.SelectMany(d => d));
            DynamicConnections.SetPoints(_dynamicData.SelectMany(d => d));
        }

        private void OnPawnDragStarted(IPawnDragStartedSignal signal) 
        {
            var allPoints = _connectionsData.SelectMany(d => d);
            var pointsToMove = allPoints.Where(p => p.Parent == signal.Pawn).ToList();
            
            _dynamicData.Clear();
            _dynamicData.AddRange(_connectionsData.Where(d => d.Any(c => pointsToMove.Contains(c))));
            _staticData.Clear();
            _staticData.AddRange(_connectionsData.Where(d => !_dynamicData.Contains(d)));

            RegenerateConnections();
        }

        private void OnPawnDragged(IPawnDraggedSignal signal) 
        {
            DynamicConnections.GenerateLineMesh();
        }

        private void OnPawnDragFinished(IPawnDragFinishedSignal signal) 
        {
            _dynamicData.Clear();
            _staticData.Clear();
            _staticData.AddRange(_connectionsData);
            
            RegenerateConnections();
        }

        private void OnPawnRemoved(IPawnRemovedSignal signal) 
        {
            var dataToRemove = _connectionsData.Where(d => d.Any(c => c.Parent == signal.Pawn));
            _connectionsData.RemoveAll(d => dataToRemove.Contains(d));
            
            _dynamicData.Clear();
            _staticData.Clear();
            _staticData.AddRange(_connectionsData);
            
            RegenerateConnections();
        }

        #endregion
    }
}