using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public delegate void HandleItemCollision();
    public static event HandleItemCollision OnItemCollision;
    private InputManager inputManager;
    private Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = API.Inventory;
        inputManager = API.InputManager;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inventory.isInvetoryFull() && inputManager.checkIfColliderWasHit("Item") && collision.gameObject.tag == "Player")
        {
            OnItemCollision?.Invoke();

            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
