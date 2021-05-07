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
    private static SceneManager _sceneManagerInstance;
    private static AvatarManager _avatarManagerInstance;
    private static CanvasGroup _fadeImageInstance;
    private static Inventory _inventoryInstance;
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

    public static SceneManager SceneManager
    {
        get
        {
            if (_sceneManagerInstance == null ||
            ReferenceEquals(_sceneManagerInstance, null))
            {
                _sceneManagerInstance = FindSingleInstance<SceneManager>();
            }
            return _sceneManagerInstance;
        }
    }

    public static AvatarManager AvatarManager
    {
        get
        {
            if (_avatarManagerInstance == null ||
            ReferenceEquals(_avatarManagerInstance, null))
            {
                //Debug.Log("was1");
                _avatarManagerInstance = AvatarManager.Instance;
            }
            return _avatarManagerInstance;
        }
    }

    public static CanvasGroup FadeImage
    {
        get
        {
            if (_fadeImageInstance == null ||
            ReferenceEquals(_fadeImageInstance, null))
            {
                _fadeImageInstance = FindSingleInstance<CanvasGroup>();
            }
            return _fadeImageInstance;
        }
    }

    public static Inventory Inventory
    {
        get
        {
            if (_inventoryInstance == null ||
            ReferenceEquals(_inventoryInstance, null))
            {
                _inventoryInstance = FindSingleInstance<Inventory>();
            }
            return _inventoryInstance;
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