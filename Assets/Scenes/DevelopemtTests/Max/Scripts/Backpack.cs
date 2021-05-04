using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public Button BackpackButton;
    public GameObject Inventory;


    private void Start()
    {
        BackpackButton.onClick.AddListener(OnClick);

    }

    void OnClick()
    {
        if (Inventory.activeSelf == true)
        {

            Inventory.SetActive(false);
        }
        else
        {
            if (Inventory.activeSelf == false)
            {

                Inventory.SetActive(true);
            }
        }
    }
}
