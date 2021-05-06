using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Input : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemSprite;
    public GameObject Itemslots;
    private bool isDragging;

    void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.tag == "Item")
                {
                    Itemslots.SetActive(true);
                    for (int i = 0; i < inventory.slots.Length; i++)
                    {
                        if (inventory.isFull[i] == false)
                        {
                            inventory.isFull[i] = true;
                            Instantiate(itemSprite, inventory.slots[i].transform, false);
                            Destroy(hit.collider.gameObject);
                            break;
                        }
                    }
                }
            }
        }


    }

}
