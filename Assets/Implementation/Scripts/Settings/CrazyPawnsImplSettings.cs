using UnityEngine;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsImplSettings : ScriptableObject {
        #region Serialized Fields

        [SerializeField] public string CheckerboardColorAParamName = "ColorA";
        
        [SerializeField] public string CheckerboardColorBParamName = "ColorB";
        
        [SerializeField] public string CheckerboardSizeParamName = "Size";
        
        [SerializeField] public float CheckerboardSquareSize = 1;

        [SerializeField] public float CheckerboardScaleCoeff = 1;

        [SerializeField] public string PawnPrefabResourceKey;

        [SerializeField] public string PawnConnectionPrefabResourceKey;
        
        [SerializeField] public string MouseConnectionPrefabResourceKey;
        
        [SerializeField] [Range(0f, 1f)] public float ConnectionLineThickness;

        [SerializeField] public Color ConnectionLineColor;
        
        [SerializeField] public float HoldThreshold = 0.25f;

        [SerializeField] public float DragThreshold = 1f;

        [SerializeField] public float MouseMovementThreshold = 1;

        #endregion
    }
}