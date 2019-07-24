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
        public enum CtrlType
        {
            Left = 0,
            Right = 1,
        };

        public enum Button
        {
            Menu = 0,
            Trackpad,
            Trigger,
            Grip,
        };
        public Controller Other { get; set; }

        protected CtrlType _Type;

        public bool IsValid { get; private set; }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        static KeyCode[][] buttonCode =  {
        new KeyCode[] {
            KeyCode.JoystickButton2,    //"Left Controller Menu Button (1)",
            KeyCode.JoystickButton8,    //"Left Controller Trackpad (2)",
            KeyCode.JoystickButton14,   //"Left Controller Trigger (7)",
            KeyCode.JoystickButton4,    //"Left Controller Grip Button (8)",
        },

        new KeyCode[] {
            KeyCode.JoystickButton0,    //"Right Controller Trackpad (2)",
            KeyCode.JoystickButton9,    //"Right Controller Menu Button (1)",
            KeyCode.JoystickButton15,   //"Right Controller Trigger (7)",
            KeyCode.JoystickButton5,    //"Right Controller Grip Button (8)",
        },
    };

        bool first = false;

        UInt32 _MenuButtonState;
        UInt32 _GripButtonState;
        UInt32 _TriggerState;
        UInt32 _TrackpadButtonState;

        public bool MenuButton
        {
            get { return ((_MenuButtonState & 0x01) == 0x01) ? true : false; }
        }

        public bool MenuButtonDown
        {
            get { return ((_MenuButtonState & 0x03) == 0x01) ? true : false; }
        }

        public bool MenuButtonUp
        {
            get { return ((_MenuButtonState & 0x03) == 0x02) ? true : false; }
        }

        public bool GripButton
        {
            get { return ((_GripButtonState & 0x01) == 0x01) ? true : false; }
        }

        public bool GripButtonDown
        {
            get { return ((_GripButtonState & 0x03) == 0x01) ? true : false; }
        }

        public bool GripButtonUp
        {
            get { return ((_GripButtonState & 0x03) == 0x02) ? true : false; }
        }

        public bool Trigger
        {
            get { return ((_TriggerState & 0x01) == 0x01) ? true : false; }
        }

        public bool TriggerDown
        {
            get { return ((_TriggerState & 0x03) == 0x01) ? true : false; }
        }

        public bool TriggerUp
        {
            get { return ((_TriggerState & 0x03) == 0x02) ? true : false; }
        }

        public bool TrackpadButton
        {
            get { return ((_TrackpadButtonState & 0x01) == 0x01) ? true : false; }
        }

        public bool TrackpadButtonDown
        {
            get { return ((_TrackpadButtonState & 0x03) == 0x01) ? true : false; }
        }

        public bool TrackpadButtonUp
        {
            get { return ((_TrackpadButtonState & 0x03) == 0x02) ? true : false; }
        }

        int searchOpenVRController(string[] array, string LeftOrRight = "Left")
        {
            int idx = -1;
            int i = 0;
            foreach (var name in array)
            {
                if (name.IndexOf("OpenVR") >= 0 && name.IndexOf(LeftOrRight) >= 0)
                {
                    idx = i;
                    break;
                }
                i++;
            }
            return idx;
        }

        int checkAlive()
        {
            var array = Input.GetJoystickNames();
            if (!first)
            {
                VRLog.Info("##input device count:" + array.Length);
                foreach (var name in array)
                {
                    VRLog.Info(name);
                }
                first = true;
            }
            int idx = -1;
            if (_Type == CtrlType.Left)
            {
                idx = searchOpenVRController(array, "Left");
            }
            else
            {
                idx = searchOpenVRController(array, "Right");
            }
            return idx;
        }

#if UNITY_2018_3_OR_NEWER
        Quaternion rotate
        {
            get
            {
                if (_Type == CtrlType.Left)
                {
                    return UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.LeftHand);
                }
                else
                {
                    return UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.RightHand);
                }
            }
        }

        Vector3 position
        {
            get
            {
                if (_Type == CtrlType.Left)
                {
                    return UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
                }
                else
                {
                    return UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand);
                }
            }
        }
#else
        Quaternion rotate
        {
            get
            {
                if (_Type == CtrlType.Left)
                {
                    return UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.LeftHand);
                }
                else
                {
                    return UnityEngine.VR.InputTracking.GetLocalRotation(UnityEngine.VR.VRNode.RightHand);
                }
            }
        }

        Vector3 position
        {
            get
            {
                if (_Type == CtrlType.Left)
                {
                    return UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.LeftHand);
                }
                else
                {
                    return UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.RightHand);
                }
            }
        }
#endif


        public KeyCode GetButtonCode(Button btnType)
        {
            return buttonCode[(int)_Type][(int)btnType];
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            base.OnUpdate();
            int idx = checkAlive();
            if (idx >= 0)
            {
                transform.localRotation = rotate;
                transform.localPosition = position;

                // ボタン状態の更新
                // メニューボタン
                _MenuButtonState <<= 1;
                if (Input.GetKey(GetButtonCode(Button.Menu)))
                {
                    _MenuButtonState |= 0x00000001;
                }
                else
                {
                    _MenuButtonState &= 0xFFFFFFFE;
                }
                // グリップボタン
                _GripButtonState <<= 1;
                if (Input.GetKey(GetButtonCode(Button.Grip)))
                {
                    _GripButtonState |= 0x00000001;
                }
                else
                {
                    _GripButtonState &= 0xFFFFFFFE;
                }
                // トリガー
                _TriggerState <<= 1;
                if (Input.GetKey(GetButtonCode(Button.Trigger)))
                {
                    _TriggerState |= 0x00000001;
                }
                else
                {
                    _TriggerState &= 0xFFFFFFFE;
                }
                // トラックパッド押下
                _TrackpadButtonState <<= 1;
                if (Input.GetKey(GetButtonCode(Button.Trackpad)))
                {
                    _TrackpadButtonState |= 0x00000001;
                }
                else
                {
                    _TrackpadButtonState &= 0xFFFFFFFE;
                }
                IsValid = true;
            }
            else
            {
                // ボタン状態の更新(リリース側)
                _MenuButtonState <<= 1;
                _GripButtonState <<= 1;
                _TriggerState <<= 1;
                _TrackpadButtonState <<= 1;
                IsValid = false;
            }
        }
    }
}
