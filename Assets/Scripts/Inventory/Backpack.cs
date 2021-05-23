using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public Button BackpackButton;
    public GameObject InventorySlots;
    public GameObject InventoryBlocker;

    public Sprite openedBackpackSprite;
    public Sprite closedBackpackSprite;
    private Image buttonSprite;

    private Inventory inventory;

    private void Start()
    {
        BackpackButton.onClick.AddListener(OnClick);
        buttonSprite = GetComponent<Image>();
        inventory = API.Inventory;
        Inventory.OnOpenInventory += openInventorySlotsAndBlockMovementForInventory;
    }

    private void OnDisable()
    {
        Inventory.OnOpenInventory -= openInventorySlotsAndBlockMovementForInventory;
    }
    void OnClick()
    {
        if (InventorySlots.activeSelf == true)
        {
            openInventorySlotsAndBlockMovementForInventory(false);
        }
        else
        {
            if (InventorySlots.activeSelf == false)
            {
                
                openInventorySlotsAndBlockMovementForInventory(true);
            }
        }
        
    }

    private void OnMouseOver()
    {
        inventory.InteractionWithUIActive = true;
    }

    private void openInventorySlotsAndBlockMovementForInventory(bool value)
    {
        InventorySlots.SetActive(value);
        InventoryBlocker.SetActive(value);

        if (value)
        {
            buttonSprite.sprite = openedBackpackSprite;
        }
        else
        {
            buttonSprite.sprite = closedBackpackSprite;
        }
    }
}
