using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//vorheriger name: ItemDragHandler vorbunden mit Item DropHandler
public class InventoryItem : MonoBehaviour, IDragHandler, IEndDragHandler
{
    Inventory inventory;
    public string inventoryItemName;

    public void OnDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = true;
        transform.position = Camera.main.ScreenToWorldPoint((Input.mousePosition));
        inventory.CurrentlyDraggedSlot = transform.parent.gameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        transform.localPosition = Vector3.zero;
        inventory.InteractionWithUIActive = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
        inventory.InteractionWithUIActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
