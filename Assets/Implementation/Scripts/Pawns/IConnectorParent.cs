using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public interface IConnectorParent : IPawnStateProvider
    {
        #region Accessors

        public Material CurrentMaterial { get; }

        #endregion
    }
}