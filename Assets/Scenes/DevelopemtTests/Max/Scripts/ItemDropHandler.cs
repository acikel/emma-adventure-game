using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    private Inventory inventory;
    public int i;
    private bool playerentered;
    private bool playerexit;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inventory.isFull[i] = false;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Object" && playerentered)
                {
                    foreach (Transform child in transform) {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }
        }
    }
    private void OnEnable()
    {
        DropOff.entered += entered;
        DropOff.entered -= exit;

    }

    private void OnDisable()
    {
        DropOff.entered -= entered;
        DropOff.entered += exit;
    }

    void entered()
    {
        playerentered = true;
    }

    void exit()
    {
        playerexit = true;
    }
}