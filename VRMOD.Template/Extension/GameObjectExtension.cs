using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRMOD.Extension
{
    public static class GameObjectExtension
    {
        /// <summary>
        /// コンポーネントを削除します
        /// </summary>
        public static void RemoveComponent<T>(this GameObject self) where T : Component
        {
            GameObject.Destroy(self.GetComponent<T>());
        }
    }
}
