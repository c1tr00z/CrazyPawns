using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(MeshRenderer))]
    public class BoardVisuals : MonoBehaviour 
    {

        #region Private Fields

        private Material BoardMaterial;

        private MeshRenderer BoardMesh;

        private SignalBus SignalBus;

        private bool BoardBuilt = false;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;
        
        [Inject] private CrazyPawnsImplSettings ImplementationSettings;
        
        [Inject] private IStateProvider StateProvider;

        #endregion

        #region Accessors

        private MeshRenderer CheckerboardMesh => CommonUtils.GetCached(ref BoardMesh, GetComponent<MeshRenderer>);

        private Material CheckerboardMaterial => CommonUtils.GetCached(ref BoardMaterial, () => {
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
            SignalBus.Subscribe<StateChangedSignal>(OnStateChanged);
        }

        private void OnDisable()
        {
            SignalBus.Unsubscribe<StateChangedSignal>(OnStateChanged);
        }

        #endregion

        #region Zenject Events

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        #endregion

        #region Class Implementation

        private void OnStateChanged(StateChangedSignal stateChanged)
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
            if (BoardBuilt) 
            {
                return;
            }
            var sideScale = ImplementationSettings.CheckerboardScaleCoeff * ImplementationSettings.CheckerboardSquareSize * CrazyPawnSettings.CheckerboardSize;
            transform.localScale = new Vector3(sideScale, 1, sideScale);
            
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorAParamName, CrazyPawnSettings.WhiteCellColor);
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorBParamName, CrazyPawnSettings.BlackCellColor);
            CheckerboardMaterial.SetFloat(ImplementationSettings.CheckerboardSizeParamName, CrazyPawnSettings.CheckerboardSize);
            
            BoardBuilt = true;
        }

        #endregion
    }
}