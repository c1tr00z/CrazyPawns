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

        private PawnConnection _staticPawnConnections;
        
        private PawnConnection _dynamicPawnConnections;

        private MouseConnection _mouseConnection;

        private Queue<PawnConnection> _cached = new();

        private Transform _poolParent;

        private List<PawnConnector[]> _connectionsData = new();
        
        private List<PawnConnector[]> _staticData = new();
        
        private List<PawnConnector[]> _dynamicData = new();

        private PawnConnection _pawnConnectionPrefab;

        private MouseConnection _mouseConnectionPrefab;
        
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

        private PawnConnection PawnConnectionPrefab => CommonUtils.GetCached(ref _pawnConnectionPrefab,
            () => _assetProvider.ProvideAssetByKey<PawnConnection>(_implementationSettings.PawnConnectionPrefabResourceKey));
        
        private MouseConnection MouseConnectionPrefab => CommonUtils.GetCached(ref _mouseConnectionPrefab,
            () => _assetProvider.ProvideAssetByKey<MouseConnection>(_implementationSettings.MouseConnectionPrefabResourceKey));

        private PawnConnection StaticPawnConnections => CommonUtils.GetCached(ref _staticPawnConnections, () => {
            var newConnection = Object.Instantiate(PawnConnectionPrefab);
            newConnection.gameObject.name = "StaticConnections";
            newConnection.Init(_implementationSettings);
            return newConnection;
        });
        
        private PawnConnection DynamicPawnConnections => CommonUtils.GetCached(ref _dynamicPawnConnections, () => {
            var newConnection = Object.Instantiate(PawnConnectionPrefab);
            newConnection.gameObject.name = "DynamicConnections";
            newConnection.Init(_implementationSettings);
            return newConnection;
        });

        private MouseConnection MouseConnection => CommonUtils.GetCached(ref _mouseConnection, () => {
            var newConnection = Object.Instantiate(MouseConnectionPrefab);
            newConnection.gameObject.name = "MouseConnection";
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
            
            _signalBus.Subscribe<IConnectionStartedSignal>(OnConnectionStarted);
            _signalBus.Subscribe<IConnectionDragSignal>(OnConnectionDrag);
            _signalBus.Subscribe<IConnectionFinishedSignal>(OnConnectionFinished);
            
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
            
            _signalBus.Unsubscribe<IConnectionStartedSignal>(OnConnectionStarted);
            _signalBus.Unsubscribe<IConnectionDragSignal>(OnConnectionDrag);
            _signalBus.Unsubscribe<IConnectionFinishedSignal>(OnConnectionFinished);
            
            _signalBus.Unsubscribe<IPawnRemovedSignal>(OnPawnRemoved);
        }

        #endregion

        #region IConnectionPooler Implementation

        public void ReturnToPool(PawnConnection pawnConnection) 
        {
            if (pawnConnection is null) 
            {
                return;
            }
            if (_cached.Contains(pawnConnection)) 
            {
                return;
            }
            _cached.Enqueue(pawnConnection);
            pawnConnection.transform.parent = Pool;
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
            StaticPawnConnections.SetPoints(_staticData.SelectMany(d => d).Select(c => c.transform));
            DynamicPawnConnections.SetPoints(_dynamicData.SelectMany(d => d).Select(c => c.transform));
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
            DynamicPawnConnections.GenerateLineMesh();
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

        private void OnConnectionStarted(IConnectionStartedSignal signal) 
        {
            MouseConnection.gameObject.SetActive(true);
            MouseConnection.SetConnector(signal.Connector);
            MouseConnection.UpdateMouse3DPosition(signal.Mouse3dPosition);
            MouseConnection.GenerateLineMesh();
        }
        
        private void OnConnectionDrag(IConnectionDragSignal signal) 
        {
            MouseConnection.SetConnector(signal.Connector);
            MouseConnection.UpdateMouse3DPosition(signal.Mouse3dPosition);
            MouseConnection.GenerateLineMesh();
        }
        
        private void OnConnectionFinished(IConnectionFinishedSignal signal) 
        {
            MouseConnection.gameObject.SetActive(false);
        }

        #endregion
    }
}