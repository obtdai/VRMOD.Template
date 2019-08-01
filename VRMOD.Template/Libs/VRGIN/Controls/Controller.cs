using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRMOD.Extension;

namespace VRGIN.Controls
{
    public abstract class Controller : ProtectedBehaviour
    {
        SteamVR_TrackedObject _TrackedObject;
        SteamVR_RenderModel _RenderModel;

        protected override void OnAwake()
        {
            base.OnAwake();
            VRLog.Info("OnAwake");
            _TrackedObject = gameObject.AddComponent<SteamVR_TrackedObject>();
            var Model = new GameObject("Model");
            Model.transform.Reset();
            Model.transform.SetParent(transform, false);
            _RenderModel = Model.AddComponent<SteamVR_RenderModel>();

            return;
        }

        protected override void OnStart()
        {
            base.OnStart();

            VRLog.Info("OnStart");
        }

        public void SetModelActive(bool active)
        {
            _RenderModel.gameObject.SetActive(active);
        }
        public bool MenuButton
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu);
            }
        }

        public bool MenuButtonDown
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu);
            }
        }

        public bool MenuButtonUp
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu);
            }
        }

    public bool GripButton
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPress(SteamVR_Controller.ButtonMask.Grip);
            }
        }

        public bool GripButtonDown
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressDown(SteamVR_Controller.ButtonMask.Grip);
            }
        }

        public bool GripButtonUp
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressUp(SteamVR_Controller.ButtonMask.Grip);
            }
        }

        public bool Trigger
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPress(SteamVR_Controller.ButtonMask.Trigger);
            }
        }

        public bool TriggerDown
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
            }
        }

        public bool TriggerUp
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
            }
        }

        public bool TrackpadButton
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPress(SteamVR_Controller.ButtonMask.Touchpad);
            }
        }

        public bool TrackpadButtonDown
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressDown(SteamVR_Controller.ButtonMask.Touchpad);
            }
        }

        public bool TrackpadButtonUp
        {
            get
            {
                return SteamVR_Controller.Input((int)_TrackedObject.index).GetPressUp(SteamVR_Controller.ButtonMask.Touchpad);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected virtual void OnDestroy()
        {
            VRLog.Info("On Destroy");

            return;
        }
    }
}
