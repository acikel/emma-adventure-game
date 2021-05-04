using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector3 mousePosition;
    private Vector2 mousePositionWorld2d;
    private Camera mainCamera;
    private RaycastHit2D raycastMainHit;
    private bool mouseDown;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            mousePosition = Input.mousePosition;
            mousePositionWorld2d = mainCamera.ScreenToWorldPoint(mousePosition);



            //Raycast 2D for item detection, object need 2d collider to be detected
            raycastMainHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.zero);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
        }
    }

    public RaycastHit2D getRaycastMainHitOnMouseDown()
    {
        return raycastMainHit;
    }

    public Vector2 getMousePositionWorld2d()
    {
        return mousePositionWorld2d;
    }

    public bool isMouseDown()
    {
        return mouseDown;
    }
}
