using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRMOD.CoreModule
{
    public class ResourceManager
    {
        private static ResourceManager _Instance;
        private Dictionary<string, Material> _Materials = new Dictionary<string, Material>();
        private Dictionary<string, Mesh> _Meshes = new Dictionary<string, Mesh>();

        public static ResourceManager Instance
        {
            get {
                if (_Instance == null)
                {
                    VRLog.Info("Get ResourceManager");
                    _Instance = new ResourceManager();
                }
                return _Instance;
            }
        }
        private AssetBundle _AssetBundle;

        private ResourceManager()
        {
            VRLog.Info("Resource Manager Create");
            // AssetBundleのロード.
            _AssetBundle = AssetBundle.LoadFromMemory(AssetBundleMemory);

            if (_AssetBundle != null)
            {
                VRLog.Info("Asset Bundle is Loaded.");
                LoadMaterials();
                WarmUpShader();
                LoadMeshes();

            }
            else
            {
                VRLog.Error("AssetBundle Load Failed.");
            }

        }

        private void LoadMeshes()
        {
            
            Mesh mesh = _AssetBundle.LoadAsset<Mesh>("uDD_Board");

            if (mesh != null)
            {
                VRLog.Info("Mesh uDD_Board.fbx is Loaded.");
                _Meshes.Add("uDD_Board", mesh);
            }
            else
            {
                VRLog.Error("Mesh uDD_Board.fbx is Loaded Failed.");
            }
        }

        private void WarmUpShader()
        {
            VRLog.Info("Warm up all shaders");
            Shader.WarmupAllShaders();

            return;
        }
        private void LoadMaterials()
        {
            // uDD_Screen_Unlit.mat
            Material mat = _AssetBundle.LoadAsset<Material>("uDD_Screen_Unlit.mat");
            if (mat != null)
            {
                VRLog.Info("Material uDD_Screen_Unlit.mat is Loaded.");
                mat.SetInt("_Forward", 1);
                mat.DisableKeyword("_FORWARD_Y");
                mat.EnableKeyword("_FORWARD_Z");
                // モニタ表示用テクスチャを取得
                _Materials.Add("uDD_Screen_Unlit", mat);
            }
            else
            {
                VRLog.Error("Material uDD_Screen_Unlit.mat is Load Failed.");
            }

            // uDD_Screen_Standard.mat
            mat = _AssetBundle.LoadAsset<Material>("uDD_Screen_Standard.mat");
            if (mat != null)
            {
                VRLog.Info("Material uDD_Screen_Standard.mat is Loaded.");
                mat.SetInt("_Forward", 1);
                mat.DisableKeyword("_FORWARD_Y");
                mat.EnableKeyword("_FORWARD_Z");
                // モニタ表示用テクスチャを取得
                _Materials.Add("uDD_Screen_Standard", mat);
            }
            else
            {
                VRLog.Error("Material uDD_Screen_Unlit.mat is Load Failed.");
            }

            // uTI_Cursor.mat
            mat = null;
            mat = _AssetBundle.LoadAsset<Material>("uTI_Cursor.mat");
            if (mat != null)
            {
                VRLog.Info("Material uTI_Cursor.mat is Loaded.");
                _Materials.Add("uTI_Cursor", mat);
            }

            // uTI_Ray.mat
            mat = null;
            mat = _AssetBundle.LoadAsset<Material>("uTI_Ray.mat");
            if (mat != null)
            {
                VRLog.Info("Material uTI_Ray.mat is Loaded.");
                _Materials.Add("uTI_Ray", mat);
            }
        }

        public Material MonitorMaterial
        {
            get
            {
                if (_Materials.ContainsKey("uDD_Screen_Unlit"))
                {
                    return _Materials["uDD_Screen_Unlit"];
                }
                return null;
            }
        }

        public Material MonitorStandardMaterial
        {
            get
            {
                if (_Materials.ContainsKey("uDD_Screen_Standard"))
                {
                    return _Materials["uDD_Screen_Standard"];
                }
                return null;
            }
        }

        public Material TouchCursorMaterial
        {
            get
            {
                if (_Materials.ContainsKey("uTI_Cursor"))
                {
                    return _Materials["uTI_Cursor"];
                }
                return null;
            }
        }

        public Material TouchRayMaterial
        {
            get
            {
                if (_Materials.ContainsKey("uTI_Ray"))
                {
                    return _Materials["uTI_Ray"];
                }
                return null;
            }
        }

        public Mesh MonitorMesh
        {
            get
            {
                if (_Meshes.ContainsKey("uDD_Board"))
                {
                    return _Meshes["uDD_Board"];
                }
                return null;
            }
        }




        private byte[] AssetBundleMemory
        {
            get
            {
                // Unityバージョンにより切り替え.
#if UNITY_5_6_OR_NEWER
                return Properties.Resources.vrmod_5;
#elif UNITY_2018_3_OR_NEWER
                return Properties.Resources.vrmod_2018_3;
#endif
            }
        }

    }
}
