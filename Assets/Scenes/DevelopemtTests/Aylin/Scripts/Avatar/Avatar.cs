using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //used for near stop distance
    [SerializeField]
    public float lerpDuration = 35f;
    public float scalingFactor = 1f;
    private Vector3 avatarLocalScale = Vector3.one;
    private CapsuleCollider2D capsuleTrigger;
    private Rigidbody2D rigidbody2d;

    private void Start()
    {
        avatarLocalScale = gameObject.transform.localScale;
        capsuleTrigger = gameObject.GetComponent<CapsuleCollider2D>();
        rigidbody2d = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    public CapsuleCollider2D getCapsuleTrigger()
    {
        return capsuleTrigger;
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
}
