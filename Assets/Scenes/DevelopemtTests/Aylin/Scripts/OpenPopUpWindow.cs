using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OpenPopUpWindow : MonoBehaviour
{
    //Asign canvas to open when 2D collider of this game object was clicked.
    [HideInInspector]//only protected for subclass but not for editor.
    protected CanvasGroup canvasToOpen;
    private Inventory inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
        initializeCanvasToOpen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this is the method of a subclass (like LockDoor and...) which needs to initialize canvasToOpen via API class 
    //as popup canvases (CanvasLock and CanvasPopUpImage) are located in the base 
    //scene and therefore not asignable through editor window as they are in different scenes.
    public abstract void initializeCanvasToOpen();

    //method which returns bool when canvas was opened to lock playerMovement.
    //inventory.InteractionWithUIActive need to set to false when PopUpWindow is closed again.
    //This needs to be done in script of PopUpWindow itself (like in LockResumePanel Or PopUpImage).
    public abstract bool activatePopUpWindow();

    public void OnMouseOver()
    {
        //Debug.Log("OnMouseOver1");
        inventory.InteractionWithUIActive = true;
    }

    public void OnMouseExit()
    {
        //Debug.Log("OnMouseExit1");
        if (canvasToOpen!=null && canvasToOpen.alpha==0)
        {
            //Debug.Log("OnMouseExit1b");
            inventory.InteractionWithUIActive = false;
        }
        
    }

    public void OnMouseDown()
    {
        //Debug.Log("OnMouseDown1");
        if (activatePopUpWindow())
        {
            //Debug.Log("OnMouseDown2");
            inventory.InteractionWithUIActive = true;
        }
        
    }

}
