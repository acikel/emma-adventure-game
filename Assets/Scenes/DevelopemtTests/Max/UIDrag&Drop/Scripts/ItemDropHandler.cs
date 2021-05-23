using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    Inventory inventory;
    DropOff dropOff;
    string currentlyDragedItemName;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }


    public void OnDrop(PointerEventData eventData)
    {
        //dropOff = this.gameObject.transform.Find("DropOffInfo").GetComponent<DropOff>();
        dropOff = GameObject.Find("DropOffInfo")?.GetComponent<DropOff>();
        inventory = API.Inventory;
        currentlyDragedItemName = inventory.CurrentlyDraggedSlot.gameObject.transform.GetChild(0).name;
        Debug.Log("DropOffInfo "+ dropOff);
        if (dropOff != null)
        {
            if (inventory.CurrentlyDraggedSlot.gameObject.transform.childCount > 0 &&  dropOff.isDropOfCollidingWithCorrectItem(currentlyDragedItemName) && dropOff.PlayerColliding && dropOff.ItemColliding)
            {
                dropOff.invokeOnItemDrop(currentlyDragedItemName);
                inventory.setCurrentlyDraggedSlotToEmpty();
                dropOff.moveToNextItemToDrop();
                dropOff.ItemColliding = false;
                GameObject.Destroy(inventory.CurrentlyDraggedSlot.transform.GetChild(0).gameObject);
            }
            /*
            foreach (string itemName in dropOff.itemNameToDrop)
            {
                //Debug.Log("itemname:" + itemName);
                //Debug.Log("iventory current tag"+ inventory.CurrentlyDraggedSlot.gameObject.transform.GetChild(0).tag.Equals(itemName));
                //Debug.Log("player:" + dropOff.PlayerColliding);
                if (inventory.CurrentlyDraggedSlot.gameObject.transform.childCount > 0 && inventory.CurrentlyDraggedSlot.gameObject.transform.GetChild(0).tag.Equals(itemName) && dropOff.PlayerColliding && dropOff.ItemColliding)
                {
                    inventory.setCurrentlyDraggedSlotToEmpty();
                    GameObject.Destroy(inventory.CurrentlyDraggedSlot.transform.GetChild(0).gameObject);
                }
            }
            */
        }


    }

    /*
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "DropOff")
                {
                    inventory.setCurrentlyDraggedSlotToEmpty();
                    foreach (Transform child in transform) {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }
    }
    */
}
