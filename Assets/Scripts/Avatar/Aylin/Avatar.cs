using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Avatar : MonoBehaviour
{
    public SpriteRenderer avatarSpriteRenderer;
    public Animator avatarAnimator;
    //used for near stop distance
    public float stopDistance = 0.1f;
    public float stopDistanceFar = 0.055f;
    //used for near and far walk distance
    public float startWalkDelay = 0.0001f;
    [SerializeField]
    public float lerpDistanceFarSummand = 40f;
    //used for ai movement in PlayerController.cs
    public Seeker seeker;
    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    //used for near stop distance
    [SerializeField]
    public float lerpDuration = 35f;
    public float scalingFactor = 1f;
    private Vector3 avatarLocalScale = Vector3.one;
    private CapsuleCollider2D capsuleTrigger;
    private CircleCollider2D circleTrigger;
    private Rigidbody2D rigidbody2d;
    private float currentFlipDirection=1; //1 or -1 for left and right flip

    /*
    public float CurrentFlipDirection
    {
        get
        {
            return currentFlipDirection;
        }
        set
        {
            currentFlipDirection = value;
        }
    }*/

    private void Start()
    {
        //currentFlipDirection = 1.0f;
        avatarLocalScale = gameObject.transform.localScale;
        capsuleTrigger = gameObject.GetComponent<CapsuleCollider2D>();
        circleTrigger = gameObject.GetComponent<CircleCollider2D>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    public CapsuleCollider2D getCapsuleTrigger()
    {
        return capsuleTrigger;
    }

    public CircleCollider2D getCircleTrigger()
    {
        return circleTrigger;
    }

    public Rigidbody2D getRigidbody2D()
    {
        return rigidbody2d;
    }

    public Vector3 getLocalScale()
    {
        return avatarLocalScale;
    }

    public void setLocalScale(Vector3 localScale)
    {
        avatarLocalScale = localScale;
    }


    public float getFlipDirection()
    {
        return currentFlipDirection;
    }

    public void setFlipDirection(float flipDirection)
    {
        currentFlipDirection = flipDirection;
    }
}
