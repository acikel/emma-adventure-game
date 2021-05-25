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

    //The inventory Canvas needs to be hidden by its Canvas Group when the dialog system is opened.
    private CanvasGroup canvasInventory;
    private string currentDialogSystemName;

    private Inventory inventory;
    private bool isReloading;
    private bool dialogUnloading;

    //need to be actions to call in IEnumerator method HandleOnCollisionWithPortal below.
    //subscribed by Saver and therefore GameObjectActivitySaver as Saver is the parent of GameObjectActivitySaver.
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;
    //subscribed by player controller to rescale player after avatar manager and player was reinitialized
    public delegate void HandleAfterAvatarInitialization(float avatarStartScale, float avatarScaleFactor);
    public event HandleAfterAvatarInitialization AfterAvatarInitialization;

    public List<AvatarScaleValues> sceneScales = new List<AvatarScaleValues>();
    private AvatarScaleValues currentSceneValues;

    private bool tmpPlayerFlip;
    private bool tmpHelperFlip;

    //keeps count of current sequence/level of the game (as after each dialog a new sequence/level is entered).
    private int currentSequenceNummber;

    public int CurrentSequenceNummber
    {
        get
        {
            return currentSequenceNummber;
        }
        set
        {
            currentSequenceNummber = value;
        }
    }

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
        currentSequenceNummber = 1;
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
        canvasInventory = API.CanvasInventory;
        

        loadStartLocations();
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

    
    private void OnEnable()
    {
        dialogUnloading = false;
        PlayerControler.OnCollisionWithPortal += HandleNextSceneLoad;
    }

    private void OnDisable()
    {
        PlayerControler.OnCollisionWithPortal -= HandleNextSceneLoad;
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


    public void loadDialogSystemLockPlayer(string dialogSystemName)
    {
        //Debug.Log("SceneManager loadDialogSystemLockPlayer");
        currentDialogSystemName = dialogSystemName;
        openCloseInventroyCanvas(false);
        StartCoroutine(loadDialogSystem(dialogSystemName));

    }

    public void unloadDialogSystemLoadNewSequenceUnlockPlayer(string newSceneNameToTransitionTo)
    {
        dialogUnloading = true;
        StartCoroutine(unloadDialogSystemLoadNewSequence(newSceneNameToTransitionTo));

    }

    private IEnumerator HandleNextSceneLoad(string sceneNameToTransitionTo)
    {
        isReloading = true;
        yield return StartCoroutine(Fade(1f));

        if (BeforeSceneUnload != null)
            BeforeSceneUnload();

        //Debug.Log(currentAdditiveSceneName);
        ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);

        if (Application.CanStreamedLevelBeLoaded(sceneNameToTransitionTo))
        {
            yield return ScM.SceneManager.LoadSceneAsync(sceneNameToTransitionTo, LoadSceneMode.Additive);
        }

        if (AfterSceneLoad != null)
            AfterSceneLoad();

        currentAdditiveSceneName = sceneNameToTransitionTo;
        assignScaleValueForCurrentScene();
        //Debug.Log("current scene name:"+ currentAdditiveSceneName);
        reloadDone = false;

        
    }


    private void loadStartLocations()
    {
        playerStartLocation = GameObject.Find("PlayerStartLocation");
        helperStartLocation = GameObject.Find("HelperStartLocation");
    }

    private void loadStartLocationsDialog()
    {
        playerStartLocation = GameObject.Find("DialogPlayerStartLocation");
        helperStartLocation = GameObject.Find("DialogHelperStartLocation");
    }

    private void initializeStartLocations()
    {
        if (playerStartLocation != null)
            AvatarManager.playerAvatar.gameObject.transform.position = playerStartLocation.transform.position;
        if (helperStartLocation != null)
            AvatarManager.helperAvatar.gameObject.transform.position = helperStartLocation.transform.position;
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
        if(finalAlpha!=1)//only when fading in
        {
            yield return StartCoroutine(Wait(0.001f));
            inventory.InteractionWithUIActive = false;
            isFading = false;
            if (dialogUnloading)
            {
                openCloseInventroyCanvas(true);
                dialogUnloading = false;
            }
            faderCanvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }


    private IEnumerator loadDialogSystem(string dialogSystemName)
    {
        yield return StartCoroutine(Fade(1f));
        yield return ScM.SceneManager.LoadSceneAsync(dialogSystemName, LoadSceneMode.Additive);
        loadStartLocationsDialog();
        initializeStartLocations();
        tmpPlayerFlip = AvatarManager.playerAvatar.avatarSpriteRenderer.flipX;
        tmpHelperFlip = AvatarManager.helperAvatar.avatarSpriteRenderer.flipX;
        AvatarManager.playerAvatar.avatarSpriteRenderer.flipX= true;
        AvatarManager.helperAvatar.avatarSpriteRenderer.flipX = true;
        if (currentSceneValues != null)
            AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);
        //assignScaleValueForCurrentScene();
        yield return StartCoroutine(Fade(0f));
        inventory.InteractionWithUIActive = true; //needs to be done in Grandma.cs script too as otherwise the player still moves when starting grandma script. And here again as Fadeing with an alpha other then 1 sets inventory.InteractionWithUIActive to false as in all other scene load the player is able to move after a scene load, except for the dialog system.
    }

    private IEnumerator unloadDialogSystemLoadNewSequence(string newSceneNameToTransitionTo)
    {
        //Debug.Log("newSceneNameToTransitionTo: "+newSceneNameToTransitionTo);
        yield return StartCoroutine(HandleNextSceneLoad(newSceneNameToTransitionTo));
        yield return ScM.SceneManager.UnloadSceneAsync(currentDialogSystemName);

        AvatarManager.playerAvatar.avatarSpriteRenderer.flipX = tmpPlayerFlip;
        AvatarManager.helperAvatar.avatarSpriteRenderer.flipX = tmpHelperFlip;
        //assignScaleValueForCurrentScene(); //This line is not needed as its already done in HandleNextSceneLoad;

        //inventory.InteractionWithUIActive = false; //This line is not needed as its already done in update after reloadDone is set to false with HandleNextSceneLoad.
    }

    private void openCloseInventroyCanvas(bool openInventory)
    {
        if (canvasInventory == null)
            return;
        canvasInventory.alpha = Convert.ToInt32(openInventory);
        canvasInventory.blocksRaycasts = openInventory;
        canvasInventory.interactable = openInventory;

    }
}
