using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRMOD.Extension
{
    /// <summary>
    /// Component 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// コンポーネントを削除します
        /// </summary>
        public static void RemoveComponent<T>(this Component self) where T : Component
        {
            GameObject.Destroy(self.GetComponent<T>());
        }
    }

}
