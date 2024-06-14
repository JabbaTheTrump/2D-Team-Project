using System;

public class ObservableVariable<T>
{
    private T _value;
    public T Value
    {
        get { return _value; }

        set { 
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public event Action<T> OnValueChanged;
}
