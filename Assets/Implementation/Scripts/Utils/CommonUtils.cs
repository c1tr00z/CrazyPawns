using System;
using UnityEngine;
namespace CrazyPawn.Implementation {
    public static class CommonUtils {
        #region Class Implementation

        public static T GetCached<T>(ref T item, Func<T> getter) 
        {
            if (item is null) {
                item = getter();
            }
            return item;
        }

        public static T GetCachedComponent<T>(this Component from, ref T componenet) 
        {
            return GetCached(ref componenet, from.GetComponent<T>);
        }

        #endregion
    }
}