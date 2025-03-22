using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsSettingsInstaller : ScriptableObjectInstaller {

        #region Serialized Fields

        [SerializeField] private CrazyPawnSettings CrazyPawnSettingsAsset;
        [SerializeField] private CrazyPawnsImplSettings ImplementationSettings;

        #endregion
        
        #region MonoInstaller Implementation

        public override void InstallBindings() 
        {
            Container.Bind<CrazyPawnSettings>().FromInstance(CrazyPawnSettingsAsset);
            Container.Bind<CrazyPawnsImplSettings>().FromInstance(ImplementationSettings);
        }

        #endregion
    }
}