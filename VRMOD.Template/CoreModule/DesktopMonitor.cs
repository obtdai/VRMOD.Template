#define USE_PREFAB

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;
using UnityEngine;
using VRMOD.Extension;
using uDesktopDuplication;




namespace VRMOD.CoreModule
{
    public class DesktopMonitor : ProtectedBehaviour
    {
        public enum CreateType
        {
            Stationary,
            RoomScale
        }

        public static DesktopMonitor Create(CreateType createType)
        {
            VRLog.Info($"Create Virtual Desktop Monitor");
#if USE_PREFAB
            GameObject go = new GameObject("");
#else
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            go.RemoveComponent<MeshCollider>();
#endif
            if (Manager.monitorCount > 0)
            {
                Monitor monitor = Manager.monitors[0];
                go.name = monitor.name;
                go.transform.Reset();
#if USE_PREFAB
                var renderer = go.AddComponent<MeshRenderer>();
#else
                var renderer = go.GetComponent<MeshRenderer>();
#endif

                // MaterialはuDD_Unlit/Texureとする.それがだめならFallbackする.
                if (renderer != null)
                {
                    VRLog.Info($"Material Change");
                    VRLog.Info($"Old Material Name is {renderer.material}");
                    renderer.material = VR.Resource.MonitorMaterial;
                    VRLog.Info($"Material Name is {renderer.material}");
                    if (renderer.material.shader)
                    {
                        VRLog.Info($"Shader Is : {renderer.material.shader.name}");
                        VRLog.Info($"Shader Is Supported Status : {renderer.material.shader.isSupported}");

                        if (renderer.material.shader.isSupported == false)
                        {
                            VRLog.Info("Fallback Shader Standard");
                            var shader = Shader.Find("Standard");
                            if (shader != null)
                            {
                                renderer.material.shader = shader;
                            }
                        }
                    }
                    else
                    {
                        VRLog.Info($"Shader Can't load Check the Material Please...");
                    }
                    // 描画が反転するので、テクスチャーのスケールを反転させる.
                    renderer.material.mainTextureScale = new Vector3(1.0f, 1.0f);
                }

#if USE_PREFAB

                var mesh = VR.Resource.MonitorMesh;
                if (mesh != null)
                {
                    var filter = go.AddComponent<MeshFilter>();
                    filter.mesh = mesh;
                    var aabbScale = mesh.bounds.size;
                    aabbScale.y = Mathf.Max(aabbScale.y, aabbScale.x);
                    aabbScale.z = Mathf.Max(aabbScale.z, aabbScale.x);
                    mesh.bounds = new Bounds(mesh.bounds.center, aabbScale);
                }
#else
                var filter = go.GetComponent<MeshFilter>();
#endif

                // Assign monitor
                var texture = go.AddComponent<uDesktopDuplication.Texture>();
                texture.monitorId = 0;

                // デフォルトだと画面が大きすぎるので調整.
                // モニタ解像度(pixel)
                VRLog.Info($"Monitor Created Monitor Resolution Width:{texture.monitor.width}, Height:{texture.monitor.height}");
                var scale = VR.Settings.MonitorScale;
                float width = 1.0f, height = 1.0f;
#if false // Real Scale
                width = monitor.widthMeter;
                height = monitor.heightMeter;
#elif false // Fixed Scale
                width = scale * (monitor.isHorizontal ? monitor.aspect : 1f);
                height = scale * (monitor.isHorizontal ? 1f : 1f / monitor.aspect);
#else // Pixel Scale
                width = scale * (monitor.isHorizontal ? 1f : monitor.aspect) * ((float)monitor.width / 1920);
                height = scale * (monitor.isHorizontal ? 1f / monitor.aspect : 1f) * ((float)monitor.width / 1920);
#endif
                texture.meshForwardDirection = uDesktopDuplication.Texture.MeshForwardDirection.Z;
                var meshForwardDirection = texture.meshForwardDirection;

                if (createType == CreateType.Stationary)
                {
                    width *= 0.2f;
                    height *= 0.2f;
                }
                else
                {
                    width *= 0.02f;
                    height *= 0.02f;
                }

                if (meshForwardDirection == uDesktopDuplication.Texture.MeshForwardDirection.Y)
                {
                    go.transform.localScale = new Vector3(width, go.transform.localScale.y, height);
                }
                else
                {
                    go.transform.localScale = new Vector3(width, height, go.transform.localScale.z);
                }

                if (createType == CreateType.RoomScale)
                {
                    go.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                }


                VRLog.Info($"Monitor Scale is {scale}");
                VRLog.Info($"Monitor Object Size is {go.transform.localScale}");
                // デフォルトでは非表示にしておく.
                VRLog.Info($"monitor Object Created Success for {createType}:{go.name}");
                go.SetActive(false);
            }
            else
            {
                VRLog.Error("Display Can't Detection");
            }
            return go.AddComponent<DesktopMonitor>();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public bool activeSelf
        {
            get { return gameObject.activeSelf; }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
