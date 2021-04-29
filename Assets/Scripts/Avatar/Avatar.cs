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
    public float lerpDistanceFarSummand = 40f;
    //used for near stop distance
    public float lerpDuration = 35f;

    private void Start()
    {

    }

    private void Update()
    {

    }
}
