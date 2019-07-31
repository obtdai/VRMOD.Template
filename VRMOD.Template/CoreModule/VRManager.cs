using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRMOD.Mode;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    /// <summary>
    /// Helper class that gives you easy access to all crucial objects.
    /// </summary>
    public static class VR
    {
        public static VRCamera Camera { get { return VRManager.Instance.Camera; } }
        public static VRSettings Settings { get { return VRManager.Settings; } }
        public static Shortcuts Shortcuts { get { return VRManager.Settings.Shortcuts; } }
        public static VRManager Manager { get { return VRManager.Instance; } }
        public static ResourceManager Resource { get { return ResourceManager.Instance; } }

        public static uDesktopDuplication.Manager MonitorManager { get { return VRManager.MonitorManager; } }

        public static uTouchInjection.Manager TouchManager { get { return VRManager.TouchManager; } }

        public static SteamVR_Render Render { get { return VRManager.Render; } }
        public static bool Active { get; set; }
    }

    public class VRManager : ProtectedBehaviour
    {
        private ControlMode _ControlMode;
        private static VRManager _Instance;
        private static VRSettings _Settings;
        private static SteamVR_Render _Render;
        private static VRCamera _Camera;

        private static uDesktopDuplication.Manager _MonitorManager;

        private static uTouchInjection.Manager _TouchManager;

        public VRCamera Camera
        {
            get
            {
                return _Camera;
            }
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

        public static uTouchInjection.Manager TouchManager
        {
            get
            {
                if (_TouchManager == null)
                {
                    _TouchManager = uTouchInjection.Manager.CreateInstance();
                }
                return _TouchManager;
            }
        }

        public static SteamVR_Render Render
        {
            get
            {
                if (_Render == null)
                {
                    throw new InvalidOperationException("SteamVR_Render has not been created yet!");
                }
                return _Render;

            }
        }

        private HashSet<Camera> _CheckedCameras = new HashSet<Camera>();

        public static VRManager Create(VRSettings settings)
        {
            if (_Instance == null)
            {
                _Settings = settings;
                _Instance = new GameObject("VRManager").AddComponent<VRManager>();

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

        public void SetMode(ControlMode.ModeType Mode)
        {
            if (Mode != _ControlMode.Mode)
            {
                var go = _ControlMode.gameObject;
                DestroyImmediate(_ControlMode);

                if (Mode == ControlMode.ModeType.SeatedMode)
                {
                    _ControlMode = go.AddComponent<SeatedMode>();
                }
                else
                {
                    _ControlMode = go.AddComponent<StandingMode>();
                }
            }
        }

        protected override void OnAwake()
        {
            // 自分自身の位置と回転を初期化.
            VRLog.Info("OnAWake");
#if UNITY_2018_3_OR_NEWER
            UnityEngine.XR.XRSettings.showDeviceView = false;
#else
            UnityEngine.VR.VRSettings.showDeviceView = false;
#endif

#if UNITY_2018_3_OR_NEWER
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = VR.Settings.IPDScale;
#else
            UnityEngine.VR.VRSettings.renderScale = VR.Settings.IPDScale;
#endif

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

            VRLog.Info("MonitorManager Created");
            _MonitorManager = uDesktopDuplication.Manager.CreateInstance();
            VRLog.Info("TouchManager Created");
            _TouchManager = uTouchInjection.Manager.CreateInstance();
            VRLog.Info("SteamVR Render Created");
            _Render = SteamVR_Render.instance;

            // VR用設定の更新.
            VRLog.Info("VR Camera Created");
            _Camera = VRCamera.Create();
            // モード初期化.
            if ((_ControlMode == null) || (_ControlMode.Mode == ControlMode.ModeType.SeatedMode))
            {
                _ControlMode = new GameObject("Mode").AddComponent<SeatedMode>();
            }
            else
            {
                _ControlMode = new GameObject("Mode").AddComponent<StandingMode>();
            }
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
