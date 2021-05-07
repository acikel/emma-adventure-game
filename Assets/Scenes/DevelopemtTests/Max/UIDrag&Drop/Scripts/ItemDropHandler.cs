using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    Inventory inventory;

    private void Start()
    {
        inventory = API.Inventory;
    }

    private void Update()
    {
        
    }
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
}
