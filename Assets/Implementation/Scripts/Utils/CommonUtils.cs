using System;
namespace CrazyPawn.Implementation {
    public class CommonUtils {
        #region Class Implementation

        public static T GetCached<T>(ref T item, Func<T> getter) 
        {
            if (item == null) {
                item = getter();
            }
            return item;
        }

        #endregion
    }
}