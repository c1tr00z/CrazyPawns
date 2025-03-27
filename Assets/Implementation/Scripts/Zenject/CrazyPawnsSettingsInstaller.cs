using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsSettingsInstaller : ScriptableObjectInstaller {

        #region Serialized Fields

        [SerializeField] private CrazyPawnSettings _crazyPawnSettingsAsset;
        [SerializeField] private CrazyPawnsImplSettings _implementationSettings;
        [SerializeField] private CrazyPawnsResourcesSettings _resourcesSettings;

        #endregion
        
        #region ScriptableObjectInstaller Implementation

        public override void InstallBindings() 
        {
            Container.Bind<CrazyPawnSettings>().FromInstance(_crazyPawnSettingsAsset).AsSingle();
            Container.Bind<CrazyPawnsImplSettings>().FromInstance(_implementationSettings).AsSingle();
            Container.Bind<CrazyPawnsResourcesSettings>().FromInstance(_resourcesSettings).AsSingle();
        }

        #endregion
    }
}