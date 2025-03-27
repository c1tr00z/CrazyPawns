using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation 
{
    // Реализовал управленеи ассетами через синхронную загрузку из ресурсов искобчительно для демонстрационных целей и в целях скорости имплементации. В продакшене так не надо
    public class AssetManager : IAssetProvider
    {

        #region Private Fields

        private Dictionary<string, Object> _loadedAssets = new();

        #endregion
        
        #region Injected Fields

        [Inject] private CrazyPawnsResourcesSettings _resourcesSettings;

        #endregion

        #region IAssetProvider Implementation

        public T ProvideAssetByKey<T>(string key) where T : Object 
        {
            if (_loadedAssets.TryGetValue(key, out var asset)) {
                return asset as T;
            }
            var newlyLoaded = LoadAsset<T>(key);
            _loadedAssets.Add(key, newlyLoaded);
            return newlyLoaded;
        }

        #endregion

        #region Class Implementation

        private T LoadAsset<T>(string key) where T : Object 
        {
            var assetData = _resourcesSettings.AssetsData.FirstOrDefault(d => d.Key == key);
            if (!assetData.IsValid()) 
            {
                throw new UnityException($"Asset data for key {key} is not exist or invalid");
            }
            var asset = Resources.Load<T>(assetData.ResourcePath);
            if (asset is null) 
            {
                throw new UnityException($"Asset with key {key} and path {assetData.ResourcePath} is not {typeof(T).FullName} or not exist");
            }
            return asset;
        }
        
        #endregion
    }
}