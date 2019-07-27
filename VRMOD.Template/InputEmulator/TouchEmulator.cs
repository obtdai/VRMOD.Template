using UnityEngine;
using VRGIN.Core;
using uTouchInjection;
using VRGIN.Controls;
using VRMOD.CoreModule;
using VRMOD.Extension;

namespace VRMOD.InputEmulator
{
    public class TouchEmulator : ProtectedBehaviour
    {

        public static TouchEmulator Create()
        {
            VRLog.Info("Touch Emulator Create");
            GameObject touchEmulator = new GameObject("Touch Emulator");
            touchEmulator.transform.Reset();

            var renderer = touchEmulator.AddComponent<LineRenderer>();

            // Material はuTI_Rayとする。それがだめならFallbackする
            if (renderer != null)
            {
                VRLog.Info($"Material Change");
                renderer.material = VR.Resource.TouchRayMaterial;
                VRLog.Info($"Material Name is {renderer.material}");
                if (renderer.material.shader)
                {
                    VRLog.Info($"Shader Is : {renderer.material.shader.name}");
                    VRLog.Info($"Shader Is Supported Status : {renderer.material.shader.isSupported}");

                    if (renderer.material.shader.isSupported == false)
                    {
                        VRLog.Info("Fallback Shader Standard");
                        var shader = Shader.Find("Standard");
                        if (shader != null)
                        {
                            renderer.material.shader = shader;
                        }
                    }
                }
                else
                {
                    VRLog.Info($"Shader Can't load Check the Material Please...");
                }
            }

            var result = touchEmulator.AddComponent<TouchEmulator>();
            // PointerDrawerを追加
            VRLog.Info("Add Pointer Drawer");
            var drawer = touchEmulator.AddComponent<TouchPointerDrawer>();
#if true
            GameObject cursor = GameObject.CreatePrimitive(PrimitiveType.Quad);
            cursor.transform.Reset();
            cursor.transform.localScale *= 0.01f;
            cursor.transform.SetParent(touchEmulator.transform);
            var cursorRenderer = cursor.GetComponent<Renderer>();
            
            // Material はuTI_Cursorとする。それがだめならFallbackする
            if (cursorRenderer != null)
            {
                VRLog.Info($"Material Change");
                cursorRenderer.material = VR.Resource.TouchCursorMaterial;
                VRLog.Info($"Material Name is {cursorRenderer.material}");
                if (cursorRenderer.material.shader)
                {
                    VRLog.Info($"Shader Is : {cursorRenderer.material.shader.name}");
                    VRLog.Info($"Shader Is Supported Status : {cursorRenderer.material.shader.isSupported}");

                    if (cursorRenderer.material.shader.isSupported == false)
                    {
                        VRLog.Info("Fallback Shader Standard");
                        var shader = Shader.Find("Standard");
                        if (shader != null)
                        {
                            cursorRenderer.material.shader = shader;
                        }
                    }
                }
                else
                {
                    VRLog.Info($"Shader Can't load Check the Material Please...");
                }
            }
            drawer.cursor = cursor;
#endif
            return result;
        }

        private static int currentId = 0;

        Pointer pointer_;
        bool isFirstTouch_ = true;

        [SerializeField, Range(0f, 1f)] float filter = 0.8f;
        [SerializeField] float maxRayDistance = 9999f;

        public Controller controller { get; set; }

        public uDesktopDuplication.Texture.RayCastResult result
        {
            get;
            private set;
        }

        public bool isPrimaryPointer
        {
            get { return pointer_ != null && pointer_.id == 0; }
        }

        public enum State
        {
            Release,
            Hover,
            Touch,
        }

        public State state
        {
            get;
            set;
        }

        public Vector2 filteredDesktopCoord
        {
            get;
            private set;
        }

        void GetPointer()
        {
            if (pointer_ != null) return;

            pointer_ = uTouchInjection.Manager.GetPointer(currentId);
            currentId++;
        }

        void ReleasePointer()
        {
            if (pointer_ == null) return;

            pointer_.Release();
            pointer_ = null;
            currentId--;
        }

        protected override void OnStart()
        {
            VRLog.Info("OnStart");
            state = State.Release;
        }

        protected override void OnUpdate()
        {
            if (controller != null)
            {
                UpdateTouch();
                UpdateState();
            }
        }

        void UpdateTouch()
        {
            result = uDesktopDuplication.Texture.RayCastAll(transform.position, transform.forward * maxRayDistance);

            if (pointer_ == null) return;

            if (result.hit)
            {
                if (isFirstTouch_)
                {
                    filteredDesktopCoord = result.desktopCoord;
                    isFirstTouch_ = false;
                }
                else
                {
                    filteredDesktopCoord += (result.desktopCoord - filteredDesktopCoord) * (Time.deltaTime * 60) * (1f - filter);
                }
            }

            pointer_.position = filteredDesktopCoord;
        }

        void UpdateState()
        {
            if (!result.hit)
            {
                StartRelease();
                return;
            }

            switch (state)
            {
                case State.Release:
                    if (controller.TriggerDown)
                    {
                        StartTouch();
                    }
                    else
                    {
                        StartHover();
                    }
                    break;
                case State.Hover:
                    Hover();
                    if (controller.TriggerDown)
                    {
                        StartTouch();
                    }
                    break;
                case State.Touch:
                    Touch();
                    if (controller.TriggerUp)
                    {
                        StartRelease();
                    }
                    break;
            }
        }

        void StartRelease()
        {
            ReleasePointer();
            state = State.Release;
        }

        void StartHover()
        {
            GetPointer();
            state = State.Hover;
        }

        void StartTouch()
        {
            isFirstTouch_ = true;
            state = State.Touch;
        }

        void Hover()
        {
            GetPointer();
            pointer_.Hover();
        }

        void Touch()
        {
            GetPointer();
            pointer_.Touch();
        }
    }

}