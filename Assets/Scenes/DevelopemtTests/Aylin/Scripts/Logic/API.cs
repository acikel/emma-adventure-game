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
    private static TextManager _textManagerInstance;
    private static LockResumePanel _lockResumePanel;
    private static CanvasGroup _canvasLockInstance;
    private static ImagePopUpResumePanel _imagePopUpResumePanel;
    private static CanvasGroup _canvasImagePopUpInstance;
    private static ImagePopUpPanel _ImagePopUpPanelInstance;

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

    public static TextManager TextManager
    {
        get
        {
            if (_textManagerInstance == null ||
            ReferenceEquals(_textManagerInstance, null))
            {
                _textManagerInstance = FindSingleInstance<TextManager>();
            }
            return _textManagerInstance;
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
                _fadeImageInstance = GameObject.FindGameObjectWithTag("FadeImage").GetComponent< CanvasGroup>();
            }
            return _fadeImageInstance;
        }
    }

    public static LockResumePanel LockResumePanel
    {
        get
        {
            if (_lockResumePanel == null ||
            ReferenceEquals(_lockResumePanel, null))
            {
                _lockResumePanel = FindSingleInstance<LockResumePanel>(); 
            }
            return _lockResumePanel;
        }
    }

    public static ImagePopUpPanel ImagePopUpPanel
    {
        get
        {
            if (_ImagePopUpPanelInstance == null ||
            ReferenceEquals(_ImagePopUpPanelInstance, null))
            {
                _ImagePopUpPanelInstance = FindSingleInstance<ImagePopUpPanel>();
            }
            return _ImagePopUpPanelInstance;
        }
    }
    


    public static ImagePopUpResumePanel ImagePopUpResumePanel
    {
        get
        {
            if (_imagePopUpResumePanel == null ||
            ReferenceEquals(_imagePopUpResumePanel, null))
            {
                _imagePopUpResumePanel = FindSingleInstance<ImagePopUpResumePanel>();
            }
            return _imagePopUpResumePanel;
        }
    }

    public static CanvasGroup CanvasLock
    {
        get
        {
            if (_canvasLockInstance == null ||
            ReferenceEquals(_canvasLockInstance, null))
            {
                _canvasLockInstance = GameObject.FindGameObjectWithTag("CanvasLock").GetComponent<CanvasGroup>();
            }
            return _canvasLockInstance;
        }
    }

    public static CanvasGroup CanvasImagePopUp
    {
        get
        {
            if (_canvasImagePopUpInstance == null ||
            ReferenceEquals(_canvasImagePopUpInstance, null))
            {
                _canvasImagePopUpInstance = GameObject.FindGameObjectWithTag("CanvasImagePopUp").GetComponent<CanvasGroup>();
            }
            return _canvasImagePopUpInstance;
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