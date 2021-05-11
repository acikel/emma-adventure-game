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
    }

    void OnClick()
    {
        if (InventorySlots.activeSelf == true)
        {
            buttonSprite.sprite = closedBackpackSprite;
            openInventorySlotsAndBlockMovementForInventory(false);
        }
        else
        {
            if (InventorySlots.activeSelf == false)
            {
                buttonSprite.sprite = openedBackpackSprite;
                openInventorySlotsAndBlockMovementForInventory(true);
            }
        }
        
    }

    private void OnMouseOver()
    {
        inventory.InteractionWithInventoryActive = true;
    }

    private void openInventorySlotsAndBlockMovementForInventory(bool value)
    {
        InventorySlots.SetActive(value);
        InventoryBlocker.SetActive(value);
    }
}
