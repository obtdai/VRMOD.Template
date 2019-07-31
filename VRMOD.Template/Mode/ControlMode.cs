using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRMOD.CoreModule;
using VRGIN.Core;
using VRGIN.Controls;
using VRMOD.Extension;


namespace VRMOD.Mode
{
    public class ControlMode : ProtectedBehaviour
    {
        public enum ModeType
        {
            SeatedMode,
            StandingMode
        }

        public virtual ModeType Mode
        {
            get;
        }

        private SteamVR_ControllerManager _ControllerManager;
        public Controller Left;
        public Controller Right;
        protected IEnumerable<IShortcut> Shortcuts { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
            VRLog.Info("OnAWake");
            Shortcuts = CreateShortcuts();
            CreateControllers();
            VR.Camera.transform.Reset();

            return;
        }
        protected override void OnStart()
        {
            VRLog.Info("OnStart");
            base.OnStart();

        }

        protected virtual IEnumerable<IShortcut> CreateShortcuts()
        {
            return new List<IShortcut>()
            {
                new KeyboardShortcut(VR.Shortcuts.ShrinkWorld, delegate { VR.Settings.IPDScale += Time.deltaTime; }),
                new KeyboardShortcut(VR.Shortcuts.EnlargeWorld, delegate { VR.Settings.IPDScale -= Time.deltaTime; }),
                new MultiKeyboardShortcut(VR.Shortcuts.SaveSettings, delegate { VR.Settings.Save(); }),
                new KeyboardShortcut(VR.Shortcuts.LoadSettings, delegate { VR.Settings.Reload(); }),
                new KeyboardShortcut(VR.Shortcuts.ResetSettings, delegate { VR.Settings.Reset(); }),
                new KeyboardShortcut(VR.Shortcuts.ApplyEffects, delegate { VR.Camera.CopyFX(Camera.main); }),
            };
        }

        protected override void OnLevel(int level)
        {
            base.OnLevel(level);

        }
        protected virtual void CreateControllers()
        {
            // コントローラマネージャの生成
            VRLog.Info("SteamVR Controller Manager Create");
            _ControllerManager = gameObject.AddComponent<SteamVR_ControllerManager>();
            _ControllerManager.enabled = false;
            // コントローラの生成.
            VRLog.Info("Left Controller Create");
            Left = LeftController.Create();
            VRLog.Info("Right Controller Create");
            Right = RightController.Create();


            _ControllerManager.left = Left.gameObject;
            _ControllerManager.right = Right.gameObject;
            _ControllerManager.UpdateTargets();
            _ControllerManager.enabled = true;

            Left.transform.SetParent(VR.Camera.Origin, true);
            Right.transform.SetParent(VR.Camera.Origin, true);
            return;
        }

 
        protected virtual void OnDestroy()
        {
            VRLog.Info("On Destroy");
            DestroyImmediate(_ControllerManager);
            DestroyImmediate(Left.gameObject);
            DestroyImmediate(Right.gameObject);

            return;
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            CheckInput();
        }

        protected void CheckInput()
        {
            
            foreach (var shortcut in Shortcuts)
            {
                shortcut.Evaluate();
            }
        }
    }
}
