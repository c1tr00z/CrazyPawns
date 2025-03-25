using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsResourcesSettings : ScriptableObject {

        #region Nested Classes

        [Serializable]
        public struct AssetData
        {
            public string Key;
            public string ResourcePath;

            public bool IsValid() 
            {
                return !Key.IsNullOrEmpty() && !ResourcePath.IsNullOrEmpty();
            }
        }

        #endregion

        #region Serialized Fields

        [SerializeField] public List<AssetData> AssetsData = new();

        [SerializeField] public string PawnPath;

        #endregion
    }
}