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

    //subscribed by PlayerController to rescale player when repositioned in lockResolved() method of this script.
    public delegate void OnPlayerRepositionHandler();
    public static event OnPlayerRepositionHandler OnPlayerReprosition;

    //needed as without it player will move to a random place as the player will get hit by the collider on activating the opened door doorOpen collider in lockResolved().
    //This happens because of the colliderStay2D code in PlayerController.cs, which moves the player to the previous position and pushes it back while colliding with the opened door collider.
    //This collision happens till the lock is closed and the player is set back very far away from the door.
    public Transform playerPositionOnDoorOpen;
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
                openCounter++;
                lockResumePanel.openCanvas();
                //imagePopUpResumePanel.justOpened(); //no need for resume onPointerDown blocker as player first needs to walk to this game object and mouse is aleady released till then.
                inventory.InteractionWithUIActive = true;
                setAlphaOfHintImage(0);
                FMODUnity.RuntimeManager.PlayOneShot(popUpSound);
                if (openCounter % skipsOfVoiceOver == 1)
                    FMODUnity.RuntimeManager.PlayOneShot(popUpSound2);
                //resetMouseClick needed otherwise after closing popupwindow it is resumed after each reinter into trigger.
                resetMouseClick();
                sceneManager.PopUpWindowIsOpen = true;//if the if was entered a popupwindow was opened scripts that reset player movement lock should check if an popupwindow is opened to not reset player movement lock if mouse enters their trigger like in InteractionSwitch.cs for interactables.
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
            openCounter++;
            sceneManager.PopUpWindowIsOpen = true;//if the if was entered a popupwindow was opened scripts that reset player movement lock should check if an popupwindow is opened to not reset player movement lock if mouse enters their trigger like in InteractionSwitch.cs for interactables.
        }
        return playerCollided;
    }
    private void lockResolved()
    {
        AvatarManager.playerAvatar.gameObject.transform.position = playerPositionOnDoorOpen.position;
        doorOpen.SetActive(true);
        gameObject.SetActive(false);

        OnPlayerReprosition?.Invoke();
        //inventory.InteractionWithUIActive = true;
    }

}
