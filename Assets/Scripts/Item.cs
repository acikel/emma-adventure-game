using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public delegate void HandleItemCollision();
    public static event HandleItemCollision OnItemCollision;
    private InputManager inputManager;
    private Inventory inventory;
    private Collider2D itemCollider;
    private bool playerIsColliding;
    // Start is called before the first frame update
    void Start()
    {
        itemCollider = gameObject.GetComponent<PolygonCollider2D>();
        inventory = API.Inventory;
        inputManager = API.InputManager;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerIsColliding && inputManager.isMouseDown() && inputManager.checkIfSpecificColliderWasHit("Item", itemCollider))
        {
            lockMovementAndPutItemIntoInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inventory.isInvetoryFull() && inputManager.checkIfSpecificColliderWasHit("Item", itemCollider) && collision.gameObject.tag == "Player")
        {
            lockMovementAndPutItemIntoInventory();
        }
        else if (collision.gameObject.tag == "Player")
        {
            playerIsColliding = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!inventory.isInvetoryFull() && inputManager.checkIfSpecificColliderWasHit("Item", itemCollider) && collision.gameObject.tag == "Player" && gameObject.activeSelf)
        {
            lockMovementAndPutItemIntoInventory();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            inventory.InteractionWithInventoryActive = false;
        }
    }

    private void lockMovementAndPutItemIntoInventory()
    {
        inventory.InteractionWithInventoryActive = true;
        OnItemCollision?.Invoke();

        gameObject.SetActive(false);
    }
    
}
