using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRGIN.Core;
using UnityEngine;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    public class DesktopMonitor : ProtectedBehaviour
    {
        public static DesktopMonitor Create()
        {
            GameObject monitor;
            float MONITOR_WIDTH_BASE = 1.92f * 0.025f;
            float MONITOR_HEIGHT_BASE = 1.08f * 0.025f;

            VRLog.Info($"Create Virtual Desktop Monitor");
            monitor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            monitor.name = "Monitor";
            monitor.transform.Reset();
            var renderer = monitor.GetComponent<MeshRenderer>();

            // MaterialはuDD_Unlit/Texureとする.それがだめならFallbackする.
            if (renderer != null)
            {
                VRLog.Info($"Material Change");
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
                renderer.material.mainTextureScale = new Vector3(-1.0f, 1.0f);
            }
            var texture = monitor.AddComponent<uDesktopDuplication.Texture>();
            // モニタ解像度(pixel)
            VRLog.Info($"Monitor Created Monitor Resolution Width:{texture.monitor.width}, Height:{texture.monitor.height}");
            // モニタ解像度(pixel) の

            // デフォルトだと画面が大きすぎるので調整.
            var scale = VR.Settings.MonitorScale;
            var monitorSize = new Vector3(MONITOR_WIDTH_BASE * scale, 1.0f, MONITOR_HEIGHT_BASE * scale);
            VRLog.Info($"Monitor Create Scale is {monitorSize}");
            monitor.transform.localScale = monitorSize;

            // デフォルトでは非表示にしておく.
            VRLog.Info($"monitor Object Created Success:{monitor.name}");
            monitor.SetActive(false);
            return monitor.AddComponent<DesktopMonitor>();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public bool activeSelf
        {
            get { return gameObject.activeSelf; }
        }
    }
}
