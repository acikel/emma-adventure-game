using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector3 mousePosition;
    private Vector2 mousePositionWorld2d;
    private Camera mainCamera;
    private RaycastHit2D[] raycastMainHit;
    private RaycastHit2D[] raycastPortalHit;
    private bool mouseDown;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    public void reasignCamera()
    {
        mainCamera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        
        

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("input manager mouse pressed");
            mouseDown = true;
            mousePosition = Input.mousePosition;
            mousePositionWorld2d = mainCamera.ScreenToWorldPoint(mousePosition);



            //Raycast 2D for item detection, object need 2d collider to be detected
            raycastMainHit = Physics2D.RaycastAll(mousePositionWorld2d, Vector2.zero);
            //Debug.Log("Raycasting" +raycastMainHit.Length);

            raycastPortalHit=Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            //above code line (raycastPortalHit=Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero)) for 3d:
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);
            */
        }
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("input manager mouse released");
            mouseDown = false;
        }
    }

    public RaycastHit2D[] getRaycastMainHitOnMouseDown()
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

    public bool wasCollidedPortalhit(Collider2D collidedPortal)
    {
        foreach(RaycastHit2D raycastCollider in raycastPortalHit)
        {
            if(raycastCollider.collider != null && raycastCollider.collider.gameObject.tag.Equals("Portal") && collidedPortal== raycastCollider.collider)
            {
                return true;
            }
        }
        return false;
    }
    public RaycastHit2D getRaycastCollider(string colliderTag)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycastCollider in raycastMainHit)
            {
                //Debug.Log("collider:"+raycastCollider.collider.name);
                if (raycastCollider.collider != null && raycastCollider.collider.gameObject.tag == colliderTag)
                {
                    return raycastCollider;
                }
            }
        }
        return new RaycastHit2D();
    }
    public RaycastHit2D getRaycastRigidbody(string rigidbodyTag)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycasRigidbody in raycastMainHit)
            {
                //Debug.Log("rigidbody:" + raycasRigidbody.rigidbody.name);
                if (raycasRigidbody.rigidbody != null && raycasRigidbody.rigidbody.gameObject.tag == rigidbodyTag)
                {
                    return raycasRigidbody;
                }
            }
        }
        return new RaycastHit2D();
    }


    public Rigidbody2D getHitRigidbody(string rigidbodyTag)
    {
        if (raycastMainHit != null)
        {
            foreach(RaycastHit2D raycastCollider in raycastMainHit)
            {
                if (raycastCollider.rigidbody != null && raycastCollider.rigidbody.gameObject.tag== rigidbodyTag)
                {
                    return raycastCollider.rigidbody;
                }
            }
        }
        return null;
    }

    public Collider2D getHitCollider(string colliderTag)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycastCollider in raycastMainHit)
            {
                if (raycastCollider.collider != null && raycastCollider.collider.gameObject.tag == colliderTag)
                {
                    return raycastCollider.collider;
                }
            }
        }
        return null;
    }

    public bool checkIfRigidbodyWasHit(string rigidbodyTag)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycastCollider in raycastMainHit)
            {
                if (raycastCollider.rigidbody != null &&  raycastCollider.rigidbody.gameObject.tag == rigidbodyTag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool checkIfColliderWasHit(string colliderTag)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycastCollider in raycastMainHit)
            {
                if (raycastCollider.collider != null &&  raycastCollider.collider.gameObject.tag == colliderTag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool checkIfSpecificColliderWasHit(string colliderTag, Collider2D collider)
    {
        if (raycastMainHit != null)
        {
            foreach (RaycastHit2D raycastCollider in raycastMainHit)
            {
                if (raycastCollider.collider != null && raycastCollider.collider.gameObject.tag == colliderTag && raycastCollider.collider==collider)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
