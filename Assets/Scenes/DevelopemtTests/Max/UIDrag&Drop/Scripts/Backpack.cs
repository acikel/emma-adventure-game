using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public Button BackpackButton;
    public GameObject InventorySlots;
    public GameObject InventoryBlocker;

    private void Start()
    {
        BackpackButton.onClick.AddListener(OnClick);

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

    private void openInventorySlotsAndBlockMovementForInventory(bool value)
    {
        InventorySlots.SetActive(value);
        InventoryBlocker.SetActive(value);
    }
}
