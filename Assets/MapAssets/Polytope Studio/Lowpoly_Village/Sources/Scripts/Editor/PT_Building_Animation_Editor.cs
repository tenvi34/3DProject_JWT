using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Polytope
{
    [CustomEditor(typeof(PT_Building_Animation)), CanEditMultipleObjects]
    public class PT_Building_Animation_Editor : Editor
    {
        SerializedProperty s_defaultOpacity;
        SerializedProperty s_glassOpacity;
        SerializedProperty s_timePerElement;
        SerializedProperty s_timePerGroup;
        SerializedProperty s_startupDelay;
        SerializedProperty s_groups;
        SerializedProperty s_snapPoints;

        private void OnEnable()
        {
            s_defaultOpacity = serializedObject.FindProperty("defaultOpacity");
            s_glassOpacity = serializedObject.FindProperty("glassOpacity");
            s_timePerElement = serializedObject.FindProperty("timePerElement");
            s_timePerGroup = serializedObject.FindProperty("timePerGroup");
            s_startupDelay = serializedObject.FindProperty("startupDelay");
            s_groups = serializedObject.FindProperty("groups");
            s_snapPoints = serializedObject.FindProperty("snapPoints");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(s_defaultOpacity);
            EditorGUILayout.PropertyField(s_glassOpacity);
            EditorGUILayout.PropertyField(s_timePerElement);
            EditorGUILayout.PropertyField(s_timePerGroup);
            EditorGUILayout.PropertyField(s_startupDelay);
            EditorGUILayout.PropertyField(s_groups);
            EditorGUILayout.PropertyField(s_snapPoints);

            if (Application.isPlaying)
            {
                if (GUILayout.Button($"Play"))
                {
                    (target as PT_Building_Animation).StartAnimation();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}