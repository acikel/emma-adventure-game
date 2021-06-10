using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//vorheriger name: ItemDragHandler vorbunden mit Item DropHandler
public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler
{
    Inventory inventory;
    public string inventoryItemName;
    private Image imageOfCurrentlyDragedItemSlot;
    private bool inventoryBackgroundWasGray;

    public void OnDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = true;
        transform.position = Camera.main.ScreenToWorldPoint((Input.mousePosition));
        inventory.CurrentlyDraggedSlot = transform.parent.gameObject;

        //if iventory background is gray set it to black while dragging
        imageOfCurrentlyDragedItemSlot = inventory.CurrentlyDraggedSlot.gameObject.GetComponent<Image>();
        if (imageOfCurrentlyDragedItemSlot != null && imageOfCurrentlyDragedItemSlot.sprite.name.Contains("gray"))
        {
            inventoryBackgroundWasGray = true;
            imageOfCurrentlyDragedItemSlot.sprite = inventory.blackBackground;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        transform.localPosition = Vector3.zero;
        inventory.InteractionWithUIActive = false;

        //Reset gray background if iventory inventory item is set back to invetory.
        imageOfCurrentlyDragedItemSlot = inventory.CurrentlyDraggedSlot.gameObject.GetComponent<Image>();
        if (inventoryBackgroundWasGray)
        {
            imageOfCurrentlyDragedItemSlot.sprite = inventory.grayBackground;
            inventoryBackgroundWasGray = false;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        inventoryBackgroundWasGray = false;
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
