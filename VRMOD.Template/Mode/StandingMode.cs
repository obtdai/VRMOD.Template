using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.CoreModule;
using UnityEngine;
using UnityEngine.XR;

namespace VRMOD.Mode
{
    class StandingMode : ControlMode
    {
        protected override void OnAwake()
        {
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
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
