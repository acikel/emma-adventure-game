using UnityEngine;

// This is an abstract MonoBehaviour that is the base class
// for all classes that want to save data to persist between
// scene loads and unloads.
// For an example of using this class, see the PositionSaver
// script.
public abstract class Saver : MonoBehaviour
{
    public string uniqueIdentifier;             // A unique string set by a scene designer to identify what is being saved.
    public SaveData saveData;                   // Reference to the SaveData scriptable object where the data is stored.


    protected string key;                       // A string to identify what is being saved.  This should be set using information about the data as well as the uniqueIdentifier.


    private SceneManager sceneManager;    // Reference to the SceneController so that this can subscribe to events that happen before and after scene loads.


    private void Awake()
    {
        // Find the SceneController and store a reference to it.
        sceneManager = API.SceneManager;

        // If the SceneController couldn't be found throw an exception so it can be added.
        if (!sceneManager)
            throw new UnityException("Scene Controller could not be found, ensure that it exists in the Persistent scene.");

        // Set the key based on information in inheriting classes.
        key = SetKey();
    }


    private void OnEnable()
    {
        // Subscribe the Save function to the BeforeSceneUnload event.
        sceneManager.BeforeSceneUnload += Save;

        // Subscribe the Load function to the AfterSceneLoad event.
        sceneManager.AfterSceneLoad += Load;
    }


    private void OnDisable()
    {
        // Unsubscribe the Save function from the BeforeSceneUnloud event.
        sceneManager.BeforeSceneUnload -= Save;

        // Unsubscribe the Load function from the AfterSceneLoad event.
        sceneManager.AfterSceneLoad -= Load;
    }


    // This function will be called in awake and must return the intended key.
    // The key must be totally unique across all Saver scripts.
    protected abstract string SetKey();


    // This function will be called just before a scene is unloaded.
    // It must call saveData.Save and pass in the key and the relevant data.
    protected abstract void Save();


    // This function will be called just after a scene is finished loading.
    // It must call saveData.Load with a ref parameter to get the data out.
    protected abstract void Load();
}
