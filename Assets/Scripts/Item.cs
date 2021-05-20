using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    //ordered list (first sprite is first sprite to change to when the corresponding first item of list dragObjects is dragged onto this item)
    //of sprites which it will change to if an item is draged onto it.
    public List<Sprite> orderedSpritesToChange;
    //ordered list of gameobjects (first game object change sprite of this item to first sprite of list orderedSpritesToChange)
    //which can be tragged to this item and cause the sprite changed of orderedSpritesToChange.
    //wenn dragObjects liste leer ist und orderedSpritesToChange vool dann �ndert sich das item sprite immer wenn drauf geklickt wird bis
    //alle sprites von 0,1,2... bis ende bei jedem klick durchiteriert sind.
    public List<InventoryItem> dragObjects;
    //holds the current index of orderedSpritesToChange and dragObjects to know which sprite the item this item schould be changed to next.
    private int counterOfDragItems;
    //Dropoff for sprite to subscribe to so item can compare if the right item was draged to the dropoff scene (dragObjects)
    //and change the sprite of the item accordingly.
    //this field need to be assigned manually so the right dropOff zone talks to the right item to change its sprites. 
    //Thats why the events of Dropoffzone are not static but bind to one DropOfZone too.
    public DropOff dropOffZone;

    private SpriteRenderer spriteRenderer;
    //subscribed by inventory.cs
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
        if(playerIsColliding && !inventory.isInvetoryFull() && inputManager.checkIfSpecificColliderWasHit("Item", itemCollider))
        {
            if (dragObjects.Count == 0 && orderedSpritesToChange.Count == 0)
            {
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
            inventory.InteractionWithInventoryActive = true;
            //OnItemCollision event is handled in the inventory script via HandleOnItemCollision method



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

            if(orderedSpritesToChange.Count == 0)
            {
                gameObject.SetActive(false);
            }
            
        }
    }

    private void OnMouseEnter()
    {
        //Debug.Log("MouseOver");
        if(playerIsColliding)
            inventory.InteractionWithInventoryActive = true;
    }
    private void OnMouseExit()
    {
        sceneManager = API.SceneManager;
        if (!sceneManager.IsFading)
        {
            //Debug.Log("MouseExit");
            inventory.InteractionWithInventoryActive = false;
        }
    }

    private void OnMouseOver()
    {
        //Debug.Log("MouseOver");
        if (playerIsColliding)
            inventory.InteractionWithInventoryActive = true;
    }

    private void OnMouseDown()
    {
        
    }
}
