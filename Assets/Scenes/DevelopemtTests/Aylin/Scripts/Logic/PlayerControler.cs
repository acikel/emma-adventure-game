using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public Collider2D colliderOfAvatarPlayer;
    public Collider2D colliderOfAvatarHelper;
    private Collider2D colliderOfAvatarCurrent;
    private Vector3 targetPosition;
    private bool isMoving;
    private Avatar avatar;
    private RaycastHit2D raycastSecondayHit;
    private Vector2 mousePositionWorld2d;

    private RaycastHit2D raycastHit;

    private AvatarManager avatarManager;
    private InputManager inputManager;
    private Inventory inventory;
    private SceneManager sceneManager;

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

    private bool avatarIsCollidingWithObstacle;
    private Vector3 avatarPreviousPosition;

    //used to flip player with its colliders. avatar.avatarSpriteRenderer.flipX only turns sprite but not the collider
    private Vector3 LocalScaleLeft = new Vector3(1f, 1f, 1f);
    private Vector3 LocalScaleRight = new Vector3(-1f, 1f, 1f);

    //subscribed by sceneManager
    public delegate IEnumerator OnCollisionWithPortalHandler(string sceneNameToTransitionTo);
    public static event OnCollisionWithPortalHandler OnCollisionWithPortal;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("PlayerController OnCollisionEnter2D" + collision.gameObject.name);
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            triggerIdleAnimation();
        //}else if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading && inputManager.checkIfColliderWasHit("Portal"))
        }
        else if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading)
        {
            triggerIdleAnimation();
            //Debug.Log("Portal name:"+ collision.gameObject.name);

            //replacement of OnCollisionWithPortal(collision.gameObject.name); to call events with IEnumerator as return type and Coroutines in Handler Methods:
            if (OnCollisionWithPortal != null)
            {
                for (int n = OnCollisionWithPortal.GetInvocationList().Length - 1; n >= 0; n--)
                {
                    OnCollisionWithPortalHandler onCollisionWithPortalCoroutine = OnCollisionWithPortal.GetInvocationList()[n] as OnCollisionWithPortalHandler;
                    StartCoroutine(onCollisionWithPortalCoroutine(collision.gameObject.name));
                }
            }

            
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            triggerIdleAnimation();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        resetTriggerIdleAnimation();
    }

    //Collision with obstacles need to be treated in OnTrigger instead of OnCollision as the obstacle colliders are triggers.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (avatarManager.checkForCollisionWithObstacles(colliderOfAvatarCurrent))
        {
            triggerIdleAnimation();
            avatarIsCollidingWithObstacle = true;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (avatarManager.checkForCollisionWithObstacles(colliderOfAvatarCurrent))
        {
            avatar.transform.position = avatarPreviousPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        avatarIsCollidingWithObstacle = false;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("avatarManager1:" + avatarManager);
        
        avatarManager = API.AvatarManager;
        avatarManager.OnControllerChange += HandleOnControllerChange;
        avatar = AvatarManager.currentAvatar;
        idleTriggered = true;
        lerpDurationNear = avatar.lerpDuration;
        lerpDurationFar = avatar.lerpDuration + avatar.lerpDistanceFarSummand;
        lerpDuration = avatar.lerpDuration;
        stopDistance = avatar.stopDistance;
        inputManager = API.InputManager;

        inventory = API.Inventory;
        sceneManager = API.SceneManager;
        sceneManager.AfterAvatarInitialization += initializeAndRescalePlayer;
        scaleCharachter();
        colliderOfAvatarCurrent = colliderOfAvatarPlayer;
    }

    private void OnDisable()
    {
        avatarManager.OnControllerChange -= HandleOnControllerChange;
        sceneManager.AfterAvatarInitialization -= initializeAndRescalePlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (!avatarIsCollidingWithObstacle)
            avatarPreviousPosition = avatar.transform.position;
       
        

        //Debug.Log("in inventory "+inventory.InteractionWithInventoryActive);
        if (!inventory.InteractionWithUIActive && inputManager.isMouseDown())
        {
            if ((raycastHit= inputManager.getRaycastRigidbody("Player")).rigidbody!=null )
            {
                if(raycastHit.rigidbody.gameObject != AvatarManager.currentAvatar.gameObject && !isMoving)
                {
                    avatarManager.ChangeController(AvatarManager.playerAvatar, AvatarManager.helperAvatar);
                    colliderOfAvatarCurrent = colliderOfAvatarPlayer;
                }  
            }
            //else if (inputManager.getRaycastMainHitOnMouseDown().rigidbody != null && inputManager.getRaycastMainHitOnMouseDown().rigidbody.tag == "Helper")

            /*//TODO UNCOMMENT TO GET CONTROL OVER HELPER BY CLICKING ON HIM
            else if ((raycastHit = inputManager.getRaycastRigidbody("Helper")).rigidbody != null)
            {
                if(raycastHit.rigidbody.gameObject != AvatarManager.currentAvatar.gameObject && !isMoving)
                {
                    avatarManager.ChangeController(AvatarManager.helperAvatar, AvatarManager.playerAvatar);
                    colliderOfAvatarCurrent = colliderOfAvatarHelper;
                }
            }
            */
            else if (inputManager.checkIfColliderWasHit("SelectedItem"))
            {

            }
            else if (inputManager.checkIfColliderWasHit("Ground"))
            {
                targetPosition = inputManager.getRaycastCollider("Ground").point;
                //triggerWalkAnimation();
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
                    
                    //triggerWalkAnimation();
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


    public void initializeAndRescalePlayer(float avatarStartScale, float avatarScaleFactor)
    {
        //Debug.Log("avatarStartScale: " + avatarStartScale);
        //Debug.Log("avatarScaleFactor: " + avatarScaleFactor);
        initializePlayerAvatar(AvatarManager.playerAvatar, avatarStartScale, avatarScaleFactor);
        initializePlayerAvatar(AvatarManager.helperAvatar, avatarStartScale, avatarScaleFactor);
        scaleCharachter(AvatarManager.playerAvatar);
        scaleCharachter(AvatarManager.helperAvatar);
    }

    private void initializePlayerAvatar(Avatar avatar, float avatarStartScale, float avatarScaleFactor)
    {
        avatar.setLocalScale(Vector3.one * avatarStartScale);
        avatar.scalingFactor = avatarScaleFactor;
    }

    private void scaleCharachter()
    {
        scaleCharachter(this.avatar);
    }

    private void scaleCharachter(Avatar avatar)
    {
        //Debug.Log("collider ground: " + AvatarManager.backgroundCollider.bounds);
        //Debug.Log("avatar: " + avatar.gameObject.transform);
        avatarDistanceToHorizont = AvatarManager.backgroundCollider.bounds.max.y - avatar.gameObject.transform.position.y;
        maxDistance = AvatarManager.backgroundCollider.bounds.max.y - AvatarManager.backgroundCollider.bounds.min.y;
        avatar.gameObject.transform.localScale = avatar.getLocalScale() + avatarDistanceToHorizont / maxDistance * Vector3.one * avatar.scalingFactor;
    }

    private void HandleOnControllerChange()
    {
        avatar = AvatarManager.currentAvatar;
        lerpDuration = avatar.lerpDuration;
        stopDistance = avatar.stopDistance;
    }

    private void resetTriggerIdleAnimation()
    {
        if (avatar.avatarAnimator != null)
        {
            avatar.avatarAnimator.ResetTrigger("Idle");
        }
        
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
            //avatar.gameObject.transform.localScale = sceneManager.getCurrentSceneValues().avatarStartScale* LocalScaleLeft;
        }
        else
        {
            //look to the right
            avatar.avatarSpriteRenderer.flipX = true;
            //localScaleAvatar.x *= -1;
            //avatar.gameObject.transform.localScale = localScaleAvatar;
            //avatar.gameObject.transform.localScale = sceneManager.getCurrentSceneValues().avatarStartScale * LocalScaleRight;
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

        if (AvatarManager.groundCenter != null)
        {
            Vector2 groundCenter2d = AvatarManager.groundCenter.position;
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
        //Debug.Log("distance: " + Vector2.Distance(avatar.transform.position, targetPosition));
        if (Vector2.Distance(avatar.transform.position, targetPosition) < 60)
        {
            lerpDuration = lerpDurationNear;
            stopDistance = avatar.stopDistance;
        }
        else
        {
            lerpDuration = lerpDurationFar;
            stopDistance = avatar.stopDistanceFar;
        }
        //Debug.Log("lerpduration: "+ lerpDuration+ " lerp near: "+lerpDurationNear+ " lerp duration far: "+lerpDurationFar);
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

        if (avatarIsCollidingWithObstacle)
        {
            avatar.transform.position = avatarPreviousPosition;
        }else
        if (t >= avatar.startWalkDelay && t < stopDistance && !walkTriggered)
        {
            triggerWalkAnimation();
        }
        else if (t >= stopDistance && !idleTriggered)
        {
            triggerIdleAnimation();
        }

        avatar.getRigidbody2D().velocity = Vector2.one * 0.001f;
        targetPosition.z = avatar.transform.position.z;
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, targetPosition, t);


    }

}
