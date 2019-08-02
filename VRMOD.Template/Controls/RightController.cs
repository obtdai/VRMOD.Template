using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRMOD.Extension;
using VRMOD.CoreModule;

namespace VRMOD.Controls
{
    class RightController : Controller
    {
        public static Controller Create()
        {
            RightController controller;
            controller = new GameObject("Right Controller").AddComponent<RightController>();
            controller.transform.Reset();

            return controller;
        }

        protected override void OnAwake()
        {
            VRLog.Info("OnAwake");
            base.OnAwake();
        }
    }
}
