using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class PawnConnection : ConnectionBase 
    {

        #region Private Fields

        private List<Transform> _points = new();

        private Material _lineMaterial;

        private CrazyPawnsImplSettings _implementationSettings;
        
        #endregion

        #region ConnectionBase Implementation

        protected override Vector3[] VectorPoints => _points.Select(p => p.position).ToArray();

        #endregion

        #region Class Implementation

        public void SetPoints(IEnumerable<Transform> newPoints)
        {
            _points.Clear();
            _points.AddRange(newPoints);
            GenerateLineMesh();
        }

        #endregion
    }
}