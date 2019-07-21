using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    /// <summary>
    /// Helper class that gives you easy access to all crucial objects.
    /// </summary>
    public static class VR
    {
        public static VRCamera Camera { get { return VRCamera.Instance; } }
        public static ControlMode Mode { get { return VRManager.Instance.Mode; } }
        public static VRSettings Settings { get { return VRManager.Settings; } }
        public static Shortcuts Shortcuts { get { return VRManager.Settings.Shortcuts; } }
        public static VRManager Manager { get { return VRManager.Instance; } }

        public static ResourceManager Resource { get { return ResourceManager.Instance; } }

        public static uDesktopDuplication.Manager MonitorManager { get { return VRManager.MonitorManager; } }
        public static bool Active { get; set; }
    }

    public class VRManager : ProtectedBehaviour
    {
        private static VRManager _Instance;
        private static VRSettings _Settings;

        private static uDesktopDuplication.Manager _MonitorManager;
        public VRCamera Camera
        {
            get { return VRCamera.Instance; }
        }

        public ControlMode Mode
        {
            get;
            private set;
        }

        public static VRSettings Settings
        {
            get {
                if (_Settings == null)
                {
                    throw new InvalidOperationException("VR Settings has not been created yet!");
                }
                return _Settings;
            }
        }
        
        public static uDesktopDuplication.Manager MonitorManager
        {
            get
            {
                if (_MonitorManager == null)
                {
                    _MonitorManager = uDesktopDuplication.Manager.CreateInstance();
                }
                return _MonitorManager;
            }
        }

        private HashSet<Camera> _CheckedCameras = new HashSet<Camera>();

        public static VRManager Create(VRSettings settings)
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("VRManager").AddComponent<VRManager>();
                _Settings = settings;
            }
            return _Instance;

        }
        public static VRManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    throw new InvalidOperationException("VR Manager has not been created yet!");
                }
                return _Instance;
            }
        }

        public void SetMode<T>() where T : ControlMode
        {
            if (Mode == null || !(Mode is T))
            {
                // Mode Change
                if (Mode != null)
                {
                    // Get on clean grounds
                    DestroyImmediate(Mode);
                }

                Mode = VRCamera.Instance.gameObject.AddComponent<T>();
            }
        }

        protected override void OnAwake()
        {
            // 自分自身の位置と回転を初期化.
            VRLog.Info("OnAWake");

            XRSettings.showDeviceView = false;
            _MonitorManager = uDesktopDuplication.Manager.CreateInstance();

            // VR用設定の更新.

            DontDestroyOnLoad(gameObject);
        }
        protected override void OnStart()
        {
            VRLog.Info("OnStart");
        }

        protected override void OnLevel(int level)
        {
            VRLog.Info("OnLevel");
            VRLog.Info($"Level:{level}");
            _CheckedCameras.Clear();
        }

        protected override void OnUpdate()
        {
            NormalCameraWithoutVR();

        }

        private void NormalCameraWithoutVR()
        {
            foreach (var camera in UnityEngine.Camera.allCameras.Except(_CheckedCameras).ToList())
            {
                if (camera != null && Camera != null)
                {
                    _CheckedCameras.Add(camera);
                    // 元々あったカメラは通常通り画面に表示.
                    if (camera.name != Camera.CameraName)
                    {
                        VRLog.Info($"Founded Camera {camera.name} Show HMD is Canceld.");
                        camera.gameObject.RemoveComponent<AudioListener>();
                        camera.stereoTargetEye = StereoTargetEyeMask.None;
                    }
                }
            }
        }
    }
}
