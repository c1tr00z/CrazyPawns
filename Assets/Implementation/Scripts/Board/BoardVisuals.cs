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

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnSettings CrazyPawnSettings;
        
        [Inject] private CrazyPawnsImplSettings ImplementationSettings;

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
            SetupCheckerboard();
        }

        #endregion

        #region Class Implementation

        private void SetupCheckerboard() {
            var sideScale = ImplementationSettings.CheckerboardScaleCoeff * ImplementationSettings.CheckerboardSquareSize * CrazyPawnSettings.CheckerboardSize;soru
            transform.localScale = new Vector3(sideScale, 1, sideScale);
            
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorAParamName, CrazyPawnSettings.WhiteCellColor);
            CheckerboardMaterial.SetColor(ImplementationSettings.CheckerboardColorBParamName, CrazyPawnSettings.BlackCellColor);
            CheckerboardMaterial.SetFloat(ImplementationSettings.CheckerboardSizeParamName, CrazyPawnSettings.CheckerboardSize);
        }

        #endregion
    }
}