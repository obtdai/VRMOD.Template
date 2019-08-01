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

        public override ModeType Mode
        {
            get { return ModeType.SeatedMode; }
        }
        protected override void OnAwake()
        {
            base.OnAwake();
            VRLog.Info("OnAWake()");
            // 座位モードに変更.
            VR.Render.trackingSpace = Valve.VR.ETrackingUniverseOrigin.TrackingUniverseSeated;
            monitor = DesktopMonitor.Create(DesktopMonitor.CreateType.Stationary);
            MoveMonitor(VR.Camera.Head);

            Left.enabled = false;
            Right.enabled = false;

            DontDestroyOnLoad(monitor.gameObject);

            return;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VRLog.Info("Mode Change to Standing Mode"); VR.Manager.SetMode(ModeType.StandingMode); }),
                new KeyboardShortcut(new KeyStroke("Ctrl+Space"), () => {
                    if (monitor != null)
                    {

                        if (monitor.activeSelf)
                        {
                            monitor.SetActive(false);
                            VRLog.Info("Monitor inactive");
                        }
                        else
                        {
                            MoveMonitor(VR.Camera.Head);
                            monitor.SetActive(true);
                            VRLog.Info($"Monitor active, show position is {monitor.transform.position}");
                        }
                    }
                    else
                    {
                        VRLog.Info($"Monitor is Destroyed, Recreated Yet");
                        monitor = DesktopMonitor.Create(DesktopMonitor.CreateType.Stationary);

                    }
                }),
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+R"), () => { SteamVR.instance.hmd.ResetSeatedZeroPose(); })

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
            base.OnDestroy();

            return;
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            // sync camerapos
            if (syncCamera != null)
            {
                VRManager.Instance.Camera.SyncCamera(syncCamera.transform, Vector3.zero);
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
        }

        private void MoveMonitor(Transform origin)
        {
            // モニタに位置をカメラ位置と同じにする.
            monitor.transform.position = origin.transform.position;
            monitor.transform.rotation = origin.transform.rotation;

            // モニタをHMDの向きに合わせて回転する.
            // TODO SteamVRのカメラ位置取得におきかえ.
#if UNITY_2018_3_OR_NEWER
            monitor.transform.Rotate(UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.Head).eulerAngles);
#else
            monitor.transform.Rotate(UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.Head).eulerAngles);
#endif
            // モニタの位置を設定値分オフセットする.
            monitor.transform.position += (monitor.transform.forward * VR.Settings.Distance);

            // モニタから原点を見る.
            monitor.transform.LookAt(origin);
            monitor.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        }
    }
}
