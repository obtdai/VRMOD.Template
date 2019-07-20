using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRMOD.Extension
{
    public static class TransformExtension
    {
        public static void Reset(this Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }
    }
}
