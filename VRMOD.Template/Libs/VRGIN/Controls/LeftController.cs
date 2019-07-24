using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRMOD.Extension;
using VRGIN.Core;
using UnityEngine;

namespace VRGIN.Controls
{
    class LeftController : Controller
    {
        public static Controller Create()
        {
            LeftController controller;
            controller = new GameObject("Left Controller").AddComponent<LeftController>();
            controller.transform.Reset();

            return controller;
        }

        protected override void OnAwake()
        {
            VRLog.Info("OnAwake");
            base.OnAwake();
            _Type = CtrlType.Left;
        }
    }
}
