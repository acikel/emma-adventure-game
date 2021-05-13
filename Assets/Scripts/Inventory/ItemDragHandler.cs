using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    Inventory inventory;
    public void OnDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = true;
        transform.position = Camera.main.ScreenToWorldPoint((Input.mousePosition));
        inventory.CurrentlyDraggedSlot = transform.parent.gameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory = API.Inventory;
        transform.localPosition = Vector3.zero;
        inventory.InteractionWithInventoryActive = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
