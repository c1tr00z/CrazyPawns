using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Connection : MonoBehaviour 
    {

        #region Private Fields

        private MeshFilter _meshFilter;
        
        private MeshRenderer _renderer;
        
        private Mesh _mesh;
    
        private List<Transform> _points;

        private Material _lineMaterial;

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsImplSettings ImplementationSettings;

        #endregion

        #region Accessors

        private MeshFilter MeshFilter => this.GetCachedComponent(ref _meshFilter);
        
        private MeshRenderer MeshRenderer => this.GetCachedComponent(ref _renderer);

        private Mesh Mesh => CommonUtils.GetCached(ref _mesh, () => {
            var newMesh = new Mesh();
            MeshFilter.mesh = newMesh;
            return newMesh;
        });

        private Material LineMaterial =>
            CommonUtils.GetCached(ref _lineMaterial, () => {
                var material = Instantiate(MeshRenderer.sharedMaterial);
                return material;
            });

        #endregion

        #region Class Implementation

        public void SetPoints(List<Transform> newPoints)
        {
            _points = newPoints;
            GenerateLineMesh();
        }
    
        public void GenerateLineMesh()
        {
            if (_points.Count < 2)
            {
                return;
            }
        
            Vector3[] vertices = new Vector3[_points.Count];
            int[] indices = new int[_points.Count];
        
            for (int i = 0; i < _points.Count; i++)
            {
                vertices[i] = _points[i].position;
                indices[i] = i;
            }
        
            Mesh.vertices = vertices;
            Mesh.SetIndices(indices, MeshTopology.Lines, 0);
        
            LineMaterial.SetColor("_Color", ImplementationSettings.ConnectionLineColor);
            LineMaterial.SetFloat("_Thickness", ImplementationSettings.ConnectionLineThickness);
        }

        #endregion
    }
}