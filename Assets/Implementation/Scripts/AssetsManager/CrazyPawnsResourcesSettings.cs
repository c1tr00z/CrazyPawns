using System;
using System.Collections.Generic;
using UnityEngine;
namespace CrazyPawn.Implementation {
    /// <summary>
    /// Вручную созданный список из resource ссылок и ключей для использования в других местах
    /// </summary>
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

        #endregion
    }
}