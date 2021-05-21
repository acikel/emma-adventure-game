using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandma : ActionObject
{
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        
    }

    public override void actionOnDrop(string itemName)
    {
        //Debug.Log("actions drop name " + itemName);
        if (animator!=null && itemName.Contains("GrandmaHands"))
        {
            //Debug.Log("actions drop animation triggered");
            animator.SetTrigger("GrandmaRevived");
        }
    }
}
