using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Scripts.Variables
{
    [CreateAssetMenu]
    public class IntVariable : Variable<int>
    {
        
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(FloatVariable))]
    public class IntVariableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            IntVariable myScript = (IntVariable) target;
            if (GUILayout.Button("Invoke"))
            {
                myScript.ValueChangedEvent.Invoke();
            }
        }
    }
    
    #endif
}