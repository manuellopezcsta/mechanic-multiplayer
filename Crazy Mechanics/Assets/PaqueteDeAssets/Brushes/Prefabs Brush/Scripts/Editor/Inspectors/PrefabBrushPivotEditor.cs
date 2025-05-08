using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Harpia.PrefabBrush
{
    [CustomEditor(typeof(PrefabBrushPivot)), CanEditMultipleObjects]
    public class PrefabBrushPivotEditor : Editor
    {
        private bool _showAbout1;

        public override void OnInspectorGUI()
        {
            var script = (PrefabBrushPivot) target;
            var serializedObj = new UnityEditor.SerializedObject(script);
            
            _showAbout1 = EditorGUILayout.Foldout(_showAbout1, "About");
            if (_showAbout1)
            {
                EditorGUILayout.LabelField("Can be used as pivot for the Prefab Brush.");
                EditorGUILayout.LabelField("Tip: Use the V key shortcut to quickly align the pivot with object mesh vertices.");
                EditorGUILayout.LabelField("Not packed to the build");
            }
            
            //Draw the priority
            EditorGUILayout.Space();
            
            //Draw the attrackted to array
            var attractedBy = serializedObj.FindProperty("attractedBy");
            EditorGUILayout.PropertyField(attractedBy);
            
            serializedObj.ApplyModifiedProperties();
        }
    }
}
