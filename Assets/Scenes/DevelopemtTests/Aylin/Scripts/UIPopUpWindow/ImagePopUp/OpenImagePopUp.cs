using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenImagePopUp : OpenPopUpWindow
{

    //checks if player is in trigger zones of lock which is put on close door.
    private bool playerCollided;
    private ImagePopUpResumePanel imagePopUpResumePanel;
    private ImagePopUpPanel imagePopUpPanel;
    


    //image to be displayed by ImagePopUpWindow when this game object was clicked.
    public Sprite imageToBeDisplayed;

    private new void OnEnable()
    {
        base.OnEnable();
        imagePopUpPanel = API.ImagePopUpPanel;
        
    }

    private new void OnDisable()
    {
        base.OnDisable();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //Debug.Log("ImagePopUp Door player collided enter");
            playerCollided = true;

            if (mouseWasClickedOnObject)
            {
                imagePopUpResumePanel.openCanvas();
                //imagePopUpResumePanel.justOpened(); //no need for resume onPointerDown blocker as player first needs to walk to this game object and mouse is aleady released till then.
                imagePopUpPanel.setImageOfPopUpImagePanel(imageToBeDisplayed);
                setAlphaOfHintImage(0);
                inventory.InteractionWithUIActive = true;
                //resetMouseClick needed otherwise after closing popupwindow it is resumed after each reinter into trigger.
                resetMouseClick();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            //Debug.Log("ImagePopUp Door player collided exit");
            playerCollided = false;
            resetMouseClick();
        }
    }

    public override bool activatePopUpWindow()
    {
        //Debug.Log("ImagePopUp Door activatePopUpWindow1 playerCollided: "+ playerCollided);
        if (playerCollided)
        {
            inventory.InteractionWithUIActive = true;
            //Debug.Log("ImagePopUp Door activatePopUpWindow2");
            imagePopUpResumePanel.openCanvas();
            imagePopUpResumePanel.justOpened();
            imagePopUpPanel.setImageOfPopUpImagePanel(imageToBeDisplayed);
        }
        return playerCollided;
    }

    public override void initializeCanvasToOpen()
    {
        canvasToOpen = API.CanvasImagePopUp;
        imagePopUpResumePanel = API.ImagePopUpResumePanel;
    }

   
}