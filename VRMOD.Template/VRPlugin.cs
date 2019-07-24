using IllusionPlugin;
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
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

#if UNITY_2018_3_OR_NEWER
            foreach (var s in UnityEngine.XR.XRSettings.supportedDevices)
            {
                VRLog.Info($"Supported VR Device :{s}");
            }
            if (vrActivated || (!vrDeactivated && UnityEngine.XR.XRSettings.isDeviceActive))
            {
                VRLog.Info("Create VR Manager");
                var Manager = VRManager.Create(VRSettings.Load<VRSettings>("VRSettings.xml"));
                VRLog.Info("VR Manager Created");
                Manager.SetMode<StandingMode>();
            }
#else
            foreach (var s in UnityEngine.VR.VRSettings.supportedDevices)
            {
                VRLog.Info($"Supported VR Device :{s}");
            }
            VRLog.Info($"VR Device Is Status:{UnityEngine.VR.VRSettings.isDeviceActive}");
            VRLog.Info($"VR Mode Enabled Status:{UnityEngine.VR.VRSettings.enabled}");
            if (vrActivated || (!vrDeactivated && UnityEngine.VR.VRSettings.isDeviceActive))
            {
                VRLog.Info("Create VR Manager");
                var Manager = VRManager.Create(VRSettings.Load<VRSettings>("VRSettings.xml"));
                VRLog.Info("VR Manager Created");
                Manager.SetMode<SeatedMode>();
            }
#endif
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
