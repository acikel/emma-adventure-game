using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class PlayerControler : MonoBehaviour
{
    public Collider2D colliderOfAvatarPlayer;
    public Collider2D colliderOfAvatarHelper;

    private FMOD.Studio.EventInstance footstepsEvent;
    private FMOD.Studio.EventDescription eventDescription;
    [FMODUnity.EventRef]
    public string footstepsSound;
    [FMODUnity.EventRef]
    public string doorHandleSound;
    [FMODUnity.EventRef]
    public string stairsSound;

    private Collider2D colliderOfAvatarCurrent;
    private Vector3 targetPosition;
    private bool isMoving;
    private Avatar avatar;
    private RaycastHit2D raycastSecondayHit;
    private RaycastHit2D raycastSecondayHitCompare;

    private Vector2 mousePositionWorld2d;
    private Vector3 directionAvatar;

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
    //private Vector3 LocalScaleLeft = new Vector3(1f, 1f, 1f);
    //private Vector3 LocalScaleRight =  new Vector3(-1f, 1f, 1f);
    private float flipLeft = 1f;
    private float flipRight = -1f;
    private float currentXScaleTmp;

    //subscribed by sceneManager
    public delegate IEnumerator OnCollisionWithPortalHandler(string sceneNameToTransitionTo);
    public static event OnCollisionWithPortalHandler OnCollisionWithPortal;


    private float speed = 400f;
    //How near AI needs to be to a waypoint till it moves on to the next one.
    private float nextWaypointDistance = 3f;

    //used to flip the helper into the direction it is travelling. This variable schould point to the graphics (like spriterenderer and so on) object of this gameobject or if everything is in one object to the transform of this game obejct.
    //public Transform avatarTransform;

    //current followed path
    private Path path;
    //Stores current waypoint along the path path we are targeting.
    private int currentWayPoint = 0;
    //tells if we have reached the end of our path
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;
    //used to stop AI if it gets stuck:
    //saves the collider which collided with player trigger (capsule) first intead with player collider (circle)
    private Collider2D currentTriggerCollider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("PlayerController OnCollisionEnter2D" + collision.gameObject.name);

        if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading )
        {
            if (inputManager.wasCollidedPortalhit(collision.gameObject.GetComponent<Collider2D>()))
            {
                if (collision.gameObject.name.Contains("Door"))
                    FMODUnity.RuntimeManager.PlayOneShot(doorHandleSound);
                else if (collision.gameObject.name.Contains("Stairs"))
                    FMODUnity.RuntimeManager.PlayOneShot(stairsSound);

                stopAvatar();
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
            else
            {
                stopPlayerAndPushBackToPrevoiusPosition();
            }

        }
        /*else if(collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player" || collision.gameObject.layer.Equals( "Foreground")) //not needed anymore because helper and player are triggers if deactivated for the AI script
        {
            Debug.Log("collision enter");
            resetTriggerWalkAnimation();
            triggerIdleAnimation();
            stopSound(footstepsEvent);
            avatarIsCollidingWithObstacle = true;
            isMoving = false;
            //directionPlayer = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
            //avatar.transform.position = avatarPreviousPosition + directionPlayer * 0.01f;
            //triggerIdleAnimation();
            //stopSound(footstepsEvent);
            //}else if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading && inputManager.checkIfColliderWasHit("Portal"))
        }*/

        /*if(collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            triggerIdleAnimation();
            stopSound(footstepsEvent);
            avatarIsCollidingWithObstacle = true;
            directionPlayer = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
            avatar.transform.position = avatarPreviousPosition + directionPlayer * 0.01f;
            //triggerIdleAnimation();
            //stopSound(footstepsEvent);
            //}else if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading && inputManager.checkIfColliderWasHit("Portal"))
        }*/



    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        /*if (collision.gameObject.tag == "Portal" && !sceneManager.IsReloading)
        {
            
            if (inputManager.wasCollidedPortalhit(collision.gameObject.GetComponent<Collider2D>()))
            {
                if (collision.gameObject.name.Contains("Door"))
                    FMODUnity.RuntimeManager.PlayOneShot(doorHandleSound);
                else if (collision.gameObject.name.Contains("Stairs"))
                    FMODUnity.RuntimeManager.PlayOneShot(stairsSound);

                stopAvatar();
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
            else
            {
                stopPlayerAndPushBackToPrevoiusPosition();
            }

        }*/
        //if (collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        //{
        //triggerIdleAnimation();
        //stopSound(footstepsEvent);
        //resetTriggerWalkAnimation();
        //triggerIdleAnimation();
        //isMoving = false;
        //stopSound(footstepsEvent);
        //directionPlayer = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
        //avatar.transform.position = avatarPreviousPosition + directionPlayer * 0.05f;
        //}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        resetTriggerIdleAnimation();
        //if (collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        //{
        avatarIsCollidingWithObstacle = false;
        //}
    }

    //Collision with obstacles need to be treated in OnTrigger instead of OnCollision as the obstacle colliders are triggers.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("trigger enter");
        currentTriggerCollider = collision;

        if (colliderOfAvatarCurrent.IsTouching(currentTriggerCollider) && (collision.gameObject.layer.Equals("Foreground") || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player"))
        {
            avatarIsCollidingWithObstacle = true;
            //Debug.Log("trigger stay2");
            if (colliderOfAvatarCurrent.IsTouching(collision))
            {
                //Debug.Log("trigger stay3");
                stopPlayerAndPushBackToPrevoiusPosition();
            }

        }
        /*
        if (collision.gameObject.layer.Equals("Foreground") || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player")
        {
            if (colliderOfAvatarCurrent.IsTouching(collision))
            {
                resetTriggerWalkAnimation();
                triggerIdleAnimation();
                stopSound(footstepsEvent);
                isMoving = false;
            }
           
        }
           */

        /*if (avatarManager.checkForCollisionWithObstacles(colliderOfAvatarCurrent))
        {
            triggerIdleAnimation();
            stopSound(footstepsEvent);
            avatarIsCollidingWithObstacle = true;
            directionPlayer = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
            avatar.transform.position = avatarPreviousPosition + directionPlayer * 0.05f;
        }*/

    }

    private void stopPlayerAndPushBackToPrevoiusPosition()
    {
        resetTriggerWalkAnimation();
        triggerIdleAnimation();
        stopSound(footstepsEvent);
        //directionAvatar = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
        //avatar.transform.position = avatarPreviousPosition + directionAvatar * 0.05f;
        //avatar.transform.position = avatar.transform.position - directionAvatar * 0.05f;
        avatar.transform.position = avatarPreviousPosition - directionAvatar * 0.05f;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("trigger stay1");
        currentTriggerCollider = collision;
        if (colliderOfAvatarCurrent.IsTouching(currentTriggerCollider) && (collision.gameObject.layer.Equals("Foreground") || collision.gameObject.tag == "Helper" || collision.gameObject.tag == "Player"))
        {
            //Debug.Log("trigger stay2");
            if (colliderOfAvatarCurrent.IsTouching(collision))
            {
                //Debug.Log("trigger stay3");
                //resetTriggerWalkAnimation();
                //triggerIdleAnimation();
                //stopSound(footstepsEvent);
                //directionAvatar = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
                //avatar.transform.position = avatarPreviousPosition + directionAvatar * 0.05f;
                //avatar.transform.position = avatar.transform.position - directionAvatar * 0.05f;
                //resetTriggerWalkAnimation();
                //triggerIdleAnimation();
                //stopSound(footstepsEvent);
                //avatar.transform.position = avatarPreviousPosition - directionAvatar * 0.05f; //needs to be previous position avatarPreviousPosition instead of current position avatar.transform.position otherwise the player slides as it changes it position as its pushed back by this line and triggerStay2D is applying it each time.
                stopPlayerAndPushBackToPrevoiusPosition();
            }

        }

        /*if (avatarManager.checkForCollisionWithObstacles(colliderOfAvatarCurrent))
        {
            stopSound(footstepsEvent);
            directionPlayer = targetPosition - new Vector3(mousePositionWorld2d.x, mousePositionWorld2d.y, targetPosition.z);
            avatar.transform.position = avatarPreviousPosition + directionPlayer * 0.05f;
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("trigger exit");
        currentTriggerCollider = null;
        //resetTriggerIdleAnimation();
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
        //scaleCharachter();
        colliderOfAvatarCurrent = colliderOfAvatarPlayer;

        //footstepsEvent = FMOD. FMOD_StudioSystem.instance.GetEvent(footstepsSound);
        eventDescription = FMODUnity.RuntimeManager.GetEventDescription(footstepsSound);
        eventDescription.createInstance(out footstepsEvent);

        //scale character with current values of scene manager for first loaded scene
        if (sceneManager.CurrentSceneValues != null) //is case for start menu as avatars are not displayed
            initializeAndRescalePlayer(sceneManager.CurrentSceneValues.avatarStartScale, sceneManager.CurrentSceneValues.avatarScaleFactor);

        seeker = avatar.seeker;
        rb = avatar.getRigidbody2D();
        speed = avatar.speed;
        nextWaypointDistance = avatar.nextWaypointDistance;

    }

    private void OnDisable()
    {
        avatarManager.OnControllerChange -= HandleOnControllerChange;
        sceneManager.AfterAvatarInitialization -= initializeAndRescalePlayer;
        releaseSound(footstepsEvent);
    }



    // Update is called once per frame
    void Update()
    {
        if (!avatarIsCollidingWithObstacle)
            avatarPreviousPosition = avatar.transform.position;



        //Debug.Log("in inventory "+inventory.InteractionWithInventoryActive);
        if (!inventory.InteractionWithUIActive && inputManager.isMouseDown() && !sceneManager.IsReloading)
        {
            if ((raycastHit = inputManager.getRaycastRigidbody("Player")).rigidbody != null)
            {
                if (raycastHit.rigidbody.gameObject != AvatarManager.currentAvatar.gameObject && !isMoving)
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
                resetTriggerIdleAnimation();
                triggerWalkAnimation();

                //Check if sprite flip needed:
                CheckAvatarFlip();

                UpdatePath();
            }
            else
            {
                //print("name3: " + raycastFirstHit.collider);
                if (getGroundColliderIntersectionToMouseclickOutsideGroundWithSmallestDistance())
                {
                    //move player infront of item.
                    if (inputManager.checkIfColliderWasHit("Item"))
                        targetPosition.x -= 1;

                    //triggerWalkAnimation();
                    initializeLerp();
                    isMoving = true;
                    resetTriggerIdleAnimation();
                    triggerWalkAnimation();

                    //Check if sprite flip needed:
                    CheckAvatarFlip();
                    UpdatePath();
                }
            }
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone()) //checking if we are currently updating our graph. As we only want to create a new path if no other path is beeing calculated.
            seeker.StartPath(rb.position, targetPosition, OnPathComplete);
    }
    //p is the generated path by seeker.StartPath(rb.position, target.position, OnPathComplete); (used in the start function).
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            //set our path to the generated path
            path = p;
            //reset progress along our path (to start the new path from the beginning).
            currentWayPoint = 0;
            isMoving = true;
            resetTriggerIdleAnimation();
            triggerWalkAnimation();

            //Debug.Log("OnPathComplete entered");
        }
    }

    //Use fixedUpdate for equal moving speed on every computer, unattached to the framerate it can display
    private void FixedUpdate()
    {
        if (isMoving)
        {
            //Lerp();
            if (sceneManager.IsReloading)
            {
                stopAvatar();
                return;
            }
                

            scaleCharachter();

            if (path == null) //no path currently exists.
                return;
            //if(currentTriggerCollider!=null)
            //Debug.Log("FixedUpdate5: "+ currentTriggerCollider + " "+ colliderOfAvatarCurrent.IsTouching(currentTriggerCollider));
            if (currentWayPoint >= path.vectorPath.Count /*|| (currentTriggerCollider!=null && colliderOfAvatarCurrent.IsTouching(currentTriggerCollider) && !currentTriggerCollider.gameObject.layer.Equals("Ground") && !currentTriggerCollider.gameObject.layer.Equals("Obstacle") && !currentTriggerCollider.gameObject.name.Contains("DropOff"))*/)//check if we havent reached the end the path. If we reached it we want to stop moving. path.vectorPath.Count describes the total amount of waypoint on our path.
            {
                //Debug.Log("FixedUpdate entered1");
                stopAvatar();

                return;
            }
            else
            {
                //Debug.Log("FixedUpdate entered2");
                reachedEndOfPath = false;
                footstepsEvent.start();
                //if (avatar.avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                //triggerWalkAnimation();

            }
            //Debug.Log("FixedUpdate entered3");
            //move helper:
            //get direction to the next waypoint along our path:
            //Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;//path.vectorPath[currentWayPoint] finds current waypoint along our path. path.vectorPath[currentWayPoint] - rb.position gives a vector that points from current position rb.position to the next waypoint path.vectorPath[currentWayPoint]. We normilize the direction to have a vector with length 1.

            directionAvatar = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;//path.vectorPath[currentWayPoint] finds current waypoint along our path. path.vectorPath[currentWayPoint] - rb.position gives a vector that points from current position rb.position to the next waypoint path.vectorPath[currentWayPoint]. We normilize the direction to have a vector with length 1.
            
            //Apply force to move helper into the direction direction://Apply force to move helper into the direction direction:
            Vector2 force = directionAvatar * speed * Time.deltaTime;

            //adding force to helper:
            rb.AddForce(force);

            //Distance to our next waypoint:
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
            //Check if current waypoint was reached and move on to the next one:
            if (distance < nextWaypointDistance)
            {
                currentWayPoint++;
            }
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

    private void stopAvatar()
    {
        reachedEndOfPath = true;
        isMoving = false;
        stopSound(footstepsEvent);
        currentWayPoint = path.vectorPath.Count;
        //if(avatar.avatarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        resetTriggerWalkAnimation();
        //avatar.avatarAnimator.enabled = false;
        //avatar.avatarAnimator.enabled = true;
        triggerIdleAnimation();
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

    public void scaleAvatar(Avatar avatar)
    {
        scaleCharachter(avatar);
    }

    private void scaleCharachter(Avatar avatar)
    {
        //Debug.Log("collider ground: " + AvatarManager.backgroundCollider.bounds);
        //Debug.Log("avatar: " + avatar.gameObject.transform);
        avatarDistanceToHorizont = AvatarManager.backgroundCollider.bounds.max.y - avatar.gameObject.transform.position.y;
        maxDistance = AvatarManager.backgroundCollider.bounds.max.y - AvatarManager.backgroundCollider.bounds.min.y;
        avatar.gameObject.transform.localScale = avatar.getLocalScale() + avatarDistanceToHorizont / maxDistance * Vector3.one * avatar.scalingFactor;

        //flip direction if CheckSpriteFlip defined that avatar need to be flipped. avatar.avatarSpriteRenderer.flipX cant be used as it only flips the sprite not the colliders.
        //Thats why avatars with colliders need to be flipped with their localScale instead. As this controller changes the size of the avatar depending on background depth it we cant just change the current local scale but need to conserve the current local state and multiply it with the right value to flip the scaled carachter. 
        currentXScaleTmp = avatar.gameObject.transform.localScale.x;
        //Debug.Log("avatar.gameObject.transform.localScale.x "+avatar.name+" " + avatar.gameObject.transform.localScale.x);
        //Debug.Log("currentXScaleTmp1 " + avatar.name + " " + currentXScaleTmp);
        //Debug.Log("avatar.CurrentFlipDirection " + avatar.name + " " + avatar.getFlipDirection());
        currentXScaleTmp *= avatar.getFlipDirection();
        //Debug.Log("currentXScaleTmp2 " + avatar.name + " " + currentXScaleTmp);
        avatar.gameObject.transform.localScale = new Vector3(currentXScaleTmp, avatar.gameObject.transform.localScale.y, avatar.gameObject.transform.localScale.z);
    }

    private void HandleOnControllerChange()
    {
        avatar = AvatarManager.currentAvatar;
        lerpDuration = avatar.lerpDuration;
        stopDistance = avatar.stopDistance;

        seeker = avatar.seeker;
        rb = avatar.getRigidbody2D();
        speed = avatar.speed;
        nextWaypointDistance = avatar.nextWaypointDistance;
    }

    private void resetTriggerIdleAnimation()
    {
        if (avatar.avatarAnimator != null)
        {
            avatar.avatarAnimator.ResetTrigger("Idle");
        }
        
    }

    private void resetTriggerWalkAnimation()
    {
        if (avatar.avatarAnimator != null)
        {
            avatar.avatarAnimator.ResetTrigger("Walk");
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


    private void CheckAvatarFlip()
    {
        if (avatar.transform.position.x > targetPosition.x)
        {
            //look to the left
            //avatar.avatarSpriteRenderer.flipX = false;


            //avatar.gameObject.transform.localScale = sceneManager.getCurrentSceneValues().avatarStartScale* LocalScaleLeft;


            avatar.setFlipDirection(flipLeft);
        }
        else
        {
            //look to the right
            //avatar.avatarSpriteRenderer.flipX = true;
            //localScaleAvatar.x *= -1;


            //avatar.gameObject.transform.localScale = localScaleAvatar;
            //avatar.gameObject.transform.localScale = sceneManager.getCurrentSceneValues().avatarStartScale * LocalScaleRight;


            avatar.setFlipDirection(flipRight);
        }
    }
    private bool getGroundColliderIntersectionToMouseclickOutsideGroundWithoutSmallestDistance()
    {
        mousePositionWorld2d = inputManager.getMousePositionWorld2d();
        //Only hits ground layers. Searches for Ground collider from above
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from below
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.up, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from right.
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.right, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        if (assignColliderAndExit()) return true;

        //Raycast to left not needed. Causes problems with center raycast (for example for the background collider in scene sequence1zone5). Better to raycast to center then.
        //Only hits ground layers. Searches for Ground collider from left
        //raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.left, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        //if (assignColliderAndExit()) return true;

        if (AvatarManager.groundCenter != null)
        {
            //Debug.Log("GroundCenter");
            Vector2 groundCenter2d = AvatarManager.groundCenter.position;
            Vector2 direction = groundCenter2d - mousePositionWorld2d;
            raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, direction, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
            if (assignColliderAndExit()) return true;

        }

        return false;

    }


    private bool getGroundColliderIntersectionToMouseclickOutsideGroundWithSmallestDistance()
    {
        mousePositionWorld2d = inputManager.getMousePositionWorld2d();
        //Only hits ground layers. Searches for Ground collider from above
        raycastSecondayHit = Physics2D.Raycast(mousePositionWorld2d, Vector2.down, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        //if (assignColliderAndExit()) return true;

        //Only hits ground layers. Searches for Ground collider from below
        rayCastAndGetSmallestDistance(Vector2.up);

        //Only hits ground layers. Searches for Ground collider from right.
        rayCastAndGetSmallestDistance(Vector2.right);

        //Only hits ground layers. Searches for Ground collider from left
        rayCastAndGetSmallestDistance(Vector2.left);

        if (AvatarManager.groundCenter != null)
        {
            //Debug.Log("GroundCenter");
            Vector2 groundCenter2d = AvatarManager.groundCenter.position;
            Vector2 direction = groundCenter2d - mousePositionWorld2d;
            rayCastAndGetSmallestDistance(direction);
        }


        if (assignColliderAndExit()) return true;
        return false;

    }

    private void rayCastAndGetSmallestDistance(Vector2 direction)
    {
        raycastSecondayHitCompare = Physics2D.Raycast(mousePositionWorld2d, direction, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Ground"));
        //if (assignColliderAndExit()) return true;
        raycastSecondayHit = getRaycastWithSmallestDistance(raycastSecondayHit, raycastSecondayHitCompare, mousePositionWorld2d);
    }

    private RaycastHit2D getRaycastWithSmallestDistance(RaycastHit2D raycast1, RaycastHit2D raycast2, Vector2 targetPosition)
    {
        if (raycast1.collider == null)
            return raycast2;

        if (raycast2.collider == null)
            return raycast1;

        if (Vector2.Distance(raycast1.point,targetPosition)> Vector2.Distance(raycast2.point, targetPosition))
        {
            return raycast2;
        }
        else
        {
            return raycast1;
        }
    }
    private bool assignColliderAndExit()
    {
        if (raycastSecondayHit.collider != null)
        {
            targetPosition = raycastSecondayHit.point;
            return true;
        }
        //Debug.Log("assignColliderAndExit false");
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

        if (path == null) //no path currently exists.
            return;
        if (currentWayPoint >= path.vectorPath.Count)//check if we havent reached the end the path. If we reached it we want to stop moving. path.vectorPath.Count describes the total amount of waypoint on our path.
        {
            reachedEndOfPath = true;
            stopSound(footstepsEvent);

            triggerIdleAnimation();
            
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        currentLerpTime += Time.fixedDeltaTime;
        if (currentLerpTime > lerpDuration)
        {
            currentLerpTime = lerpDuration;
        }

        //lerp!
        float t = currentLerpTime / lerpDuration;
        //faster lerp:
        //t= Mathf.Sin(t * Mathf.PI * 0.5f); // best movement parameters: lerp duration: 20, lerp distance far summand: 60, start walk delay: 0, stop distance far: 0.05, stop distance: 0.085
        t = t * Mathf.PI * 0.5f; //best movement parameters: lerp duration: 30, lerp distance far summand: 80, start walk delay: 0.001, stop distance far: 0.043, stop distance: 0.07
        if (avatarIsCollidingWithObstacle)
        {
            avatar.transform.position = avatarPreviousPosition;
        }else if (t >= avatar.startWalkDelay && t < stopDistance && !walkTriggered)
        {
            footstepsEvent.start();
            triggerWalkAnimation();
        }
        else if (t >= stopDistance && !idleTriggered)
        {
            currentWayPoint++;
        }
        avatar.getRigidbody2D().velocity = Vector2.one * 0.001f;
        targetPosition = path.vectorPath[currentWayPoint];
        targetPosition.z = avatar.transform.position.z;
        avatar.transform.position = Vector3.Lerp(avatar.transform.position, targetPosition, t);


    }

    private void stopSound(FMOD.Studio.EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //instance.release();
        //instance.clearHandle();
    }

    private void releaseSound(FMOD.Studio.EventInstance instance)
    {
        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
        instance.clearHandle();
    }
}
