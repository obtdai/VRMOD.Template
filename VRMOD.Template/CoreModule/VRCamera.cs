using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    public class VRCamera : ProtectedBehaviour
    {
        private SteamVR_Camera _SteamVR_Camera;

        public static VRCamera Create()
        {
            return new GameObject("VR Camera").AddComponent<VRCamera>();
        }
        public Transform Origin
        {
            get
            {
                return _SteamVR_Camera.origin;
            }
        }

        public Transform Head
        {
            get
            {
                return _SteamVR_Camera.head;
            }
        }


        public string CameraName
        {
            get { return gameObject.name; }
        }

        protected override void OnAwake()
        {
            VRLog.Info("OnAWake");
            transform.Reset();
            VRLog.Info("Create VR Camera");
            // VR用の設定を反映.


            VRLog.Info("Add Audio Component");
            gameObject.AddComponent<AudioListener>();
            VRLog.Info("Add Camera Component");
            // VR設定の反映.
            var camera = gameObject.AddComponent<Camera>();
            camera.stereoTargetEye = StereoTargetEyeMask.Both;
            camera.nearClipPlane = VR.Settings.NearClipPlane;
            camera.cameraType = CameraType.VR;

            _SteamVR_Camera = gameObject.AddComponent<SteamVR_Camera>();

            return;
        }

        public void SyncCamera(Transform t, Vector3 positionOffset)
        {
            Origin.position = (t.position + positionOffset);
            Origin.rotation = t.rotation;
        }

        public void SyncCameraPosition(Vector3 position)
        {
            Origin.position = position;
        }

        public void CopyFX(Camera source)
        {
            if (source != null)
            {
                var target = GetComponent<Camera>();

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
