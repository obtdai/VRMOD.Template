using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRMOD.CoreModule;
using VRGIN.Core;
using VRGIN.Controls;


namespace VRMOD.Mode
{
    public class ControlMode : ProtectedBehaviour
    {
        public Controller Left;
        public Controller Right;
        protected IEnumerable<IShortcut> Shortcuts { get; private set; }

        protected override void OnAwake()
        {
            VRLog.Info("OnAWake");
            base.OnAwake();
            CreateControllers();

            return;
        }
        protected override void OnStart()
        {
            VRLog.Info("OnStart");
            base.OnStart();
            Shortcuts = CreateShortcuts();
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

        protected virtual void CreateControllers()
        {
            // コントローラの生成.
            VRLog.Info("Left Controller Create");
            Left = LeftController.Create();
            Left.transform.SetParent(VR.Camera.transform, false);
            VRLog.Info("Right Controller Create");
            Right = RightController.Create();
            Right.transform.SetParent(VR.Camera.transform, false);

            Left.Other = Right;
            Right.Other = Left;

            return;
        }

 
        protected virtual void OnDestroy()
        {
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
