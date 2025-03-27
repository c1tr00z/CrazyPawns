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

        #endregion
    }
}