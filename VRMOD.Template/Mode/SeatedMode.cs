using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using VRMOD.Extension;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Controls;
using VRGIN.Helpers;

namespace VRMOD.Mode
{
    class SeatedMode : ControlMode
    {
        private DesktopMonitor monitor;
        private Camera syncCamera;

        protected override void OnAwake()
        {
            VRLog.Info("OnAWake()");
            base.OnAwake();
            // 座位モードに変更.
#if UNITY_2018_3_OR_NEWER
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.Stationary);
#else
            UnityEngine.VR.VRDevice.SetTrackingSpaceType(UnityEngine.VR.TrackingSpaceType.Stationary);
#endif
            monitor = DesktopMonitor.Create();
            MoveMonitor(VR.Camera.transform);
        }

        protected override void OnStart()
        {
            base.OnStart();
            Left.gameObject.SetActive(false);
            Right.gameObject.SetActive(false);
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
#if UNITY_2018_3_OR_NEWER
                new KeyboardShortcut(new KeyStroke("R"), () => { VRLog.Info("VR Camera Recenterd"); UnityEngine.XR.InputTracking.Recenter(); })
#else
                new KeyboardShortcut(new KeyStroke("R"), () => { VRLog.Info("VR Camera Recenterd"); UnityEngine.VR.InputTracking.Recenter(); })
#endif

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

        protected override void OnDestroy()
        {
            VRLog.Info("On Destroy");
            DestroyImmediate(monitor.gameObject);

            return;
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
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

            monitor = DesktopMonitor.Create();
            MoveMonitor(VR.Camera.transform);
        }



        private void MoveMonitor(Transform origin)
        {
            // モニタに位置をカメラ位置と同じにする.
            monitor.transform.position = origin.transform.position;
            monitor.transform.rotation = origin.transform.rotation;

            // モニタをHMDの向きに合わせて回転する.
#if UNITY_2018_3_OR_NEWER
            monitor.transform.Rotate(UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head).eulerAngles);
#else
            monitor.transform.Rotate(UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.Head).eulerAngles);
#endif
            // モニタの位置を設定値分オフセットする.
            monitor.transform.position += (monitor.transform.forward * VR.Settings.Distance);

            // モニタから原点を見る.
            monitor.transform.LookAt(origin);
            monitor.transform.Rotate(new Vector3(-90.0f, 180.0f, 0.0f));

        }
    }
}
