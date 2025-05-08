using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Harpia.PrefabBrush
{
    [CustomEditor(typeof(PrefabBrushAttractor)), CanEditMultipleObjects]
    public class PrefabBrushAttractorEditor : Editor
    {
        private bool _showAbout2;

        public override void OnInspectorGUI()
        {
            var script = (PrefabBrushAttractor) target;
            var serializedObj = new UnityEditor.SerializedObject(script);
            
            _showAbout2 = EditorGUILayout.Foldout(_showAbout2, "About");
            if (_showAbout2)
            {
                EditorGUILayout.LabelField(@"This will attract the Prefab Brush pivot that contains the key in the attractedBy list.");
                EditorGUILayout.LabelField("Tip: Use the V key shortcut to quickly align the pivot with object mesh vertices.");
                EditorGUILayout.LabelField("Not packed to the build");
            }
            
            
            //Draw the attractor name
            var attractorName = serializedObj.FindProperty("attractorName");
            EditorGUILayout.PropertyField(attractorName);
            
            //Draw the attractor range
            var attractorRange = serializedObj.FindProperty("attractorRadius");
            EditorGUILayout.PropertyField(attractorRange);
            
            
            serializedObj.ApplyModifiedProperties();
            
        }
    }
}