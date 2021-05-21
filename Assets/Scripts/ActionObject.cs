using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionObject : MonoBehaviour
{
    //Dropoff to subscribe to, to get notified if a item was dragged onto the corresponding dropzone.
    //And after checking if its the right item (in order of the list excecute needed action with actionOnDrop (like animating the npc or deactivating it etc).
    public DropOff dropOff;
    //bool to tell if list of names (itemNameToDrop) should be ordered (if list has more then one item first item need to be dropped 
    //frist then second and so on) or not.
    public bool orderedList;
    //names of items that can be dropped to this dropzone. 
    [SerializeField]
    public List<string> itemNameToDrop = new List<string>();
    //variable to track which object need to collide next if list is ordered (orderedList=true);
    private int currentListItem;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        currentListItem = 0;
        //Debug.Log("actions object OnEnable dropOff: " + dropOff);
        if (dropOff != null)
        {
            //Debug.Log("actions object OnEnable assigning drop item ");
            dropOff.OnItemDrop += HandleItemDropOntoDropzone;
        }
    }
    private void OnDisable()
    {
        dropOff.OnItemDrop -= HandleItemDropOntoDropzone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleItemDropOntoDropzone(string itemName)
    {
        if (orderedList && currentListItem< itemNameToDrop.Count)
        {
            if (itemName.Contains(itemNameToDrop[currentListItem]))
            {
                currentListItem++;
                actionOnDrop(itemName);
            }
        }
        else if(!orderedList && currentListItem < itemNameToDrop.Count)
        {
            if (checkIfItemNameIsInList(itemName))
            {
                currentListItem++;
                actionOnDrop(itemName);
            }
        }
    }

    protected bool checkIfItemNameIsInList(string itemName)
    {
        foreach (string name in itemNameToDrop)
        {
            if (itemName.Contains(name))
            {
                return true;
            }
        }
        return false;
    }
    public abstract void actionOnDrop(string itemName);
}
