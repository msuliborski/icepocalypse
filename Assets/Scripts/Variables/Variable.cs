using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Variables
{
    public class Variable<T> : ScriptableObject
    {
        [SerializeField] private T _value;
        public UnityEvent ValueChangedEvent;

        public T Value
        {
            get
            {
                return _value; 
                
            }
            set
            {
                _value = value;
                ValueChangedEvent.Invoke();
            }
            
        }

    }
}