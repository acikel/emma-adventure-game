using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    //ordered list (first sprite is first sprite to change to when the corresponding first item of list dragObjects is dragged onto this item)
    //of sprites which it will change to if an item is draged onto it.
    //this only happens if orderedSpritesToChange and dragObjects are not empty.
    //If orderedSpritesToChange is not empty and dragObjects is empty. Only sprites will changed when item is put into inventory.
    //There is no need to orderedSpritesToChange beeing empty and dragObjects beeing not empty so its not handled.
    //If orderedSpritesToChange and dragObjects are empty this item is put into inventory without other effects.
    public List<Sprite> orderedSpritesToChange;
    //ordered list of gameobjects (first game object change sprite of this item to first sprite of list orderedSpritesToChange)
    //which can be tragged to this item and cause the sprite changed of orderedSpritesToChange.
    //wenn dragObjects liste leer ist und orderedSpritesToChange vool dann ändert sich das item sprite immer wenn drauf geklickt wird bis
    //alle sprites von 0,1,2... bis ende bei jedem klick durchiteriert sind.
    public List<InventoryItem> dragObjects;
    //holds the current index of orderedSpritesToChange and dragObjects to know which sprite the item this item schould be changed to next.
    private int counterOfDragItems;
    //Dropoff for sprite to subscribe to so item can compare if the right item was draged to the dropoff scene (dragObjects)
    //and change the sprite of the item accordingly.
    //this field need to be assigned manually so the right dropOff zone talks to the right item to change its sprites. 
    //Thats why the events of Dropoffzone are not static but bind to one DropOfZone too.
    public DropOff dropOffZone;
    //gameObjectToDeactivateAfterLastDrag is the GameObject which is deactivated after the last item of dragObjects was dragged onto this item.
    //If orderedSpritesToChange.Count == 0 this item is deactivated. This way no item is deactivated if 
    //orderedSpritesToChange has items and the last changed item stays in the scene for this gameObjectToDeactivateAfterLastDrag needs to be 
    //null otherwise gameObjectToDeactivateAfterLastDrag will be deactivated even if this item stays visible.
    public GameObject gameObjectToDeactivateAfterLastDrag;

    private SpriteRenderer spriteRenderer;
    //subscribed by inventory.cs used to search for the corresponding sprite to instanciate in list. This items corresponding sprite needs to be added to the list of inventory.cs via unity editor too.
    public delegate void HandleItemCollision(string itemName);
    public static event HandleItemCollision OnItemCollision;
    private InputManager inputManager;
    private Inventory inventory;
    private Collider2D itemCollider;
    private bool playerIsColliding;
    private SceneManager sceneManager;
    // Start is called before the first frame update
    void Start()
    {
        if(dropOffZone!=null)
            dropOffZone.OnItemDrop += ChangeSpriteAccordingToDropedItem;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        itemCollider = gameObject.GetComponent<Collider2D>();
        inventory = API.Inventory;
        inputManager = API.InputManager;
        sceneManager = API.SceneManager;
        counterOfDragItems = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputManager.isMouseDown())
        {
            putItemToInventory();
        }
    }

    private void ChangeSpriteAccordingToDropedItem(string itemName)
    {
        //Debug.Log("ChangeSpriteAccordingToDropedItem1 dragObjects: "+ dragObjects 
        //    + " orderedSpritesToChange: "+ orderedSpritesToChange + " spriteRenderer: "+ spriteRenderer);
        if (dragObjects == null || orderedSpritesToChange == null || spriteRenderer == null)
            return;

        //Debug.Log("ChangeSpriteAccordingToDropedItem1.5 itemName: " + itemName 
        //    + " dragObjects[counterOfDragItems].inventoryItemName "+ dragObjects[counterOfDragItems].inventoryItemName);
        if (itemName.Contains(dragObjects[counterOfDragItems].inventoryItemName)){
            spriteRenderer.sprite = orderedSpritesToChange[counterOfDragItems];
            counterOfDragItems++;
            //when the last inventory item was dragged onto this item the item with the name itemName is put into the inventory.
            if (counterOfDragItems>= dragObjects.Count)
            {
                lockMovementAndPutItemIntoInventory(this.itemName);
            }
            //Debug.Log("ChangeSpriteAccordingToDropedItem2");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {
            playerIsColliding = true;
            //Debug.Log("player is colliding"+ playerIsColliding);
        }
        putItemToInventory();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void putItemToInventory()
    {
        //Debug.Log("itemCollider name: " + itemCollider.name);
        //Debug.Log("putItemToInventory1: " + itemName + "playerIsColliding:"+ playerIsColliding + "inputManager.checkIfSpecificColliderWasHit: "+ inputManager.checkIfSpecificColliderWasHit("Item", itemCollider));
        if (playerIsColliding && !inventory.isInvetoryFull() && inputManager.checkIfSpecificColliderWasHit("Item", itemCollider))
        {
            //Debug.Log("putItemToInventory1.5: " + itemName);
            if (dragObjects.Count == 0 && orderedSpritesToChange.Count == 0)
            {
                //Debug.Log("putItemToInventory2: " + itemName);
                lockMovementAndPutItemIntoInventory(itemName);
            }
            else if (dragObjects.Count == 0 && orderedSpritesToChange.Count != 0)
            {
                if (counterOfDragItems < orderedSpritesToChange.Count && spriteRenderer != null)
                {
                    spriteRenderer.sprite = orderedSpritesToChange[counterOfDragItems];
                    counterOfDragItems++;
                    lockMovementAndPutItemIntoInventory(itemName);
                }
            }

            
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerIsColliding = false;
        }
    }

    private void lockMovementAndPutItemIntoInventory(string itemName)
    {
        if (!inventory.isInvetoryFull())
        {
            //inventory.InteractionWithInventoryActive is set to false in inventory after putting this item into the inventory slot.
            inventory.InteractionWithUIActive = true;
            //OnItemCollision event is handled in the inventory script via HandleOnItemCollision method


            //Debug.Log("itemName: "+ itemName);
            OnItemCollision?.Invoke(itemName);
            //replacement of OnItemCollision?.Invoke(); to call events with IEnumerator as return type and Coroutines in Handler Methods:
            /*
            if (OnItemCollision != null)
            {
                for (int n = OnItemCollision.GetInvocationList().Length - 1; n >= 0; n--)
                {
                    HandleItemCollision onCollisionWithPortalCoroutine = OnItemCollision.GetInvocationList()[n] as HandleItemCollision;
                    StartCoroutine(OnItemCollision());
                }
            }*/

            /*
            if(orderedSpritesToChange.Count == 0)
            {
                gameObject.SetActive(false);
            }*/
            if (gameObjectToDeactivateAfterLastDrag != null)
            {
                gameObjectToDeactivateAfterLastDrag.SetActive(false);
            }
            else if(orderedSpritesToChange.Count == 0)
            {
                gameObject.SetActive(false);
            }
            
        }
    }

    private void OnMouseEnter()
    {
        //Debug.Log("MouseOver");
        if(playerIsColliding)
            inventory.InteractionWithUIActive = true;
    }
    private void OnMouseExit()
    {
        sceneManager = API.SceneManager;
        if (!sceneManager.IsFading)
        {
            //Debug.Log("MouseExit");
            inventory.InteractionWithUIActive = false;
        }
    }

    private void OnMouseOver()
    {
        //Debug.Log("MouseOver");
        if (playerIsColliding)
            inventory.InteractionWithUIActive = true;
    }

    private void OnMouseDown()
    {
        
    }
}
