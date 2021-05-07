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
        transform.position = Input.mousePosition;
        inventory.CurrentlyDraggedSlot = transform.parent.gameObject;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
