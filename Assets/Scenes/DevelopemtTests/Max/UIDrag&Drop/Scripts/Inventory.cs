using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    public GameObject itemSprite;
    public GameObject Itemslots;

    public bool[] isFull;
    public GameObject[] slots;
    private bool interactionWithInventoryActive;
    private GameObject currentlyDraggedSlot;

    public bool InteractionWithInventoryActive
    {
        get
        {
            return interactionWithInventoryActive;
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
        interactionWithInventoryActive = false;
        for (int i=0 ; i < isFull.Length; i++)
        {
            isFull[i] = false;
        }
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
    }
    public void setCurrentlyDraggedSlotToEmpty()
    {
        int index;
        for(index=0; index<slots.Length; index++)
        {
            if (slots[index] == currentlyDraggedSlot)
                break;
        }
        isFull[index] = false;
    }

    public bool isInvetoryFull()
    {
        if (isFull[isFull.Length-1] == true)
            return true;
        return false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactionWithInventoryActive = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        interactionWithInventoryActive = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        interactionWithInventoryActive = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        interactionWithInventoryActive = true;
    }
}
