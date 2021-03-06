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
    private FMODUnity.StudioListener audioListener;
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
    private bool activateInventory;
    private bool unHideAvatars;
    //tells if dialog system is loaded. So the player interaction only gets enabled after fade to alpha=0 (in the Fade method) if no dialog system is loaded.
    private bool dialogSystemLoaded;

    //need to be actions to call in IEnumerator method HandleOnCollisionWithPortal below.
    //subscribed by Saver and therefore GameObjectActivitySaver as Saver is the parent of GameObjectActivitySaver.
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;
    //subscribed by player controller to rescale player after avatar manager and player was reinitialized
    public delegate void HandleAfterAvatarInitialization(float avatarStartScale, float avatarScaleFactor);
    public event HandleAfterAvatarInitialization AfterAvatarInitialization;

    //subscribed by DataResetter.cs to reset all saved states of objects when game is done (when resetting start Menu)
    public delegate void HandleOnStartMenuEntered();
    public event HandleOnStartMenuEntered OnStartMenuEntered;


    public List<AvatarScaleValues> sceneScales = new List<AvatarScaleValues>();
    private AvatarScaleValues currentSceneValues;

    private float tmpPlayerFlip;
    private float tmpHelperFlip;

    //vairable um door wort von name der n?chster scene rauszuschneiden.
    private string substringTMp;
    private int indexTmp;

    private float flipAvatarLeft = 1f;
    private float flipAvatarRight = -1f;

    private bool startMenuWasEntered;

    public AsyncOperation async;

    //keeps count of current sequence/level of the game (as after each dialog a new sequence/level is entered).
    private int currentSequenceNummber;

    //used by OpenPopUpWindow.cs to define if any popup window is currently opened (for example Lock popupwindow or image popup window)
    //This is then used by objects which reset the player movement unlock like the InteractionSwitch.cs which resets the player movement unlock
    //whenever the mouse exits the collider of interactables. If it does not know when the popup window is open it will reset it when an interactable
    //object is overlayed by an opened popupwindow and the player starts moving even if this is not wanted.
    private bool popUpWindowIsOpen;
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

    public bool PopUpWindowIsOpen
    {
        get
        {
            return popUpWindowIsOpen;
        }
        set
        {
            popUpWindowIsOpen = value;
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

    public AvatarScaleValues CurrentSceneValues
    {
        get
        {
            return currentSceneValues;
        }
    }


    
        // Start is called before the first frame update
        void Start()
    {
        StartCoroutine(startGame());

    }

    
    private void OnEnable()
    {
        unHideAvatars = false;
        activateInventory = false;
        dialogSystemLoaded = false;
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
                //Debug.Log("Scene Manager in reloade Done");
                avatarManager.ReloadGround();
                avatarManager.ReloadObstacles();
                loadStartLocations();
                initializeStartLocations();
                if (currentSceneValues != null)
                {
                    //Debug.Log("Scene Manager in currentSceneValues currentSceneValues.avatarStartScale: "+ currentSceneValues.avatarStartScale + " currentSceneValues.avatarScaleFactor: "+ currentSceneValues.avatarScaleFactor);
                    AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);
                }

                StartCoroutine(Fade(0f));
                reloadDone = true;
                isReloading = false;

                if (startMenuWasEntered && currentAdditiveSceneName.Equals("Sequence1Zone1"))
                {
                    AvatarManager.helperAvatar.gameObject.SetActive(false);
                    //inventory.clearInventorySlots(); //sadly buggy as audiolistener is not disableble
                    startMenuWasEntered = false;
                }
                //disable audio listener on scene load (in HandleNextSceneLoad) and enable it after loading (here in update -> !reloadeDone).
                //audioListener.enabled = true; //sadly buggy as audiolistener is not disableble
            }
        }
    }

    public AvatarScaleValues getCurrentSceneValues()
    {
        return currentSceneValues;
    }


    public void loadDialogSystemLockPlayer(string dialogSystemName, string dreamSceneName)
    {
        //Debug.Log("SceneManager loadDialogSystemLockPlayer");
        dialogSystemLoaded = true;
        currentDialogSystemName = dialogSystemName;
        openInventroyCanvas(false);
        StartCoroutine(loadDialogSystem(dialogSystemName, dreamSceneName));

    }

    public void unloadDialogSystemLoadNewSequenceUnlockPlayer(string newSceneNameToTransitionTo)
    {
        dialogSystemLoaded = false;
        activateInventory = true;
        StartCoroutine(unloadDialogSystemLoadNewSequence(newSceneNameToTransitionTo));

    }

    public void loadNextSceneUnhideInventoryAndAvatarsLast(string sceneNameToTransitionTo)
    {
        activateInventory = true;
        unHideAvatars = true;
        StartCoroutine(HandleNextSceneLoad(sceneNameToTransitionTo));
    }

    public void loadNextSceneUnhideInventoryAndAvatarsFirst(string sceneNameToTransitionTo)
    {
        activateInventory = true;
        unHideAvatars = true;
        StartCoroutine(loadNextSceneUnHideInventoryAndAvatarsRoutineFirst(sceneNameToTransitionTo));
    }

    public void loadNextSceneHideInventoryAndAvatars(string sceneNameToTransitionTo) {
        StartCoroutine(loadNextSceneHideInventoryAndAvatarsRoutine(sceneNameToTransitionTo));
    }
    private IEnumerator loadNextSceneHideInventoryAndAvatarsRoutine(string sceneNameToTransitionTo)
    {
        openInventroyCanvas(false);
        avatarManager.hideAvatars(true);
        yield return StartCoroutine(HandleNextSceneLoad(sceneNameToTransitionTo));
    }

    private IEnumerator loadNextSceneUnHideInventoryAndAvatarsRoutineFirst(string sceneNameToTransitionTo)
    {
        
        yield return StartCoroutine(HandleNextSceneLoad(sceneNameToTransitionTo));
        openInventroyCanvas(false);
        avatarManager.hideAvatars(true);
    }

    private IEnumerator HandleNextSceneLoad(string sceneNameToTransitionTo)
    {
        //disable audio listener on scene load (here in HandleNextSceneLoad) and enable it after loading (in update -> !reloadeDone).
        //audioListener.enabled = false;//sadly buggy as audiolistener is not disableble

        isReloading = true;
        indexTmp = sceneNameToTransitionTo.IndexOf("_");

        if (sceneNameToTransitionTo.Contains("_"))
            substringTMp = sceneNameToTransitionTo.Substring(indexTmp + 1);
        else
            substringTMp = sceneNameToTransitionTo;

        if (substringTMp.Equals("Sequence1Zone5") || substringTMp.Equals("Sequence2Zone1"))
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Location", 1);
        else
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Location", 0);

        //Debug.Log("HandleNextSceneLoad SceneName: "+ substringTMp);
        yield return StartCoroutine(Fade(1f));

        if (BeforeSceneUnload != null)
            BeforeSceneUnload();

        //Debug.Log(currentAdditiveSceneName);
        ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);

        if (Application.CanStreamedLevelBeLoaded(substringTMp))
        {
            yield return ScM.SceneManager.LoadSceneAsync(substringTMp, LoadSceneMode.Additive);
        }

        if (AfterSceneLoad != null)
            AfterSceneLoad();

        currentAdditiveSceneName = substringTMp;
        hideAvatarsAndInventoryOnStartMenuLoad();
        assignScaleValueForCurrentScene();
        //Debug.Log("current scene name:"+ currentAdditiveSceneName);
        reloadDone = false;

        
    }

    private void hideAvatarsAndInventoryOnStartMenuLoad()
    {
        if (currentAdditiveSceneName.Equals("StartMenu"))
        {
            //Debug.Log("Start Menu Loaded");
            //hide avatars (player and helper) and inventory canvas when start menu is displayed.
            openInventroyCanvas(false);
            activateInventory = false;
            unHideAvatars = false;
            avatarManager.hideAvatars(true);
            startMenuWasEntered = true;
            popUpWindowIsOpen = false;
            OnStartMenuEntered?.Invoke();
        }
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
            if(!dialogSystemLoaded)
                inventory.InteractionWithUIActive = false;
            isFading = false;
            if (activateInventory)
            {
                //Debug.Log("activate Inventory");
                openInventroyCanvas(true);
                activateInventory = false;
            }
            if (unHideAvatars)
            {
                avatarManager.hideAvatars(false);
                unHideAvatars = false;
            }
            faderCanvasGroup.blocksRaycasts = false;
        }
    }

    private IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }


    private IEnumerator loadDialogSystem(string dialogSystemName, string dreamSceneName)
    {
        //loadStartLocationsDialog();
        //initializeStartLocations();
        tmpPlayerFlip = AvatarManager.playerAvatar.getFlipDirection();
        tmpHelperFlip = AvatarManager.helperAvatar.getFlipDirection();
        AvatarManager.playerAvatar.setFlipDirection(flipAvatarRight);
        AvatarManager.helperAvatar.setFlipDirection(flipAvatarRight);

        yield return StartCoroutine(HandleNextSceneLoad(dreamSceneName));
        yield return async = ScM.SceneManager.LoadSceneAsync(dialogSystemName, LoadSceneMode.Additive);
        

        //if (currentSceneValues != null)
        //    AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);
        //assignScaleValueForCurrentScene();
        //yield return StartCoroutine(Fade(0f));
        //inventory.InteractionWithUIActive = true; //needs to be done in Grandma.cs script too as otherwise the player still moves when starting grandma script. And here again as Fadeing with an alpha other then 1 sets inventory.InteractionWithUIActive to false as in all other scene load the player is able to move after a scene load, except for the dialog system.
    }

    private IEnumerator unloadDialogSystemLoadNewSequence(string newSceneNameToTransitionTo)
    {
        //Debug.Log("newSceneNameToTransitionTo: "+newSceneNameToTransitionTo);
        yield return StartCoroutine(HandleNextSceneLoad(newSceneNameToTransitionTo));
        yield return ScM.SceneManager.UnloadSceneAsync(currentDialogSystemName);

        AvatarManager.playerAvatar.setFlipDirection(tmpPlayerFlip);
        AvatarManager.helperAvatar.setFlipDirection(tmpHelperFlip);
        //assignScaleValueForCurrentScene(); //This line is not needed as its already done in HandleNextSceneLoad;

        //inventory.InteractionWithUIActive = false; //This line is not needed as its already done in update after reloadDone is set to false with HandleNextSceneLoad.
    }

    private void openInventroyCanvas(bool openInventory)
    {
        if (canvasInventory == null)
            return;

        //Debug.Log("openIventoryCanvas"+ Convert.ToInt32(openInventory));
        canvasInventory.alpha = Convert.ToInt32(openInventory);
        canvasInventory.blocksRaycasts = openInventory;
        canvasInventory.interactable = openInventory;

    }

    private void loadCameraAfterInitialization()
    {
        inputManager.reasignCamera();
    }

    private IEnumerator loadInitialSceneSetup()
    {
        //disable audio listener on scene load (here in HandleNextSceneLoad) and enable it after loading (in update -> !reloadeDone).
        //audioListener.enabled = false;//sadly buggy as audiolistener is not disableble
        Scene scene = ScM.SceneManager.GetActiveScene();
        if (scene == null)
            yield return ScM.SceneManager.LoadSceneAsync("Logic", LoadSceneMode.Additive);
        else if (scene.name.Equals("Logic"))
            ScM.SceneManager.SetActiveScene(ScM.SceneManager.GetSceneByName("Logic"));
        else
        {
            yield return ScM.SceneManager.LoadSceneAsync("Logic", LoadSceneMode.Additive);
            ScM.SceneManager.SetActiveScene(ScM.SceneManager.GetSceneByName("Logic"));
        }

        yield return ScM.SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        yield return ScM.SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Additive);
        currentAdditiveSceneName = ScM.SceneManager.GetSceneAt(ScM.SceneManager.sceneCount - 1).name;
        loadCameraAfterInitialization();
    }

    private IEnumerator startGame()
    {
        //ScM.SceneManager.LoadSceneAsync("Sequence1Zone1", LoadSceneMode.Additive);
        //ScM.SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        inputManager = API.InputManager;
        if(ScM.SceneManager.sceneCount==1)
            yield return StartCoroutine(loadInitialSceneSetup());
        currentSequenceNummber = 1;
        faderCanvasGroup = API.FadeImage;
        faderCanvasGroup.alpha = 1f;
        avatarManager = API.AvatarManager;
        //Debug.Log("avatarManager1:" + avatarManager);
        //currentAdditiveSceneName ="Sequence1Zone1";

        //3. additive scene at game start is set as first loaded scene and unloaded first.
        currentAdditiveSceneName = ScM.SceneManager.GetSceneAt(ScM.SceneManager.sceneCount - 1).name;
        //Debug.Log("currentAdditiveSceneName: " + currentAdditiveSceneName);

        inventory = API.Inventory;
        canvasInventory = API.CanvasInventory;


        assignScaleValueForCurrentScene();


        //hide avatars (player and helper) and inventory canvas when start menu is displayed on start:
        if (ScM.SceneManager.sceneCount == 1)
            hideAvatarsAndInventoryOnStartMenuLoad();
        loadStartLocations();
        initializeStartLocations();
        popUpWindowIsOpen = false;

        audioListener = API.FMODAudioListenerInstance;
        //PLayer scale and scale needs to be set on player for first scene as AfterAvatarInitialization is null on start.
        /*
        assignScaleValueForCurrentScene();

        if (currentSceneValues != null)
        {
            AfterAvatarInitialization?.Invoke(currentSceneValues.avatarStartScale, currentSceneValues.avatarScaleFactor);
        }
        */

        //AvatarManager.helperAvatar.gameObject.SetActive(false);
        StartCoroutine(Fade(0f));
    }
}
