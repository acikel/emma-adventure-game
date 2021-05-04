using UnityEngine;
public static class API
{
    private static T FindSingleInstance<T>() where T : Object
    {
        if (Application.isEditor)
        {
            T[] result = GameObject.FindObjectsOfType(typeof(T)) as T[];
            if (result.Length == 0)
            {
                throw new System.Exception("API: can't find module " + typeof(T) +
                " in the scene!");
            }
            if (result.Length > 1)
            {
                throw new System.Exception("API: there is more than one " +
                typeof(T) + " in the scene!");
            }
            if (result[0] is T)
            {
                return result[0];
            }
            else
            {
                throw new System.Exception("API: there is a type mismatch with " +
                typeof(T) + "!");
            }
        }
        else
        {
            return GameObject.FindObjectOfType(typeof(T)) as T;
        }
    }
    private static InputManager _inputManagerInstance;
    public static InputManager InputManager
    {
        get
        {
            if (_inputManagerInstance == null ||
            ReferenceEquals(_inputManagerInstance, null))
            {
                _inputManagerInstance = FindSingleInstance<InputManager>();
            }
            return _inputManagerInstance;
        }
    }
    public static bool PrewarmReferences()
    {
        if (InputManager)
        {
            return true;
        }
        return false;
    }
}