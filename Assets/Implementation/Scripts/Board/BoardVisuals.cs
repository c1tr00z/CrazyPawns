using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BoardVisuals : MonoBehaviour 
    {

        #region Private Fields

        private Material _boardMaterial;

        private MeshRenderer _boardMesh;

        private SignalBus _signalBus;

        private bool _boardBuilt = false;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;
        
        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IStateProvider StateProvider;
        
        [Inject] private IStateCompleter StateCompleter;

        #endregion

        #region Accessors

        private MeshRenderer CheckerboardMesh => CommonUtils.GetCached(ref _boardMesh, GetComponent<MeshRenderer>);

        private Material CheckerboardMaterial => CommonUtils.GetCached(ref _boardMaterial, () => {
            var newMaterial = Instantiate(CheckerboardMesh.sharedMaterial);
            CheckerboardMesh.sharedMaterial = newMaterial;
            return newMaterial;
        });

        #endregion

        #region Unity Events

        private void Start() 
        {
            TryToSetupBoard(StateProvider.State);
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<IStateChangedSignal>(OnStateChanged);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<IStateChangedSignal>(OnStateChanged);
        }

        #endregion

        #region Zenject Events

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        #endregion

        #region Class Implementation

        private void OnStateChanged(IStateChangedSignal stateChanged)
        {
            TryToSetupBoard(stateChanged.State);
        }

        private void TryToSetupBoard(State newState)
        {
            if (newState != State.BoardInit)
            {
                return;
            }
            
            SetupCheckerboard();
        }

        private void SetupCheckerboard() {
            if (_boardBuilt) 
            {
                return;
            }
            var sideScale = ImplementationSettings.CheckerboardScaleCoeff * ImplementationSettings.CheckerboardSquareSize * CrazyPawnSettings.CheckerboardSize;
            transform.localScale = new Vector3(sideScale, 1, sideScale);
            
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorAParamName, CrazyPawnSettings.WhiteCellColor);
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorBParamName, CrazyPawnSettings.BlackCellColor);
            CheckerboardMaterial.SetFloat(ImplementationSettings.CheckerboardSizeParamName, CrazyPawnSettings.CheckerboardSize);
            
            _boardBuilt = true;
            
            StateCompleter.CompleteState(State.BoardInit);
        }

        #endregion
    }
}