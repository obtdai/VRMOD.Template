using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using VRMOD.Extension;
using UnityEngine;
using UnityEngine.XR;
using VRGIN.Core;

namespace VRMOD.Mode
{
    class SeatedMode : ControlMode
    {
        private uDesktopDuplication.Manager manager;
        private uDesktopDuplication.Texture texture;
        private Camera syncCamera;

        protected override void OnAwake()
        {
            manager = uDesktopDuplication.Manager.CreateInstance();

            GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            monitor.name = "Monitor";
            monitor.transform.Reset();
            var renderer = monitor.GetComponent<MeshRenderer>();

            monitor.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            monitor.transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            monitor.transform.localRotation = Quaternion.Euler(new Vector3(-90.0f, 0.0f, 0.0f));
            texture = monitor.AddComponent<uDesktopDuplication.Texture>();
            texture.meshForwardDirection = uDesktopDuplication.Texture.MeshForwardDirection.Y;
            texture.invertX = true;

            monitor.transform.SetParent(transform, false);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            // 同期用のカメラを取得.
            if (Camera.main != null && syncCamera == null)
            {
                syncCamera = Camera.main;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                texture.gameObject.SetActive(!texture.gameObject.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftCommand))
            {
                VRManager.Instance.SetMode<StandingMode>();
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                InputTracking.Recenter();
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
                VRLog.Info($"Can't Find Sync camera!!");

            }
        }
    }
}
