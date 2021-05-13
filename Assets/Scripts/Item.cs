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
    private SceneManager sceneManager;
    // Start is called before the first frame update
    void Start()
    {
        itemCollider = gameObject.GetComponent<PolygonCollider2D>();
        inventory = API.Inventory;
        inputManager = API.InputManager;
        sceneManager = API.SceneManager;
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
        if (collision.gameObject.tag == "Player")
        {
            playerIsColliding = true;
            //Debug.Log("player is colliding"+ playerIsColliding);
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
            playerIsColliding = false;

            if (!sceneManager.IsFading)
            {
                //Debug.Log("MouseExit");
                inventory.InteractionWithInventoryActive = false;
            }
        }
    }

    private void lockMovementAndPutItemIntoInventory()
    {
        if (!inventory.isInvetoryFull())
        {
            //inventory.InteractionWithInventoryActive is set to false in inventory after putting this item into the inventory slot.
            inventory.InteractionWithInventoryActive = true;
            //OnItemCollision event is handled in the inventory script via HandleOnItemCollision method



            OnItemCollision?.Invoke();
            //replacement of OnItemCollision?.Invoke(); to call events with IEnumerator as return type and Coroutines in Handler Methods:
            /*
            if (OnItemCollision != null)
            {
                for (int n = OnItemCollision.GetInvocationList().Length - 1; n >= 0; n--)
                {
                    HandleItemCollision onCollisionWithPortalCoroutine = OnItemCollision.GetInvocationList()[n] as HandleItemCollision;
                    StartCoroutine(OnItemCollision());
                }
            }*/


            gameObject.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        //Debug.Log("MouseOver");
        if(playerIsColliding)
            inventory.InteractionWithInventoryActive = true;
    }
    private void OnMouseExit()
    {
        sceneManager = API.SceneManager;
        if (!sceneManager.IsFading)
        {
            //Debug.Log("MouseExit");
            inventory.InteractionWithInventoryActive = false;
        }
    }

    private void OnMouseOver()
    {
        //Debug.Log("MouseOver");
        if (playerIsColliding)
            inventory.InteractionWithInventoryActive = true;
    }

}
