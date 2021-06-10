using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    private Animator animator;
    private SceneManager sceneManager;
    private bool sceneWasLoaded;
    public string nextSceneName;
    // Start is called before the first frame update
    void Start()
    {
        sceneManager = API.SceneManager;
        animator = GetComponent<Animator>();
        sceneWasLoaded = false;
    }

    // Update is called once per frame
    void Update()
    {
        //check if animator is animating any animation and if no animation is played go into next scene
        if (animator!=null && !sceneWasLoaded && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {  //If normalizedTime is 0 to 1 means animation is playing, if greater than 1 means finished
            //Debug.Log("not playing");
            //sceneManager.loadNextSceneUnhideInventoryAndAvatars("Sequence1Zone1");
            sceneManager.loadNextSceneUnhideInventoryAndAvatars(nextSceneName);
            sceneWasLoaded = true;
        }
        //else
        //{
            //Debug.Log("playing");
        //}
    }
}
