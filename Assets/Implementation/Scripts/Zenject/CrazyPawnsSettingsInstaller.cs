using UnityEngine;
using Zenject;
namespace CrazyPawn.Implementation {
    public class CrazyPawnsSettingsInstaller : ScriptableObjectInstaller {

        #region Serialized Fields

        [SerializeField] private CrazyPawnSettings CrazyPawnSettingsAsset;

        #endregion
        
        #region MonoInstaller Implementation

        public override void InstallBindings() 
        {
            Container.Bind<CrazyPawnSettings>().FromInstance(CrazyPawnSettingsAsset);
        }

        #endregion
    }
}