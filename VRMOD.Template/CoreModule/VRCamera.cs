using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    public class VRCamera : ProtectedBehaviour
    {
        private static VRCamera _Instance;
        private GameObject eye;

        public static VRCamera Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new GameObject("VRCamera").AddComponent<VRCamera>();
                }
                return _Instance;
            }
        }

        public string CameraName
        {
            get { return eye.name; }
        }

        protected override void OnAwake()
        {
            transform.Reset();

            eye = new GameObject("eye");
            eye.transform.Reset();
            eye.transform.SetParent(transform, false);
            eye.AddComponent<AudioListener>();
            var camera = eye.AddComponent<Camera>();
            // VR用の設定を反映.
            camera.stereoTargetEye = StereoTargetEyeMask.Both;
            camera.nearClipPlane = VR.Settings.NearClipPlane;
            XRSettings.eyeTextureResolutionScale = VR.Settings.IPDScale;

            DontDestroyOnLoad(gameObject);
        }

        public void SyncCamera(Transform t)
        {
            transform.position = t.position;
            transform.rotation = t.rotation;
        }

        public void CopyFX(Camera source)
        {
            if (source != null)
            {
                var target = eye.GetComponent<Camera>();

                // Clean
                foreach (var fx in target.gameObject.GetCameraEffects())
                {
                    DestroyImmediate(fx);
                }
                int comps = target.GetComponents<Component>().Length;

                VRLog.Info("Copying FX to {0}...", target.name);
                // Rebuild
                foreach (var fx in source.gameObject.GetCameraEffects())
                {
                    VRLog.Info("Copy FX: {0} (enabled={1})", fx.GetType().Name, fx.enabled);
                    var attachedFx = target.gameObject.CopyComponentFrom(fx);
                    if (attachedFx)
                    {
                        VRLog.Info("Attached!");
                        attachedFx.enabled = fx.enabled;
                    }
                    else
                    {
                        VRLog.Info("Skipping image effect {0}", fx.GetType().Name);
                    }
                }
                VRLog.Info("{0} components before the additions, {1} after", comps, target.GetComponents<Component>().Length);
            }
        }
    }
}
