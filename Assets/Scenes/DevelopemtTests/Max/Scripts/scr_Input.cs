using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Input : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemSprite;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
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
                    for (int i = 0; i < inventory.slots.Length; i++)
                    {
                        if (inventory.isFull[i] == false)
                        {
                            inventory.isFull[i] = true;
                            Instantiate(itemSprite, inventory.slots[i].transform, false);
                            Destroy(hit.collider.gameObject);
                            break;
                        }
                        //https://www.youtube.com/watch?v=DLAIYSMYy2g
                    }
                }
            }
        }
    }
}
