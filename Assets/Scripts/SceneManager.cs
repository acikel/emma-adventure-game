using System.Collections;
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

    // Start is called before the first frame update
    void Start()
    {
        faderCanvasGroup = API.FadeImage;
        faderCanvasGroup.alpha = 1f;
        avatarManager = API.AvatarManager;
        //Debug.Log("avatarManager1:" + avatarManager);
        currentAdditiveSceneName ="Sequence1Zone1";
        //ScM.SceneManager.LoadSceneAsync("Sequence1Zone1", LoadSceneMode.Additive);
        //ScM.SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        inputManager = API.InputManager;
        loadStartLocations();
        if(playerStartLocation!=null)
            initializeStartLocations();
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
                reloadDone = true;
                avatarManager.ReloadGround();
                loadStartLocations();
                if (playerStartLocation != null)
                    initializeStartLocations();
                StartCoroutine(Fade(0f));
            }
        }
    }

    private IEnumerator HandleOnCollisionWithPortal(string sceneNameToTransitionTo)
    {
        yield return StartCoroutine(Fade(1f));
        ScM.SceneManager.LoadSceneAsync(sceneNameToTransitionTo, LoadSceneMode.Additive);
        ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);
        currentAdditiveSceneName = sceneNameToTransitionTo;
        reloadDone = false;
    }


    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;
        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);
            yield return null;
        }
        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
