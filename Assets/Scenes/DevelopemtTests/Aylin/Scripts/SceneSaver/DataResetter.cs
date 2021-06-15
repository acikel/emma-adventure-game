using UnityEngine;

// This script is used to reset scriptable objects
// back to their default values.  This is useful
// in the editor when serialized data can persist
// between entering and exiting play mode.  It is
// also useful for situations where the game needs
// to reset without being closed, for example a new
// play through.
public class DataResetter : MonoBehaviour
{
	public ResettableScriptableObject[] resettableScriptableObjects;    // All of the scriptable object assets that should be reset at the start of the game.
	private SceneManager sceneManager;

	private void Awake()
	{
		sceneManager = API.SceneManager;
		// Go through all the scriptable objects and call their Reset function.
		resetObjects();

	}
    private void OnEnable()
    {
		sceneManager.OnStartMenuEntered += resetObjects;
    }

    private void OnDisable()
    {
		sceneManager.OnStartMenuEntered -= resetObjects;
	}
    private void resetObjects()
    {
		for (int i = 0; i < resettableScriptableObjects.Length; i++)
		{
			resettableScriptableObjects[i].Reset();
		}
	}
}
