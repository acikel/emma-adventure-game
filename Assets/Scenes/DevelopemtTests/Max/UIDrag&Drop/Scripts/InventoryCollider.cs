using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Entered");
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entered2");
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Entered3");
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Entered4");
        inventory = API.Inventory;
        inventory.InteractionWithInventoryActive = true;
    }
}
