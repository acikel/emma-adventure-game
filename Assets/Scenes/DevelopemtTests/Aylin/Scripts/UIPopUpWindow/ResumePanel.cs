using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//If this script is used a canvasgroup need to be attacked to the canvas which holds the ResumePanel and same for the canvas/gameobject which holds the inventory.
//Also the Panel itself has to have a canvas conponent which does not inherit the sorting order from its parents and is one less then the sorting order of canvasPopUpToClose.
//And a Graphic Raycaster component. Also the image which partly covers this resume panel needs to have an Graphic Raycaster component otherwise this panel will apear and disapear immidiatly.
//The image of the panel which holds this scripts can be set invisible by setting the alpha of its image to transparent.
public abstract class ResumePanel : MonoBehaviour, IPointerClickHandler
{
    public Canvas canvasPopUpToClose;
    private CanvasGroup canvasGroupPopUpToClose;
    protected Inventory inventory;
    private Canvas canvasResumePanel;

    //The inventory Canvas needs to be hidden by its Canvas Group when the canvas of this gameobject is active.
    public CanvasGroup canvasInventory;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroupPopUpToClose = canvasPopUpToClose.GetComponent<CanvasGroup>();
        canvasResumePanel = gameObject.GetComponent<Canvas>();
        openCloseLockInventroyCanvas(false);
        inventory = API.Inventory;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //canvasToClose.SetActive(false);
        openCloseLockInventroyCanvas(false);
        onPointerAction();
        //needs to be set to false so player can move player again.
        //It is set to true when this panel is opened up through the script ImagePopUp
        //or its subclasses LockDoor or ImagePopUp.
        inventory.InteractionWithUIActive = false;
    }

    public abstract void onPointerAction();

    public void openCanvas()
    {
        openCloseLockInventroyCanvas(true);
    }

    protected void openCloseLockInventroyCanvas(bool openLockCloseInventory)
    {
        if (openLockCloseInventory)
        {
            canvasPopUpToClose.sortingOrder = 8;
            canvasResumePanel.sortingOrder = 8;


        }
        else
        {
            canvasPopUpToClose.sortingOrder = 0;
            canvasResumePanel.sortingOrder = 0;
        }

        canvasGroupPopUpToClose.alpha = Convert.ToInt32(openLockCloseInventory);
        canvasGroupPopUpToClose.blocksRaycasts = openLockCloseInventory;
        canvasGroupPopUpToClose.interactable = openLockCloseInventory;



        canvasInventory.alpha = Convert.ToInt32(!openLockCloseInventory);
        canvasInventory.blocksRaycasts = !openLockCloseInventory;
        canvasInventory.interactable = !openLockCloseInventory;

    }
}
