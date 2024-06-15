using System;
using UnityEngine;

[System.Serializable]
public class ObservableVariable<T>
{
    [SerializeField]
    private T _value;
    [HideInInspector] public T Value
    {
        get { return _value; }

        set {
            if (!_value.Equals(value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    public ObservableVariable(T initialValue)
    {
        _value = initialValue;
    }

    public event Action<T> OnValueChanged;
}
