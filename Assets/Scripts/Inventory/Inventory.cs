using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> inventoryItemSprites;
    public GameObject inventoryItemslots;
    public GameObject InventoryBlocker;


    public bool[] isFull;
    public GameObject[] slots;

    private bool interactionWithUIActive;
    private GameObject currentlyDraggedSlot;
    //private bool collisionWasHandled;
    private InputManager inputManager;

    //Subscribed from backpack.cs
    public delegate void OnOpenInventoryHandler(bool openInventory);
    public static event OnOpenInventoryHandler OnOpenInventory;

    public Sprite blackBackground;
    public Sprite grayBackground;
    private Image spriteRendererOfCurrentlyDraggedSlot;

    public bool InteractionWithUIActive
    {
        get
        {
            return interactionWithUIActive;
        }
        set
        {
            interactionWithUIActive = value;
        }
    }

    public GameObject CurrentlyDraggedSlot
    {
        get
        {
            return currentlyDraggedSlot;
        }
        set
        {
            currentlyDraggedSlot = value;
        }
    }

    void Start()
    {
        inputManager = API.InputManager;
        currentlyDraggedSlot = new GameObject();
        //interactionWithInventoryActive = false;
        for (int i=0 ; i < isFull.Length; i++)
        {
            isFull[i] = false;
        }
        openInventorySlotsAndBlockMovementForInventory(false);
    }
    private void OnEnable()
    {
        Item.OnItemCollision += HandleOnItemCollision;
    }
    private void OnDisable()
    {
        Item.OnItemCollision -= HandleOnItemCollision;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(collisionWasHandled && !inputManager.isMouseDown())
        {
            interactionWithInventoryActive = false;
            collisionWasHandled = false;
        }
            */
        //Debug.Log("interactionWithInventoryActive: "+ interactionWithInventoryActive);
    }

    private void openInventorySlotsAndBlockMovementForInventory(bool value)
    {
        inventoryItemslots.SetActive(value);
        InventoryBlocker.SetActive(value);
        OnOpenInventory(value);
    }
    private GameObject getCorrespondingSpriteToItemThatWasClicked(string itemName)
    {
        foreach(InventoryItem invItem in inventoryItemSprites) 
        {
            if (itemName.Contains(invItem.inventoryItemName))
            {
                return invItem.gameObject;
            }
        }
        return null;
    }

    private void HandleOnItemCollision(string itemName, bool needsGrayInventoryBackground)
    {
        openInventorySlotsAndBlockMovementForInventory(true);
        GameObject invItem = getCorrespondingSpriteToItemThatWasClicked(itemName);

        for (int i = 0; i < slots.Length; i++)
        {
            if (isFull[i] == false)
            {
                isFull[i] = true;
                GameObject newItem = Instantiate(invItem, slots[i].transform, false);
                newItem.transform.position = slots[i].transform.position;
                spriteRendererOfCurrentlyDraggedSlot = slots[i].GetComponent<Image>();
                if (spriteRendererOfCurrentlyDraggedSlot!=null && needsGrayInventoryBackground)
                    spriteRendererOfCurrentlyDraggedSlot.sprite = grayBackground;
                break;
            }
        }
        //collisionWasHandled = true;
        //yield return new WaitForSeconds(0);
    }

    public void setCurrentlyDraggedSlotToEmpty()
    {
        int index=-1;
        for(index=0; index<slots.Length; index++)
        {
            if (slots[index] == currentlyDraggedSlot)
                break;
        }
        if(index != -1)
            isFull[index] = false;
    }

    public bool isInvetoryFull()
    {
        foreach (bool fullSlot in isFull)
        {
            if (!fullSlot)
                return false;
        }

        return true;
        /*
        if (isFull[isFull.Length-1] == true)
            return true;
        return false;
        */
    }

    //used to clear inventory slots when entering a new sequence (like in Grandma.cs script when loading Sequence2 when ending Seuqence1)
    //or resarting the game (when Start Menu was entered the slots are cleared in the SceneManager).
    public void clearInventorySlots()
    {
        foreach (GameObject slot in slots)
        {
            if(slot.gameObject.transform.childCount>0)
                GameObject.Destroy(slot.gameObject.transform.GetChild(0).gameObject);
        }
        
    }
   
}
