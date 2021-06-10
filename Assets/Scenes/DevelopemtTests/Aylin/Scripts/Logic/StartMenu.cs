using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public Canvas startMenuCanvas;
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        startMenuCanvas.worldCamera = Camera.main;
        //Set CanvasDialog.planeDistance to zero as mouse events (onclick) wont work on buttons otherwise as colliders/raycastblockers are overlapped.
        startMenuCanvas.planeDistance = 0;
        sceneManager = API.SceneManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        //sceneManager.loadNextSceneUnhideInventoryAndAvatars("Sequence1Zone1");
        sceneManager.loadNextSceneHideInventoryAndAvatars("Sequence1Zone1Intro");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
