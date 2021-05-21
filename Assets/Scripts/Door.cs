using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This scripts activates the door doorToActivate and deactivates this door, when the right item 
//(with name defined in itemNameToDrop in ActionObject, this needs to contain inventoryItemName value of InventoryItem.cs as this 
//is the object which will be dragged to the dropzone dropOff defined in  ActionObject)
//is dragged into dropzone dropOff (defined in  ActionObject).
public class Door : ActionObject
{
    public GameObject doorToActivate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void actionOnDrop(string itemName)
    {
        doorToActivate.SetActive(true);
        gameObject.SetActive(false);

    }
}
