﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using VRGIN.Helpers;
using VRGIN.Controls;
using VRGIN.Core;
using UnityEngine;
using VRMOD.Extension;
using VRMOD.InputEmulator;

namespace VRMOD.Mode
{
    class StandingMode : ControlMode
    {
        // 画面表示用モニタ
        DesktopMonitor Monitor;
        TouchEmulator Emulator;
        //1フレーム前と差を比較するために前回のコントローラ位置・回転を格納する変数
        private Vector3 beforeLeftControllerPosition;
        private Quaternion beforeLeftControllerRotation;
        private Vector3 beforeRightControllerPosition;
        private Quaternion beforeRightControllerRotation;
        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override IEnumerable<IShortcut> CreateShortcuts()
        {
            return base.CreateShortcuts().Concat(new IShortcut[] {
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+C"), () => { VRLog.Info("Mode Change to Seated Mode"); VR.Manager.SetMode<SeatedMode>(); }),
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+S"), () => {  VRLog.Info("Camera Position Sync Main Camera"); SyncCameraForRoomScale(); }),
                new MultiKeyboardShortcut(new KeyStroke("Ctrl+C"), new KeyStroke("Ctrl+R"), () => { VRLog.Info("Camera Position Reset to {0, 0, 0}"); VR.Camera.transform.Reset(); }),
            });
        }

        protected override void OnStart()
        {
            base.OnStart();

            VRLog.Info("OnStart()");
#if UNITY_2018_3_OR_NEWER
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);
#else
            UnityEngine.VR.VRDevice.SetTrackingSpaceType(UnityEngine.VR.TrackingSpaceType.RoomScale);
#endif

            Monitor = DesktopMonitor.Create(DesktopMonitor.CreateType.RoomScale);
            Emulator = TouchEmulator.Create();
            Emulator.transform.SetParent(Right.transform, false);
            SyncCameraForRoomScale();
        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);
            VRLog.Info("OnLevel");
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            GripMoveLeft();
            GripMoveRight();
            MonitorDisplayChange();
            ReSyncCamera();

        }

        protected override void OnDestroy()
        {
            VRLog.Info("On Destroy");
            DestroyImmediate(Emulator.gameObject);
            DestroyImmediate(Monitor.gameObject);
            base.OnDestroy();
            return;
        }

        private void ReSyncCamera()
        {
            // 右コントローラーのトラックパッドボタン押下でカメラを再同期.
            if (Right.TrackpadButtonDown)
            {
                SyncCameraForRoomScale();
            }
        }

        private void SyncCameraForRoomScale()
        {
            if (Camera.main != null)
            {
                Vector3 pos = Camera.main.transform.position;
                pos.y = Camera.main.transform.position.y - VR.Camera.transform.position.y;
                VR.Camera.SyncCameraPosition(pos);
            }

        }
        private void MonitorDisplayChange()
        {
            // モニタをメニューボタンを押下した側に移動するか、関連付け中にメニューボタンを押したら表示／非表示を切り替える.
            if (Monitor != null)
            {
                if (Left.MenuButtonDown)
                {
                    if (Monitor.transform.parent == Left.transform)
                    {
                        Monitor.SetActive(!Monitor.isActiveAndEnabled);
                    }
                    else
                    {
                        Monitor.transform.SetParent(Left.transform, false);
                        Emulator.transform.SetParent(Right.transform, false);
                        Emulator.controller = Right;
                        Monitor.SetActive(true);
                    }
                }
                else if (Right.MenuButtonDown)
                {
                    if (Monitor.transform.parent == Right.transform)
                    {
                        Monitor.SetActive(!Monitor.isActiveAndEnabled);
                    }
                    else
                    {
                        Monitor.transform.SetParent(Right.transform, false);
                        Emulator.transform.SetParent(Left.transform, false);
                        Emulator.controller = Left;
                        Monitor.SetActive(true);
                    }
                }
            }
            else
            {
                VRLog.Info("Monitor Destroyed Recreated Yet");
                Monitor = DesktopMonitor.Create(DesktopMonitor.CreateType.RoomScale);
            }
        }
        private void GripMoveLeft()
        {
            //現在(移動後)のコントローラの位置・回転を格納
            Vector3 afterControllerPosition = Left.transform.position;
            Quaternion afterControllerRotation = Left.transform.rotation;

            //"Grip"Moveなのでグリップボタンを入力したときに移動処理を実行
            if (Left.GripButton)
            {
                //1フレーム前と比較したコントローラの移動距離を算出
                Vector3 distanceDifference = beforeLeftControllerPosition - afterControllerPosition;

                //1フレーム前と比較したコントローラのY軸回転を算出
                float rotationDifferenceY = Mathf.DeltaAngle(beforeLeftControllerRotation.eulerAngles.y, afterControllerRotation.eulerAngles.y);

                //コントローラが移動した距離分、CameraRigを移動
                VR.Camera.transform.position += distanceDifference;

                //コントローラのY軸が回転した分、コントローラ位置を軸としてCameraRigを回転移動
                VR.Camera.transform.RotateAround(afterControllerPosition, Vector3.down, rotationDifferenceY);
            }
            //現在のコントローラの位置・回転を次回に移動前として使うために格納
            beforeLeftControllerPosition = afterControllerPosition;
            beforeLeftControllerRotation = afterControllerRotation;
        }

        private void GripMoveRight()
        {
            //現在(移動後)のコントローラの位置・回転を格納
            Vector3 afterControllerPosition = Right.transform.position;
            Quaternion afterControllerRotation = Right.transform.rotation;

            //"Grip"Moveなのでグリップボタンを入力したときに移動処理を実行
            if (Right.GripButton)
            {
                //1フレーム前と比較したコントローラの移動距離を算出
                Vector3 distanceDifference = beforeRightControllerPosition - afterControllerPosition;

                //1フレーム前と比較したコントローラのY軸回転を算出
                float rotationDifferenceY = Mathf.DeltaAngle(beforeRightControllerRotation.eulerAngles.y, afterControllerRotation.eulerAngles.y);

                //コントローラが移動した距離分、CameraRigを移動
                VR.Camera.transform.position += distanceDifference;

                //コントローラのY軸が回転した分、コントローラ位置を軸としてCameraRigを回転移動
                VR.Camera.transform.RotateAround(afterControllerPosition, Vector3.down, rotationDifferenceY);
            }
            //現在のコントローラの位置・回転を次回に移動前として使うために格納
            beforeRightControllerPosition = afterControllerPosition;
            beforeRightControllerRotation = afterControllerRotation;
        }

    }
}
