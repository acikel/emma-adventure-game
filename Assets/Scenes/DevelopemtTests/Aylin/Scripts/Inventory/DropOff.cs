using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropOff : MonoBehaviour
{
    Inventory inventory;
    InputManager inputManager;
    private bool playerColliding;
    private bool itemColliding;

    //bool to tell if list of names (itemNameToDrop) should be ordered (if list has more then one item first item need to be dropped 
    //frist then second and so on) or not.
    public bool orderedList;

    //names of items that can be dropped to this dropzone. 
    //Need to be same (and same order) as the ones defined in Item.cs defined in dragObjects if drag should activate a sprite change on an item (these sprites are defined in orderedSpritesToChange).
    [SerializeField]
    public List<string> itemNameToDrop=new List<string>();
    
    //variable to track which object need to collide next if list is ordered (orderedList=true);
    private int currentListItem;
    //subscribed by Item.cs and event called by ItemDropHandler which has a dropOffObject as Child.
    public delegate void HandleItemDrop(string itemName);
    public event HandleItemDrop OnItemDrop;


    private string currentlyDragedItemName;
    private Image imageOfCurrentlyDragedItemSlot;

    public bool PlayerColliding
    {
        get
        {
            return playerColliding;
        }
    }

    public bool ItemColliding
    {
        get
        {
            return itemColliding;
        }
        //need to be set by ItemDropHandler.cs when item is distroyed as it wont exit this trigger in that case
        //for the next item to be placed exactly on the dropoff zone itemColliding need to be reset to false.
        set
        {
            itemColliding = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inputManager = API.InputManager;
        inventory = API.Inventory;
        currentListItem = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputManager.isMouseDown())
        {
            dropDraggedItem();
        }
    }

    //Called by ItemDropHandler which has a dropOffObject as Child.
    public void invokeOnItemDrop(string currentlyDragedItemName)
    {
        OnItemDrop?.Invoke(currentlyDragedItemName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision: "+collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
            playerColliding = true;
        /* else
         {
             foreach (string itemName in itemNameToDrop)
             {
                 if (inventory.CurrentlyDraggedSlot.gameObject.tag == itemName && playerColliding)
                 {
                     inventory.setCurrentlyDraggedSlotToEmpty();
                     GameObject.Destroy(inventory.CurrentlyDraggedSlot.transform.GetChild(0).gameObject);
                 }
             }
         }*/
        else if (isDropOfCollidingWithCorrectItem(collision))
        {
            //Debug.Log("item colliding start"+ collision.gameObject.name);
            itemColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerColliding = false;
        }
        else if (isDropOfCollidingWithCorrectItem(collision))
        {
           //Debug.Log("item colliding end");
            itemColliding = false;
        }
        
    }

    private void dropDraggedItem()
    {
        if (inventory.CurrentlyDraggedSlot.gameObject.transform.childCount == 0)
            return;

        currentlyDragedItemName = inventory.CurrentlyDraggedSlot.gameObject.transform.GetChild(0).name;
        //Debug.Log("DropOffInfoOnMouseUp " + inventory.CurrentlyDraggedSlot.gameObject.transform.childCount + " 2: " + isDropOfCollidingWithCorrectItem(currentlyDragedItemName) + " PlayerColliding: " + PlayerColliding + " ItemColliding: " + ItemColliding);
        {
            if (inventory.CurrentlyDraggedSlot.gameObject.transform.childCount > 0 && isDropOfCollidingWithCorrectItem(currentlyDragedItemName) /*&& PlayerColliding*/ && ItemColliding) //On drag player does not have to collide with dropoff only when picking up item.
            {
                invokeOnItemDrop(currentlyDragedItemName);
                inventory.setCurrentlyDraggedSlotToEmpty();
                moveToNextItemToDrop();
                ItemColliding = false;

                //Set background of inventory slot to black if it was set to gray for the currently item.
                imageOfCurrentlyDragedItemSlot = inventory.CurrentlyDraggedSlot.gameObject.GetComponent<Image>();
                if (imageOfCurrentlyDragedItemSlot != null && imageOfCurrentlyDragedItemSlot.sprite.name.Contains("gray"))
                {
                    imageOfCurrentlyDragedItemSlot.sprite = inventory.blackBackground;
                }

                GameObject.Destroy(inventory.CurrentlyDraggedSlot.transform.GetChild(0).gameObject);
                
                
            }
        }
    }
    /*
        private void OnMouseUp()
        {
            dropDraggedItem();
        }
    */

    private bool isDropOfCollidingWithCorrectItem(Collider2D collision)
    {
        if (itemNameToDrop == null)
            return false;
            
        if (orderedList)
        {
            return searchForItemInOrderedList(collision.gameObject.name);
        }
        else
        {
            return searchForItemInUnorderedList(collision.gameObject.name);
        }
    }

    public bool isDropOfCollidingWithCorrectItem(string nameToCompare)
    {
        if (itemNameToDrop == null)
            return false;

        if (orderedList)
        {
            return searchForItemInOrderedList(nameToCompare);
        }
        else
        {
            return searchForItemInUnorderedList(nameToCompare);
        }
        
    }

    //used by ItemDropHandler as user can also just drag and not drop item to dropzone.
    //currentListItem should only be incremented if right item was dropped.
    public void moveToNextItemToDrop()
    {
        currentListItem++;
    }
    private bool searchForItemInOrderedList(string nameToCompare)
    {
        //Debug.Log("searchForItemInOrderedList currentListItem: " + currentListItem) ;
        //Debug.Log("searchForItemInOrderedList itemNameToDrop.Count: " + itemNameToDrop.Count);
        //Debug.Log("searchForItemInOrderedList nameToCompare: " + nameToCompare);
        //Debug.Log("searchForItemInOrderedList itemNameToDrop[currentListItem]: " + itemNameToDrop[currentListItem]);
        if (currentListItem < itemNameToDrop.Count && nameToCompare.Contains(itemNameToDrop[currentListItem]))
        {
            //Debug.Log("searchForItemInOrderedList true");
            return true;
        }
        else
        {
            //Debug.Log("searchForItemInOrderedList false");
            return false;
        }
    }

    private bool searchForItemInUnorderedList(string nameToCompare)
    {
        foreach (string name in itemNameToDrop)
        {
            if (nameToCompare.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
}
