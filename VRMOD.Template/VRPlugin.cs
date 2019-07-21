using IllusionPlugin;
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR;
using VRGIN.Core;
using VRMOD.CoreModule;
using VRMOD.Mode;

namespace VRMOD
{

    /// <summary>
    /// This is an example for a VR plugin. At the same time, it also functions as a generic one.
    /// </summary>
    public class VRPlugin : IPlugin
    {

        /// <summary>
        /// Put the name of your plugin here.
        /// </summary>
        public string Name
        {
            get
            {
                return "VRMOD";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Determines when to boot the VR code. In most cases, it makes sense to do the check as described here.
        /// </summary>
        public void OnApplicationStart()
        {
            bool vrDeactivated = Environment.CommandLine.Contains("--novr");
            bool vrActivated = Environment.CommandLine.Contains("--vr");

            VRLog.Info("Start VRMOD");
            if (vrActivated || (!vrDeactivated && XRSettings.isDeviceActive))
            {
                var Manager = VRManager.Create(VRSettings.Load<VRSettings>("VRSettings.xml"));
                Manager.SetMode<SeatedMode>();
            }
        }

        public void OnApplicationQuit() { }
        public void OnFixedUpdate() { }
        public void OnLevelWasInitialized(int level)
        {
        }
        public void OnLevelWasLoaded(int level)
        {
        }
        public void OnUpdate()
        {
        }
    }
}
