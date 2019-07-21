using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using UnityEngine;


namespace VRMOD.Mode
{
    class StandingMode : ControlMode
    {
        protected override void OnAwake()
        {
#if UNITY_2018_3_OR_NEWER
            UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.RoomScale);
#else
            UnityEngine.VR.VRDevice.SetTrackingSpaceType(UnityEngine.VR.TrackingSpaceType.RoomScale);
#endif
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftCommand))
            {
                VRManager.Instance.SetMode<SeatedMode>();
            }
        }
    }
}
