using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : ActionObject
{
    
    public Animator animator;
    private SceneManager sceneManager;
    private string dialogSystemNameToLoad = "Sequence1DialogSystem";
    private string dreamSceneNameToLoad = "Sequence1Dream";
    private Inventory inventory;

    
    private void Start()
    {
        sceneManager = API.SceneManager;
        inventory = API.Inventory;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void actionOnDrop(string itemName)
    {
        //Debug.Log("actions drop name " + itemName);
        if (animator!=null && itemName.Contains("GrandmaHands"))
        {
            //Debug.Log("actions drop animation triggered");
            animator.SetTrigger("GrandmaRevived");
            StartCoroutine(waitAndLoadDialogSystem(1.5f));
        }
    }

    private IEnumerator waitAndLoadDialogSystem(float seconds)
    {
        //Debug.Log("waitAndLoadDialogSystem1");
        inventory.InteractionWithUIActive = true;
        yield return new WaitForSeconds(seconds);
        //Debug.Log("waitAndLoadDialogSystem2");
        sceneManager.loadDialogSystemLockPlayer(dialogSystemNameToLoad, dreamSceneNameToLoad);
        //inventory.clearInventorySlots(); //sadly buggy as audiolistener is not disableble
    }
}
