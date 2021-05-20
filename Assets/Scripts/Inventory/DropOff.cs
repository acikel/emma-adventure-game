using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOff : MonoBehaviour
{
    Inventory inventory;
    private bool playerColliding;
    private bool itemColliding;

    [SerializeField]
    public List<string> itemNameToDrop=new List<string>();

    //subscribed by Item.cs and event called by ItemDropHandler which has a dropOffObject as Child.
    public delegate void HandleItemDrop(string itemName);
    public event HandleItemDrop OnItemDrop;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    private bool isDropOfCollidingWithCorrectItem(Collider2D collision)
    {
        foreach(string name in itemNameToDrop)
        {
            if (collision.gameObject.name.Contains(name))
            {
                return true;
            }
        }
        return false;
    }

    public bool isDropOfCollidingWithCorrectItem(string nameToCompare)
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
