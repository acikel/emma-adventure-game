using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public GameObject itemSprite;
    public GameObject Itemslots;
    

    public bool[] isFull;
    public GameObject[] slots;

    private bool interactionWithInventoryActive;
    private GameObject currentlyDraggedSlot;
    private bool collisionWasHandled;
    private InputManager inputManager;

    public bool InteractionWithInventoryActive
    {
        get
        {
            return interactionWithInventoryActive;
        }
        set
        {
            interactionWithInventoryActive = value;
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
        interactionWithInventoryActive = false;
        for (int i=0 ; i < isFull.Length; i++)
        {
            isFull[i] = false;
        }
        Itemslots.SetActive(false);
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
        if(collisionWasHandled && !inputManager.isMouseDown())
        {
            interactionWithInventoryActive = false;
            collisionWasHandled = false;
        }
            
    }

    private void HandleOnItemCollision()
    {
        Itemslots.SetActive(true);
        for (int i = 0; i < slots.Length; i++)
        {
            if (isFull[i] == false)
            {
                isFull[i] = true;
                Instantiate(itemSprite, slots[i].transform, false);
                break;
            }
        }
        collisionWasHandled = true;
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

   
}
