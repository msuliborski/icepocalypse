using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Scripts.Variables
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        private readonly List<GameEventListener> _eventListeners = new List<GameEventListener>();

        public void RegisterListener(GameEventListener listener)
        {
            _eventListeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            _eventListeners.Remove(listener);
        }
        
        public void Raise()
        {
            Debug.Log("ty kurwo");
            for (var i = _eventListeners.Count - 1; i >= 0; i--)
            {
                _eventListeners[i].OnEventRaised();
            }
            
        }
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(GameEvent))]
        public class GameEventEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                var myScript = (GameEvent) target;
                if (GUILayout.Button("Raise"))
                {
                     myScript.Raise();   
                }
                
            }
        }
        
        
        #endif
    }
}