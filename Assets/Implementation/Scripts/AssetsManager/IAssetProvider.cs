using UnityEngine;
namespace CrazyPawn.Implementation 
{
    public interface IAssetProvider 
    {
        T ProvideAssetByKey<T>(string key) where T : Object;
    }
}