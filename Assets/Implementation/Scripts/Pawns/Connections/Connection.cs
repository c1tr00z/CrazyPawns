using System.Collections.Generic;
using UnityEngine;
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

        private List<PawnConnector> _points = new();

        private Material _lineMaterial;

        private CrazyPawnsImplSettings _implementationSettings;
        
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

        public IEnumerable<PawnConnector> Points => _points;

        #endregion

        #region Class Implementation

        public void Init(CrazyPawnsImplSettings implementationSettings) 
        {
            _implementationSettings = implementationSettings;
        }

        public void SetPoints(IEnumerable<PawnConnector> newPoints)
        {
            _points.Clear();
            _points.AddRange(newPoints);
            GenerateLineMesh();
        }
    
        public void GenerateLineMesh()
        {
            if (_points.Count < 2)
            {
                gameObject.SetActive(false);
                return;
            }
            
            Mesh.Clear();

            if (!gameObject.activeSelf) 
            {
                gameObject.SetActive(true);
            }
        
            Vector3[] vertices = new Vector3[_points.Count];
            int[] indices = new int[_points.Count];
        
            for (int i = 0; i < _points.Count; i++)
            {
                vertices[i] = _points[i].transform.position;
                indices[i] = i;
            }
            
            Mesh.vertices = vertices;
            Mesh.SetIndices(indices, MeshTopology.Lines, 0);
        
            LineMaterial.SetColor("_Color", _implementationSettings.ConnectionLineColor);
            LineMaterial.SetFloat("_Thickness", _implementationSettings.ConnectionLineThickness);
        }

        #endregion
    }
}