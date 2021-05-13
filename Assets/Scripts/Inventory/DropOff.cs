using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOff : MonoBehaviour
{
    Inventory inventory;
    private bool playerColliding;

    [SerializeField]
    public List<string> itemNameToDrop=new List<string>();

    public bool PlayerColliding
    {
        get
        {
            return playerColliding;
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision: "+collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
            playerColliding = true;
        /* else
         {
             foreach (string itemName in itemNameToDrop)
             {
                 if (inventory.CurrentlyDraggedSlot.gameObject.tag == itemName && playerColliding)
                 {
                     inventory.setCurrentlyDraggedSlotToEmpty();
                     GameObject.Destroy(inventory.CurrentlyDraggedSlot.transform.GetChild(0).gameObject);
                 }
             }
         }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            playerColliding = false;
    }
}