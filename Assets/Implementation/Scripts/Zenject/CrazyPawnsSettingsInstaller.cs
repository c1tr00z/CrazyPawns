using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsSettingsInstaller : ScriptableObjectInstaller {

        #region Serialized Fields

        [SerializeField] private CrazyPawnSettings CrazyPawnSettingsAsset;
        [SerializeField] private CrazyPawnsImplSettings ImplementationSettings;
        [SerializeField] private CrazyPawnsResourcesSettings ResourcesSettings;

        #endregion
        
        #region ScriptableObjectInstaller Implementation

        public override void InstallBindings() 
        {
            Container.Bind<CrazyPawnSettings>().FromInstance(CrazyPawnSettingsAsset).AsSingle();
            Container.Bind<CrazyPawnsImplSettings>().FromInstance(ImplementationSettings).AsSingle();
            Container.Bind<CrazyPawnsResourcesSettings>().FromInstance(ResourcesSettings).AsSingle();
        }

        #endregion
    }
}