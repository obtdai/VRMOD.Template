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
            // AssetBundleのロード.
            _AssetBundle = AssetBundle.LoadFromMemory(AssetBundleMemory);

            // モニタ表示用テクスチャを取得
            _Materials.Add("uDD_Screen_Unlit", _AssetBundle.LoadAsset<Material>("uDD_Screen_Unlit.mat"));
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
                return Properties.Resources.vrmod_2018_3;
            }
        }

    }
}
