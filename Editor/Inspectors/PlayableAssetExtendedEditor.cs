using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Celezt.Timeline.Editor
{
    [CustomEditor(typeof(PlayableAssetExtended), true)]
    public class PlayableAssetExtendedEditor : UnityEditor.Editor
    {
        public virtual void BuildInspector() { }

        public sealed override void OnInspectorGUI()
        {
            serializedObject.Update();

            BuildInspector();

            if (GetType() == typeof(PlayableAssetExtendedEditor))             // If not inherited.
                DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }

        protected object GetValue(object instance, string name) => GetValue<object>(instance, name);
        protected T GetValue<T>(object instance, string name)
        {
            return (T)instance.GetType()
                .GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .GetValue(instance);
        }
    }
}