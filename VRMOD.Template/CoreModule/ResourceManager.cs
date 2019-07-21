using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VRGIN.Core;

namespace VRMOD.CoreModule
{
    public class ResourceManager
    {
        private static ResourceManager _Instance;
        private Dictionary<string, Material> _Materials = new Dictionary<string, Material>();

        public static ResourceManager Instance
        {
            get {
                if (_Instance == null)
                {
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
            }
            else
            {
                VRLog.Error("AssetBundle Load Failed.");
            }

        }

        private void LoadMaterials()
        {
            // uDD_Screen_Unlit.mat
            Material mat = _AssetBundle.LoadAsset<Material>("uDD_Screen_Unlit.mat");
            if (mat != null)
            {
                VRLog.Info("Material uDD_Screen_Unlit.mat is Loaded.");
                // モニタ表示用テクスチャを取得
                _Materials.Add("uDD_Screen_Unlit", _AssetBundle.LoadAsset<Material>("uDD_Screen_Unlit.mat"));
            }
            else
            {
                VRLog.Error("Material uDD_Screen_Unlit.mat is Load Failed.");
            }
        }

        public Material MonitorMaterial
        {
            get
            {
                return _Materials["uDD_Screen_Unlit"];
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
