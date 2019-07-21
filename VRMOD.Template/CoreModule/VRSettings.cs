using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;
using VRGIN.Core;
using VRGIN.Helpers;

namespace VRMOD.CoreModule
{
    /// <summary>
    /// Class that holds settings for VR. Saved as an XML file.
    /// 
    /// In order to create your own settings file, extend this class and add your own properties. Make sure to call <see cref="TriggerPropertyChanged(string)"/> if you want to use
    /// the events.
    /// IMPORTANT: When extending, add an XmlRoot annotation to the class like so:
    /// <code>[XmlRoot("Settings")]</code>
    /// </summary>
    [XmlRoot("Settings")]
    public class VRSettings
    {
        private VRSettings _OldSettings;
        private IDictionary<string, IList<EventHandler<PropertyChangedEventArgs>>> _Listeners = new Dictionary<string, IList<EventHandler<PropertyChangedEventArgs>>>();

        [XmlIgnore]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the distance between the camera and the Monitor at [0,0,0] [seated]
        /// </summary>
        [XmlComment("The distance between the camera and the GUI at [0,0,0] [seated]")]
        public float Distance { get { return _Distance; } set { _Distance = Mathf.Clamp(value, 0.1f, 10f); TriggerPropertyChanged("Distance"); } }
        private float _Distance = 0.5f;

        /// <summary>
        /// Show Monitor Scale. [seated]
        /// </summary>
        [XmlComment("The Monitor Scale. [seated]")]
        public float MonitorScale { get { return _MonitorScale; } set { _MonitorScale = Mathf.Clamp(value, 0.2f, 5.0f); TriggerPropertyChanged("MonitorScale"); } }
        private float _MonitorScale = 1.0f;

        /// <summary>
        /// Gets or sets the scale of the camera. The higher, the more gigantic the player is.
        /// </summary>
        [XmlComment("Scale of the camera. The higher, the more gigantic the player is.")]
        public float IPDScale { get { return _IPDScale; } set { _IPDScale = Mathf.Clamp(value, 0.01f, 50f); TriggerPropertyChanged("IPDScale"); } }
        private float _IPDScale = 1f;

        /// <summary>
        /// Gets or sets the Near Clip Plane of Camra.
        /// </summary>
        [XmlComment("Near Clip Plane Camera")]
        public float NearClipPlane { get { return _NearClipPlane; } set { _NearClipPlane = Mathf.Clamp(value, 0.0001f, 50f); TriggerPropertyChanged("NearClipPlane"); } }
        private float _NearClipPlane = 0.1f;

        /// <summary>
        /// Gets or sets the render scale of the renderer. Increase for better quality but less performance, decrease for more performance but poor quality.
        /// </summary>
        [XmlComment("The render scale of the renderer. Increase for better quality but less performance, decrease for more performance but poor quality. ]0..2]")]
        public float RenderScale { get { return _RenderScale; } set { _RenderScale = Mathf.Clamp(value, 0.1f, 4f); TriggerPropertyChanged("RenderScale"); } }
        private float _RenderScale = 1f;

        [XmlComment("How quickly the the view should rotate when doing so with the controllers.")]
        public float RotationMultiplier { get { return _RotationMultiplier; } set { _RotationMultiplier = value; TriggerPropertyChanged("RotationMultiplier"); } }
        private float _RotationMultiplier = 1f;

        [XmlComment("Shortcuts used by VRMOD. Refer to https://docs.unity3d.com/ScriptReference/KeyCode.html for a list of available keys.")]
        public virtual Shortcuts Shortcuts { get { return _Shortcuts; } protected set { _Shortcuts = value; } }
        private Shortcuts _Shortcuts = new Shortcuts();

        public event EventHandler<PropertyChangedEventArgs> PropertyChanged = delegate { };

        public VRSettings()
        {
            PropertyChanged += Distribute;

            _OldSettings = this.MemberwiseClone() as VRSettings;
        }

        /// <summary>
        /// Triggers a PropertyChanged event and notifies the listeners.
        /// </summary>
        /// <param name="name"></param>
        protected void TriggerPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Saves the settings to the path that was initially set.
        /// </summary>
        public virtual void Save()
        {
            Save(Path);
        }

        /// <summary>
        /// Saves the settings to a given path.
        /// </summary>
        /// <param name="path"></param>
        public virtual void Save(string path)
        {
            if (path != null)
            {
                var serializer = new XmlSerializer(GetType());
                using (var stream = File.OpenWrite(path))
                {
                    stream.SetLength(0);
                    serializer.Serialize(stream, this);
                }

                PostProcess(path);

                Path = path;
            }

            _OldSettings = this.MemberwiseClone() as VRSettings;
        }

        protected virtual void PostProcess(string path)
        {
            // Add comments
            var doc = XDocument.Load(path);
            foreach (var element in doc.Root.Elements())
            {
                var property = FindProperty(element.Name.LocalName);
                if (property != null)
                {
                    var commentAttribute = property.GetCustomAttributes(typeof(XmlCommentAttribute), true).FirstOrDefault() as XmlCommentAttribute;
                    if (commentAttribute != null)
                    {
                        element.AddBeforeSelf(new XComment(" " + commentAttribute.Value + " "));
                    }
                }
            }
            doc.Save(path);
        }

        private PropertyInfo FindProperty(string name)
        {
            return GetType()
                .FindMembers(MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public, Type.FilterName, name)
                .FirstOrDefault() as PropertyInfo;
        }

        /// <summary>
        /// Loads the settings from a file. Generic to enable handling of sub classes.
        /// </summary>
        /// <typeparam name="T">Type of the settings</typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : VRSettings
        {
            VRLog.Info("VRSettings Load Start");
            try
            {
                if (!File.Exists(path))
                {
                    VRLog.Info("VRSettings Not Exist Create New XML File.");
                    var settings = Activator.CreateInstance<T>();
                    settings.Save(path);
                    return settings;
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (var stream = new FileStream(path, FileMode.Open))
                    {
                        var settings = serializer.Deserialize(stream) as T;
                        settings.Path = path;
                        VRLog.Info("VRSettings Load End");
                        return settings;
                    }
                }
                
            }
            catch (Exception e)
            {
                VRLog.Error("Fatal exception occured while loading XML! (Make sure System.Xml exists!) {0}", e);
                throw e;
            }

        }

        /// <summary>
        /// Adds a listener for a certain property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="handler"></param>
        public void AddListener(string property, EventHandler<PropertyChangedEventArgs> handler)
        {
            if (!_Listeners.ContainsKey(property))
            {
                _Listeners[property] = new List<EventHandler<PropertyChangedEventArgs>>();
            }

            _Listeners[property].Add(handler);
        }


        /// <summary>
        /// Removes a listener for a certain property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="handler"></param>
        public void RemoveListener(string property, EventHandler<PropertyChangedEventArgs> handler)
        {
            if (_Listeners.ContainsKey(property))
            {
                _Listeners[property].Remove(handler);
            }
        }

        private void Distribute(object sender, PropertyChangedEventArgs e)
        {
            if (!_Listeners.ContainsKey(e.PropertyName))
            {
                _Listeners[e.PropertyName] = new List<EventHandler<PropertyChangedEventArgs>>();
            }

            foreach (var listener in _Listeners[e.PropertyName])
            {
                listener(sender, e);
            }
        }

        /// <summary>
        /// Resets all values.
        /// </summary>
        public void Reset()
        {
            var blueprint = Activator.CreateInstance(this.GetType()) as VRSettings;
            this.CopyFrom(blueprint);
        }

        /// <summary>
        /// Restores the last saved state.
        /// </summary>
        public void Reload()
        {
            this.CopyFrom(_OldSettings);
        }

        /// <summary>
        /// Clone settings from another instance.
        /// </summary>
        /// <param name="settings"></param>
        public void CopyFrom(VRSettings settings)
        {
            foreach (var key in _Listeners.Keys)
            {
                var prop = settings.GetType().GetProperty(key, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null)
                {
                    try
                    {
                        prop.SetValue(this, prop.GetValue(settings, null), null);
                    }
                    catch (Exception e)
                    {
                        VRLog.Warn(e);
                    }
                }
            }
        }

    }

    public class Shortcuts
    {
        public XmlKeyStroke ResetView = new XmlKeyStroke("F12");
        public XmlKeyStroke ChangeMode = new XmlKeyStroke("Ctrl+C, Ctrl+C");
        public XmlKeyStroke ShrinkWorld = new XmlKeyStroke("Alt + KeypadMinus", KeyMode.Press);
        public XmlKeyStroke EnlargeWorld = new XmlKeyStroke("Alt + KeypadPlus", KeyMode.Press);
        public XmlKeyStroke ToggleUserCamera = new XmlKeyStroke("Ctrl+C, Ctrl+V");
        public XmlKeyStroke SaveSettings = new XmlKeyStroke("Alt + S");
        public XmlKeyStroke LoadSettings = new XmlKeyStroke("Alt + L");
        public XmlKeyStroke ResetSettings = new XmlKeyStroke("Ctrl + Alt + L");
        public XmlKeyStroke ApplyEffects = new XmlKeyStroke("Ctrl + F5");

        [XmlElement("GUI.Raise")]
        public XmlKeyStroke GUIRaise = new XmlKeyStroke("KeypadMinus", KeyMode.Press);
        [XmlElement("GUI.Lower")]
        public XmlKeyStroke GUILower = new XmlKeyStroke("KeypadPlus", KeyMode.Press);
        [XmlElement("GUI.IncreaseAngle")]
        public XmlKeyStroke GUIIncreaseAngle = new XmlKeyStroke("Ctrl + KeypadMinus", KeyMode.Press);
        [XmlElement("GUI.DecreaseAngle")]
        public XmlKeyStroke GUIDecreaseAngle = new XmlKeyStroke("Ctrl + KeypadPlus", KeyMode.Press);
        [XmlElement("GUI.IncreaseDistance")]
        public XmlKeyStroke GUIIncreaseDistance = new XmlKeyStroke("Shift + KeypadMinus", KeyMode.Press);
        [XmlElement("GUI.DecreaseDistance")]
        public XmlKeyStroke GUIDecreaseDistance = new XmlKeyStroke("Shift + KeypadPlus", KeyMode.Press);
        [XmlElement("GUI.RotateRight")]
        public XmlKeyStroke GUIRotateRight = new XmlKeyStroke("Ctrl + Shift + KeypadMinus", KeyMode.Press);
        [XmlElement("GUI.RotateLeft")]
        public XmlKeyStroke GUIRotateLeft = new XmlKeyStroke("Ctrl + Shift + KeypadPlus", KeyMode.Press);
        [XmlElement("GUI.ChangeProjection")]
        public XmlKeyStroke GUIChangeProjection = new XmlKeyStroke("F4");
    }

    public class XmlKeyStroke
    {
        [XmlAttribute("on")]
        public KeyMode CheckMode { get; private set; }

        [XmlText]
        public string Keys { get; private set; }

        public XmlKeyStroke()
        {
        }

        public XmlKeyStroke(string strokeString, KeyMode mode = KeyMode.PressUp)
        {
            CheckMode = mode;
            Keys = strokeString;
        }

        public KeyStroke[] GetKeyStrokes()
        {
            return Keys.Split(',', '|').Select(part => new KeyStroke(part.Trim())).ToArray();
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlCommentAttribute : Attribute
    {
        public XmlCommentAttribute(string value)
        {
            Value = value;
        }
        public string Value { get; set; }
    }
}
