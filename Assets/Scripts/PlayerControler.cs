using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{

    private Vector3 targetPosition;
    private bool isMoving;
    private Avatar avatar;
    private RaycastHit2D raycastSecondayHit;
    private Vector2 mousePositionWorld2d;

    private RaycastHit2D raycastHit;

    private GameManager GM;
    private InputManager inputManager;

    //lerp parameters:
    private float lerpDurationNear;
    private float lerpDurationFar;
    private float currentLerpTime;
    private bool idleTriggered;
    private bool walkTriggered;
    private float lerpDuration;
    private float stopDistance;
    private float avatarDistanceToHorizont;
    private float maxDistance;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            triggerIdleAnimation();
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            triggerIdleAnimation();
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        GM = GameManager.Instance;
        GM.OnControllerChange += HandleOnControllerChange;
        avatar = GameManager.currentAvatar;

        idleTriggered = true;
        lerpDurationNear = avatar.lerpDuration;
        lerpDurationFar = avatar.lerpDuration + avatar.lerpDistanceFarSummand;
        lerpDuration = avatar.lerpDuration;
        stopDistance = avatar.stopDistance;
        inputManager = API.InputManager;

    }

    private void OnDisable()
    {
        GM.OnControllerChange -= HandleOnControllerChange;
    }
    // Update is called once per frame
    void Update()
    {

        if (inputManager.isMouseDown())
        {
            if ((raycastHit= inputManager.getRaycastRigidbody("Player")).rigidbody!=null )
            {
                if(raycastHit.rigidbody.gameObject != GameManager.currentAvatar.gameObject && !isMoving)
                {
                    GM.ChangeController(GameManager.playerAvatar, GameManager.helperAvatar);
                }  
            }
            //else if (inputManager.getRaycastMainHitOnMouseDown().rigidbody != null && inputManager.getRaycastMainHitOnMouseDown().rigidbody.tag == "Helper")
            
            else if ((raycastHit = inputManager.getRaycastRigidbody("Helper")).rigidbody != null)
            {
                if(raycastHit.rigidbody.gameObject != GameManager.currentAvatar.gameObject && !isMoving)
                {
                    GM.ChangeController(GameManager.helperAvatar, GameManager.playerAvatar);
                }
            }
            else if (inputManager.checkIfColliderWasHit("SelectableItem"))
            {

            }
            else if (inputManager.checkIfColliderWasHit("Ground"))
            {
                targetPosition = inputManager.getRaycastCollider("Ground").point;
                triggerWalkAnimation();
                initializeLerp();
                isMoving = true;

                //Check if sprite flip needed:
                CheckSpriteFlip();
            }
            else
            {
                //print("name3: " + raycastFirstHit.collider);
                if (getGroundColliderIntersectionToMouseclickOutsideGround())
                {
                    //move player infront of item.
                    if (inputManager.checkIfColliderWasHit("Item"))
                        targetPosition.x -= 1;
                    
                    triggerWalkAnimation();
                    initializeLerp();
                    isMoving = true;

                    //Check if sprite flip needed:
                    CheckSpriteFlip();
                }
            }
        }
    }


    //Use fixedUpdate for equal moving speed on every computer, unattached to the framerate it can display
    private void FixedUpdate()
    {
        if (isMoving)
        {
            Lerp();
            scaleCharachter();
        }
    }


    private void scaleCharachter()
    {
        avatarDistanceToHorizont = GameManager.backgroundCollider.bounds.max.y - avatar.gameObject.transform.position.y;
        maxDistance = GameManager.backgroundCollider.bounds.max.y - GameManager.backgroundCollider.bounds.min.y;
        avatar.gameObject.transform.localScale = avatar.getLocalScale() + avatarDistanceToHorizont / maxDistance * Vector3.one * avatar.scalingFactor;
    }

    private void HandleOnControllerChange()
    {
        avatar = GameManager.currentAvatar;
        lerpDuration = avatar.lerpDuration;
        stopDistance = avatar.stopDistance;
    }

    private void triggerWalkAnimation()
    {
        walkTriggered = true;
        idleTriggered = false;
        if (avatar.avatarAnimator != null)
        {
            avatar.avatarAnimator.SetTrigger("Walk");
        }
    }

    private void triggerIdleAnimation()
    {
        isMoving = false;
        idleTriggered = true;
        walkTriggered = false;
        if (avatar.avatarAnimator != null)
        {
            avatar.avatarAnimator.SetTrigger("Idle");
        }
    }

    private void CheckSpriteFlip()
    {
        if (avatar.transform.position.x > targetPosition.x)
        {
            //look to the left
            avatar.avatarSpriteRenderer.flipX = false;
        }
        else
        {
            //look to the right
            avatar.avatarSpriteRenderer.flipX = true;
        }
    }
    private bool getGroundColliderIntersectionToMouseclickOutsideGround()
    {
        mousePositionWorld2d = inputManager.getMousePositionWorld2d();
        //Only hits ground layers. Searches for Ground collider from above
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from below
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.up, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from right
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.right, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from left
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.right, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        if (GameManager.groundCenter != null)
        {
            Vector2 groundCenter2d = GameManager.groundCenter.position;
            Vector2 direction = groundCenter2d - mousePositionWorld2d;
            raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.right, 1 << LayerMask.NameToLayer("Ground"));
            if (assignColliderAndExit()) return true;

        }

        return false;

    }
    private bool assignColliderAndExit()
    {
        if (raycastSecondayHit.collider != null)
        {
            targetPosition = raycastSecondayHit.point;
            return true;
        }
        return false;
    }

    private void initializeLerp()
    {
        currentLerpTime = 0f;
        if (Vector2.Distance(avatar.transform.position, targetPosition) < 7)
        {
            lerpDuration = lerpDurationNear;
            stopDistance = avatar.stopDistance;
        }
        else
        {
            lerpDuration = lerpDurationFar;
            stopDistance = avatar.stopDistanceFar;
        }
    }

    private void Lerp()
    {
        currentLerpTime += Time.fixedDeltaTime;
        if (currentLerpTime > lerpDuration)
        {
            currentLerpTime = lerpDuration;
        }

        //lerp!
        float t = currentLerpTime / lerpDuration; 

        if (t >= avatar.startWalkDelay && t < stopDistance && !walkTriggered)
        {
            triggerWalkAnimation();
        }
        if (t >= stopDistance && !idleTriggered)
        {
            triggerIdleAnimation();
        }

        avatar.getRigidbody2D().velocity = Vector2.one * 0.001f;
        targetPosition.z = avatar.transform.position.z;
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, targetPosition, t);
    }

}
