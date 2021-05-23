using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ScM=UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private InputManager inputManager;
    private string currentAdditiveSceneName;
    private AvatarManager avatarManager;
    private bool reloadDone = true;

    private GameObject playerStartLocation;
    private GameObject helperStartLocation;

    private bool isFading;
    private CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;

    private Inventory inventory;
    private bool isReloading;

    //need to be actions to call in IEnumerator method HandleOnCollisionWithPortal below.
    //subscribed by Saver and therefore GameObjectActivitySaver as Saver is the parent of GameObjectActivitySaver.
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;
    //subscribed by player controller to rescale player after avatar manager and player was reinitialized
    public delegate void HandleAfterAvatarInitialization(float avatarStartScale, float avatarScaleFactor);
    public event HandleAfterAvatarInitialization AfterAvatarInitialization;

    public List<AvatarScaleValues> sceneScales = new List<AvatarScaleValues>();
    private AvatarScaleValues currentSceneValues;

    public bool IsReloading
    {
        get
        {
            return isReloading;
        }
    }

    public bool IsFading
    {
        get
        {
            return isFading;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        faderCanvasGroup = API.FadeImage;
        faderCanvasGroup.alpha = 1f;
        avatarManager = API.AvatarManager;
        //Debug.Log("avatarManager1:" + avatarManager);
        currentAdditiveSceneName ="Sequence1Zone1";
        assignScaleValueForCurrentScene();
        //ScM.SceneManager.LoadSceneAsync("Sequence1Zone1", LoadSceneMode.Additive);
        //ScM.SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        inputManager = API.InputManager;
        inventory = API.Inventory;

        loadStartLocations();
        if(playerStartLocation!=null)
            initializeStartLocations();

        //PLayer scale and scale needs to be set on player for first scene as AfterAvatarInitialization is null on start.
        /*
        assignScaleValueForCurrentScene();

        if (currentSceneValues != null)
        {
            AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);
        }
        */
        StartCoroutine(Fade(0f));

    }

    
    private void loadStartLocations()
    {
        playerStartLocation = GameObject.Find("PlayerStartLocation");
        helperStartLocation = GameObject.Find("HelperStartLocation");
    }
    private void initializeStartLocations()
    {
        AvatarManager.playerAvatar.gameObject.transform.position = playerStartLocation.transform.position;
        AvatarManager.helperAvatar.gameObject.transform.position = helperStartLocation.transform.position;
    }
    private void OnEnable()
    {
        PlayerControler.OnCollisionWithPortal += HandleOnCollisionWithPortal;
    }

    private void OnDisable()
    {
        PlayerControler.OnCollisionWithPortal -= HandleOnCollisionWithPortal;
    }

    // Update is called once per frame
    void Update()
    {
        /* if (inputManager.isMouseDown())
         {
             if ((hitPortal= inputManager.getHitCollider("Portal"))!=null)
             {
                 ScM.SceneManager.LoadSceneAsync(hitPortal.gameObject.name, LoadSceneMode.Additive);
                 ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);
                 currentAdditiveSceneName = hitPortal.gameObject.name;
             }
         }*/

        if (!reloadDone)
        {
            if (ScM.SceneManager.GetSceneByName(currentAdditiveSceneName).isLoaded)
            {
                
                avatarManager.ReloadGround();
                loadStartLocations();
                if (playerStartLocation != null)
                    initializeStartLocations();
                if (currentSceneValues != null)
                    AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);

                StartCoroutine(Fade(0f));
                reloadDone = true;
                isReloading = false;
            }
        }
    }

    public AvatarScaleValues getCurrentSceneValues()
    {
        return currentSceneValues;
    }
    private IEnumerator HandleOnCollisionWithPortal(string sceneNameToTransitionTo)
    {
        isReloading = true;
        yield return StartCoroutine(Fade(1f));

        if (BeforeSceneUnload != null)
            BeforeSceneUnload();

        ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);
        yield return ScM.SceneManager.LoadSceneAsync(sceneNameToTransitionTo, LoadSceneMode.Additive);
        

        if (AfterSceneLoad != null)
            AfterSceneLoad();

        currentAdditiveSceneName = sceneNameToTransitionTo;
        assignScaleValueForCurrentScene();
        //Debug.Log("current scene name:"+ currentAdditiveSceneName);
        reloadDone = false;

        
    }

    private void assignScaleValueForCurrentScene()
    {
        foreach (AvatarScaleValues asv in sceneScales)
        {
            if (asv.sceneName.Equals(currentAdditiveSceneName))
            {
                currentSceneValues = asv;
                return;
            }
        }
        currentSceneValues = null;
    }

    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        //Debug.Log("inventory interaction1:" + inventory.InteractionWithInventoryActive);
        inventory.InteractionWithUIActive = true;
        //Debug.Log("inventory interaction2:" + inventory.InteractionWithInventoryActive);
        faderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null;
        }
        yield return StartCoroutine(Wait(0.001f));
        inventory.InteractionWithUIActive = false;
        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
