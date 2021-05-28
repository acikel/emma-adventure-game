using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLockDoor : OpenPopUpWindow
{

    //Door which needs to be set active when code was solved.
    //This LockDoor is set inactive when the Lock was solved as this is the closed door with a lock.
    //The behaviour when the Lock code was solved is written in lockResolved() of this class.
    public GameObject doorOpen;
    //checks if player is in trigger zones of lock which is put on close door.
    private bool playerCollided;
    private LockResumePanel lockResumePanel;


    private new void OnEnable()
    {
        base.OnEnable();
        LockPanel.OnLockSolved += lockResolved;
    }
    private new void OnDisable()
    {
        base.OnDisable();
        LockPanel.OnLockSolved -= lockResolved;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //Debug.Log("Lock Door player collided enter");
            playerCollided = true;

            if (mouseWasClickedOnObject)
            {
                lockResumePanel.openCanvas();
                //imagePopUpResumePanel.justOpened(); //no need for resume onPointerDown blocker as player first needs to walk to this game object and mouse is aleady released till then.
                inventory.InteractionWithUIActive = true;
                setAlphaOfHintImage(0);
                FMODUnity.RuntimeManager.PlayOneShot(popUpSound);
                //resetMouseClick needed otherwise after closing popupwindow it is resumed after each reinter into trigger.
                resetMouseClick();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //Debug.Log("Lock Door player collided exit");
            playerCollided = false;
        }
    }

    public override void initializeCanvasToOpen()
    {
        canvasToOpen = API.CanvasLock;
        lockResumePanel= API.LockResumePanel;
    }

    public override bool activatePopUpWindow()
    {
        if (playerCollided)
        {
            lockResumePanel.openCanvas();
            lockResumePanel.justOpened();
        }
        return playerCollided;
    }
    private void lockResolved()
    {
        doorOpen.SetActive(true);
        gameObject.SetActive(false);
    }

}
