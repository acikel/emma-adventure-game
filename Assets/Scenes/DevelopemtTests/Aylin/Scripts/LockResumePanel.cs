using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LockResumePanel : MonoBehaviour, IPointerClickHandler
{
    //Canvas to close when this panel was clicked on.
    public Canvas canvasLock;
    private CanvasGroup canvasGroupLock;
    private Inventory inventory;
    private Canvas canvasLockResumePanel;

    //The inventory Canvas needs to be hidden by its Canvas Group when the canvas of this gameobject is active.
    public CanvasGroup canvasInventory;

    private void Start()
    {
        canvasGroupLock = canvasLock.GetComponent<CanvasGroup>();
        canvasLockResumePanel = gameObject.GetComponent<Canvas>();
        openCloseLockInventroyCanvas(false);
        inventory = API.Inventory;
        //needs to be active at the beginning so it can be assigned to the LockDoor script.
        //It is set to false in the lockdoor script after assigning.
        //canvasToClose.SetActive(true);
    }
    private void OnEnable()
    {
        LockPanel.OnLockSolved += closeLockCanvas;
    }
    private void OnDisable()
    {
        LockPanel.OnLockSolved -= closeLockCanvas;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        //canvasToClose.SetActive(false);
        openCloseLockInventroyCanvas(false);
        //needs to be set to false so player can move player again.
        //It is set to true when this panel is opened up through the script OpenPopUpWindow
        //or its subclasses LockDoor or OpenImagePopUpCanvas.
        inventory.InteractionWithUIActive = false;
    }

    public void closeLockCanvas()
    {
        StartCoroutine(waitAndCloseLock());
        inventory.InteractionWithUIActive = false;

    }

    private IEnumerator waitAndCloseLock()
    {
        yield return new WaitForSeconds(0.1f);
        openCloseLockInventroyCanvas(false);
    }
    public void openLockCanvas()
    {
        openCloseLockInventroyCanvas(true);
    }

    private void openCloseLockInventroyCanvas(bool openLockCloseInventory)
    {
        if (openLockCloseInventory)
        {
            canvasLock.sortingOrder = 7;
            canvasLockResumePanel.sortingOrder = 7;


        }
        else
        {
            canvasLock.sortingOrder = 0;
            canvasLockResumePanel.sortingOrder = 0;
        }

        canvasGroupLock.alpha = Convert.ToInt32(openLockCloseInventory);
        canvasGroupLock.blocksRaycasts = openLockCloseInventory;
        canvasGroupLock.interactable = openLockCloseInventory;
        

        canvasInventory.alpha = Convert.ToInt32(!openLockCloseInventory);
        canvasInventory.blocksRaycasts = !openLockCloseInventory;
        canvasInventory.interactable = !openLockCloseInventory;
    }
}
