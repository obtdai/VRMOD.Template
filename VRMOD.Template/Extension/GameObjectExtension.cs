using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
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

        public static IEnumerable<MonoBehaviour> GetCameraEffects(this GameObject go)
        {
            return go.GetComponents<MonoBehaviour>().Where(IsCameraEffect);
        }

        private static bool IsCameraEffect(MonoBehaviour component)
        {
            return IsImageEffect(component.GetType());
        }

        public static int Level(this GameObject go)
        {
            return go.transform.parent ? go.transform.parent.gameObject.Level() + 1 : 0;
        }

        private static bool IsImageEffect(Type type)
        {
            return type != null && (type.Name.EndsWith("Effect") || type.Name.Contains("AmbientOcclusion") || IsImageEffect(type.BaseType));
        }
        public static T CopyComponentFrom<T>(this GameObject destination, T original) where T : Component
        {
            Type type = original.GetType();
            T copy = destination.AddComponent(type) as T;
            // Copied fields can be restricted with BindingFlags
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }

            return copy;
        }
    }
}
