using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Controls;

namespace VRMOD.CoreModule
{
    public class ControlMode : ProtectedBehaviour
    {
        protected IEnumerable<IShortcut> Shortcuts { get; private set; }

        protected override void OnStart()
        {
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
