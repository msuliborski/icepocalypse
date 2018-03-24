using System;
using UnityEditor;
using UnityEngine;

namespace Scripts.Variables
{
    [CreateAssetMenu]
    public class FloatVariable : Variable<float>
    {
        
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(FloatVariable))]
    public class FloatVariableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            FloatVariable myScript = (FloatVariable) target;
            if (GUILayout.Button("Invoke"))
            {
                myScript.ValueChangedEvent.Invoke();
            }
        }
    }
    
    #endif
}