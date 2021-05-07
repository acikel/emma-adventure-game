using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public Button BackpackButton;
    public GameObject InventorySlots;

    private void Start()
    {
        BackpackButton.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        if (InventorySlots.activeSelf == true)
        {

            InventorySlots.SetActive(false);
        }
        else
        {
            if (InventorySlots.activeSelf == false)
            {

                InventorySlots.SetActive(true);
            }
        }
    }
}
