using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using VRMOD.Extension;
using UnityEngine;
using UnityEngine.XR;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;

namespace VRMOD.Mode
{
    class SeatedMode : ControlMode
    {
        private GameObject monitor;
        private Camera syncCamera;

        protected override void OnAwake()
        {
            base.OnAwake();
            // 座位モードに変更.
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);
            VRLog.Info("OnAWake()");
            CreateMonitor();
            MoveMonitor(VR.Camera.transform);
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VRLog.Info("Mode Change to Standing Mode"); VR.Manager.SetMode<StandingMode>(); }),
                new KeyboardShortcut(new KeyStroke("Space"), () => {
                    if (monitor.activeSelf)
                    {
                        monitor.SetActive(false);
                        VRLog.Info("Monitor inactive");
                    }
                    else
                    {
                        MoveMonitor(VR.Camera.transform);
                        monitor.SetActive(true);
                        VRLog.Info($"Monitor active, show position is {monitor.transform.position}");
                    }
                }),
                new KeyboardShortcut(new KeyStroke("R"), () => { VRLog.Info("VR Camera Recenterd"); InputTracking.Recenter(); })

            });
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            // 同期用のカメラを取得.
            if (Camera.main != null && syncCamera == null)
            {
                syncCamera = Camera.main;
            }
            
        }

        protected override void OnLateUpdate()
        {
            // sync camerapos
            if (syncCamera != null)
            {
                VRManager.Instance.Camera.SyncCamera(syncCamera.transform);
            }
            else
            {
            }
        }

        protected override void OnLevel(int level)
        {

            base.OnLevel(level);
            VRLog.Info("OnLevel");
            VRLog.Info($"Level:{level}");

            CreateMonitor();
            MoveMonitor(VR.Camera.transform);
        }

        private void CreateMonitor()
        {
            float MONITOR_WIDTH_BASE = 1.92f * 0.025f;
            float MONITOR_HEIGHT_BASE = 1.08f * 0.025f;

            monitor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            monitor.name = "Monitor";
            monitor.transform.Reset();
            var renderer = monitor.GetComponent<MeshRenderer>();

            // MaterialはuDD_Unlit/Texureとする.
            if (renderer != null)
            {
                renderer.material = VR.Resource.MonitorMaterial;
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
            monitor.SetActive(false);
        }

        private void MoveMonitor(Transform origin)
        {
            // モニタに位置をカメラ位置と同じにする.
            monitor.transform.position = origin.transform.position;
            monitor.transform.rotation = origin.transform.rotation;

            // モニタをHMDの向きに合わせて回転する.
            monitor.transform.Rotate(InputTracking.GetLocalRotation(XRNode.Head).eulerAngles);
            // モニタの位置を設定値分オフセットする.
            monitor.transform.position += (monitor.transform.forward * VR.Settings.Distance);

            // モニタから原点を見る.
            monitor.transform.LookAt(origin);
            monitor.transform.Rotate(new Vector3(-90.0f, 180.0f, 0.0f));

        }
    }
}
