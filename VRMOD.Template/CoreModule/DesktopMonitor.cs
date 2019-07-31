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
            bool is_mesh = false;
            GameObject go;
            MeshFilter filter;
            Mesh mesh;
            Renderer renderer;
            Material material;

            // メッシュのロードを試す.
            VRLog.Info("Mesh Load Start");
            mesh = VR.Resource.MonitorMesh;
            VRLog.Info("Mesh Load End");
            if (mesh != null)
            {
                // 半径を再計算.
                var aabbScale = mesh.bounds.size;
                aabbScale.y = Mathf.Max(aabbScale.y, aabbScale.x);
                aabbScale.z = Mathf.Max(aabbScale.z, aabbScale.x);
                mesh.bounds = new Bounds(mesh.bounds.center, aabbScale);
                is_mesh = true;
            }

            if (is_mesh)
            {
                // メッシュからモニタを生成する.
                go = new GameObject("");
                filter = go.AddComponent<MeshFilter>();
                filter.mesh = mesh;
                renderer = go.AddComponent<MeshRenderer>();
            }
            else
            {
                // メッシュが読み込めないので、QUADから生成する.
                go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.RemoveComponent<MeshCollider>();
                renderer = go.GetComponent<MeshRenderer>();
            }

            if (Manager.monitorCount > 0)
            {
                Monitor monitor = Manager.monitors[0];
                go.name = monitor.name;
                go.transform.Reset();

                // MaterialはuDD_Unlit/Texureとする.それがだめならFallbackする.
                if (renderer != null)
                {
                    VRLog.Info($"Material Change");
                    VRLog.Info($"Old Material Name is {renderer.material}");
                    material = VR.Resource.MonitorMaterial;
                    if (material != null)
                    {
                        renderer.material = material;
                        VRLog.Info($"Material Name is {renderer.material}");
                        if (renderer.material.shader)
                        {
                            VRLog.Info($"Shader Is : {renderer.material.shader.name}");
                            VRLog.Info($"Shader Is Supported Status : {renderer.material.shader.isSupported}");

                            if (renderer.material.shader.isSupported == false)
                            {
                                VRLog.Info("Fallback uDD_Screen_Standard");
                                material = VR.Resource.MonitorStandardMaterial;

                                if (material != null)
                                {
                                    renderer.material = material;
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
                                }
                            }
                        }
                        else
                        {
                            VRLog.Info($"Shader Can't load Check the Material Please...");
                        }
                    }
                    // 描画が反転するので、テクスチャーのスケールを反転させる.
                    if (!is_mesh)
                    {
                        renderer.material.mainTextureScale = new Vector3(1.0f, -1.0f);
                    }
                }
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

                if (!is_mesh)
                {
                    width *= 10.0f;
                    height *= 10.0f;
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
