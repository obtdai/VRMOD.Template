using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRMOD.CoreModule
{
    class VRCamera : ProtectedBehaviour
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
            eye.AddComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Both;

            DontDestroyOnLoad(gameObject);
        }

        public void SyncCamera(Transform t)
        {
            transform.position = t.position;
            transform.rotation = t.rotation;
        }
    }
}
