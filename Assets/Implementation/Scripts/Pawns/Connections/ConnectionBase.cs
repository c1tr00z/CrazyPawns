using System.Collections.Generic;
using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public abstract class ConnectionBase : MonoBehaviour 
    {
        #region Private Fields

        private MeshFilter _meshFilter;
        
        private MeshRenderer _renderer;
        
        private Mesh _mesh;

        private Material _lineMaterial;

        private CrazyPawnsImplSettings _implementationSettings;
        
        #endregion

        #region Accessors
        
        protected abstract Vector3[] VectorPoints { get; }

        private MeshFilter MeshFilter => this.GetCachedComponent(ref _meshFilter);

        private Mesh Mesh => CommonUtils.GetCached(ref _mesh, () => {
            var newMesh = new Mesh();
            MeshFilter.mesh = newMesh;
            return newMesh;
        });

        #endregion

        #region Class Implementation

        public void Init(CrazyPawnsImplSettings implementationSettings) 
        {
            _implementationSettings = implementationSettings;
        }
    
        public void GenerateLineMesh()
        {
            if (VectorPoints.Length < 2)
            {
                gameObject.SetActive(false);
                return;
            }
            
            Mesh.Clear();

            if (!gameObject.activeSelf) 
            {
                gameObject.SetActive(true);
            }
        
            Vector3[] vertices = new Vector3[VectorPoints.Length];
            int[] indices = new int[VectorPoints.Length];
        
            for (int i = 0; i < VectorPoints.Length; i++)
            {
                vertices[i] = VectorPoints[i];
                indices[i] = i;
            }
            
            Mesh.vertices = vertices;
            Mesh.SetIndices(indices, MeshTopology.Lines, 0);
        }

        #endregion
    }
}