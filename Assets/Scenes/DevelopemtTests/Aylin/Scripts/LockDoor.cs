using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockDoor : OpenPopUpWindow
{

    //Door which needs to be set active when code was solved.
    //This LockDoor is set inactive when the Lock was solved as this is the closed door with a lock.
    //The behaviour when the Lock code was solved is written in lockResolved() of this class.
    public GameObject doorOpen;
    //checks if player is in trigger zones of lock which is put on close door.
    private bool playerCollided;
    private LockResumePanel lockResumePanel;


    private void OnEnable()
    {
        LockPanel.OnLockSolved += lockResolved;
    }
    private void OnDisable()
    {
        LockPanel.OnLockSolved -= lockResolved;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //Debug.Log("Lock Door player collided enter");
            playerCollided = true;
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
            lockResumePanel.openLockCanvas();
        }
        return playerCollided;
    }
    private void lockResolved()
    {
        doorOpen.SetActive(true);
        gameObject.SetActive(false);
    }

}
