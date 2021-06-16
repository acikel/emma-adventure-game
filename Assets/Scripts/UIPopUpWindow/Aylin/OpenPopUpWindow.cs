using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OpenPopUpWindow : MonoBehaviour
{
    //Asign canvas to open when 2D collider of this game object was clicked.
    [HideInInspector]//only protected for subclass but not for editor.
    protected CanvasGroup canvasToOpen;
    protected Inventory inventory;
    private InputManager inputManager;
    protected SceneManager sceneManager;
    //Determines if this object was clicked
    private bool mouseWasClicked;
    //looks at each mouse click in game and is true if this obejct was clicked. 
    //To open the PopUpWindow right away when player arives without an extra click.
    //This is handled in the subclass OpenImagePopUp and OpenLockDoor when player enters a trigger.
    protected bool mouseWasClickedOnObject;

    //counts how many times the popupwindow was opened and plays the voice over every third time.
    protected int openCounter;
    //amount of clicks which are done to play the voice over for the popupwindow again
    public int skipsOfVoiceOver = 3;

    //variables for sounds reference.
    //protected FMOD.Studio.EventInstance popUpSoundEvent;
    //protected FMOD.Studio.EventDescription eventDescription;
    [FMODUnity.EventRef]
    public string popUpSound;

    [FMODUnity.EventRef]
    public string popUpSound2;

    public SpriteRenderer spriteRendererHintImage;
    private Color tmpColor;
    private float hintImageFadeInDuration = 1f;
    private float hintImageDisappearDuration = 0.3f;

    private bool resetPlayerMovement;

    // Start is called before the first frame update
    void Start()
    {
        resetPlayerMovement = false;
        inputManager = API.InputManager;
        inventory = API.Inventory;
        sceneManager = API.SceneManager;
        initializeCanvasToOpen();
        //spriteRendererHintImage = GetComponent<SpriteRenderer>();
        setAlphaOfHintImage(0);
        openCounter = 0;

        //eventDescription = FMODUnity.RuntimeManager.GetEventDescription(popUpSound);
        //if(eventDescription.isValid())
        //    eventDescription.createInstance(out popUpSoundEvent);
    }

    public void OnEnable()
    {
        ResumePanel.OnClosePopUpWinodwByResumePanel += unlockPlayerMovement;
    }

    public void OnDisable()
    {
        ResumePanel.OnClosePopUpWinodwByResumePanel -= unlockPlayerMovement;
    }

    private void unlockPlayerMovement()
    {
        resetPlayerMovement = true;

        //Debug.Log("unlockPlayerMovement1");
        //time based reset not optimal:
        //StartCoroutine(waitunlockPlayerMovement());
        //NOT TIME BASED optimal!:
        StartCoroutine(waitTillMouseWasReleasedAfterResumingFromPopUpPanel());
        
    }
    private IEnumerator waitTillMouseWasReleasedAfterResumingFromPopUpPanel()
    {
        while (resetPlayerMovement)
        {
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("unlockPlayerMovement2 inputManager.isMouseDown():" + inputManager.isMouseDown());
                resetPlayerMovement = false;
            }
            yield return null;
        }
        inventory.InteractionWithUIActive = false;
        //Debug.Log("waitTillMouseWasReleasedAfterResumingFromPopUpPanel");
        resetMouseClick();

        sceneManager.PopUpWindowIsOpen = false;//if this method was entered a popupwindow was closed scripts that reset player movement lock should check if an popupwindow is opened to not reset player movement lock if mouse enters their trigger like in InteractionSwitch.cs for interactables.
        //sceneManager.PopUpWindowIsOpen needs to be set to true in every subclass of OpenPopUpWindow.cs like OpenImagePopUp.cs and OpenLockDoor.cs, everytime the player opens the popupwindow while colliding (in the activatePopUpWindow() method) and in the OnTriggerEnter2D function which also opens the popupwindow when the popupwindow collider was clicked and the player entered later (by walking to the collider first).

    }


    //inventory.InteractionWithUIActive needs to be reset if player was on this game object when entering the pop up image
    //and exited this game object with the mouse while the pop up image was open this way the mouse exit wasnt entered
    // and inventory.InteractionWithUIActive wasnt reset to false.
    private IEnumerator waitunlockPlayerMovement()
    {
        //need to wait for a short amout of time before unlocking player movement as the mouse is used to return from
        //the popup window. When playermovement is unlocked immidiatly then closing the popupwindow already triggers
        //player movement which is not desired.
        yield return new WaitForSeconds(0.3f);
        inventory.InteractionWithUIActive = false;
        //Debug.Log("waitunlockPlayerMovement2");
        resetMouseClick();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.isMouseDown())
        {
            if (resetPlayerMovement && canvasToOpen.alpha == 0)//when canvas was closed
            {
                //resetPlayerMovement = false;
                //inventory.InteractionWithUIActive = false;
            }

            if (mouseWasClicked)
            {
                mouseWasClickedOnObject = true;
                //Debug.Log("mouseWasClickedOnObject1 "+name);
            }
            else
            {
                //Debug.Log("mouseWasClickedOnObject2" + name);
                mouseWasClickedOnObject = false;
            } 
        }
            
    }

    //this is the method of a subclass (like LockDoor and ImagePopUp) which needs to initialize canvasToOpen via API class 
    //as popup canvases (CanvasLock and CanvasImagePopUp) are located in the base 
    //scene and therefore not asignable through editor window as they are in different scenes.
    public abstract void initializeCanvasToOpen();

    //method which returns bool when canvas was opened to lock playerMovement.
    //inventory.InteractionWithUIActive need to set to false when PopUpWindow is closed again.
    //This needs to be done in script of PopUpWindow itself (like in LockResumePanel Or PopUpImage).
    //This script also only work if this gameobject is at the top of all colliders in the scene 
    //(order in layer can be low for right rendering/visualization of its sprite BUT the z position (ex.: z=100) of this 
    //game object needs to be at the top of all game objects in the scene so no collider/trigger is inbetween this 
    //object and the mouse, otherwise the OnMouse events of this script wont be triggered when entering the trigger/collider).
    public abstract bool activatePopUpWindow();

    private void OnMouseOver()
    {
        if (spriteRendererHintImage != null && canvasToOpen.alpha == 0)
        {
            //inventory.InteractionWithUIActive = true;
            StartCoroutine(Fade(1, spriteRendererHintImage));
        }
            

    }

    public void OnMouseExit()
    {
        //Debug.Log("OnMouseExit1");
        if (canvasToOpen != null && canvasToOpen.alpha==0 && !resetPlayerMovement)
        {
            StartCoroutine(waitAndHideHintImage());
            //Debug.Log("OnMouseExit1b");
            inventory.InteractionWithUIActive = false;
            mouseWasClicked = false;
        }


        //resetMouseClick();
    }

    public void OnMouseDown()
    {
        mouseWasClicked = true;
        //Debug.Log("OnMouseDown1 canvasToOpen.alpha: "+ canvasToOpen.alpha);
        if (canvasToOpen.alpha == 0 && activatePopUpWindow())
        {
            inventory.InteractionWithUIActive = true;
            setAlphaOfHintImage(0);
            //if (eventDescription.isValid())
            //    popUpSoundEvent.start();

            FMODUnity.RuntimeManager.PlayOneShot(popUpSound);
            if(openCounter% skipsOfVoiceOver == 1)//play mouseover sound every 3. time the popupWindow was opened
                FMODUnity.RuntimeManager.PlayOneShot(popUpSound2);
            //Debug.Log("OnMouseDown2");
            //resetMouseClick();
        }

    }
    public void resetMouseClick()
    {
        mouseWasClickedOnObject = false;
        mouseWasClicked = false;
    }

    private IEnumerator waitAndHideHintImage()
    {
        if (spriteRendererHintImage == null || canvasToOpen.alpha == 1)
            yield break;

        yield return new WaitForSeconds(hintImageDisappearDuration);
        setAlphaOfHintImage(0);
    }

    protected void setAlphaOfHintImage(float alpha)
    {
        if (spriteRendererHintImage == null)
            return;

        tmpColor = spriteRendererHintImage.color;
        tmpColor.a = alpha;
        spriteRendererHintImage.color = tmpColor;
    }

    private IEnumerator Fade(float finalAlpha, SpriteRenderer spriteRenderer)
    {

        float fadeSpeed = Mathf.Abs(spriteRenderer.color.a - finalAlpha) / hintImageFadeInDuration;
        while (!Mathf.Approximately(spriteRenderer.color.a, finalAlpha))
        {
            setAlphaOfHintImage(Mathf.MoveTowards(spriteRenderer.color.a, finalAlpha,
                fadeSpeed * Time.deltaTime));
            yield return null;
        }
    }
}
