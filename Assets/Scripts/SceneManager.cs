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


    // Start is called before the first frame update
    void Start()
    {
        avatarManager = API.AvatarManager;
        //Debug.Log("avatarManager1:" + avatarManager);
        currentAdditiveSceneName ="Sequence1Zone1";
        //ScM.SceneManager.LoadSceneAsync("Sequence1Zone1", LoadSceneMode.Additive);
        //ScM.SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        inputManager = API.InputManager;
        loadStartLocations();
        if(playerStartLocation!=null)
            initializeStartLocations();
        
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
            }
        }
    }

    private void HandleOnCollisionWithPortal(string sceneNameToTransitionTo)
    {
        ScM.SceneManager.LoadSceneAsync(sceneNameToTransitionTo, LoadSceneMode.Additive);
        ScM.SceneManager.UnloadSceneAsync(currentAdditiveSceneName);
        currentAdditiveSceneName = sceneNameToTransitionTo;
        reloadDone = false;
        
    }
}
