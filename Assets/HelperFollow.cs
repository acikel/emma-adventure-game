using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//To use this script the game obejct it is applied to needs a seeker and a rigidbody2d.
//The linear drag of the rigidbody2d should be set to 1.5 or a similar value to make the
//game object stop on path end. The speed can be increased to something like 400.
public class HelperFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;

    public float speed = 200f;
    //How near AI needs to be to a waypoint till it moves on to the next one.
    public float nextWaypointDistance =3f;

    //used to flip the helper into the direction it is travelling. This variable schould point to the graphics (like spriterenderer and so on) object of this gameobject or if everything is in one object to the transform of this game obejct.
    public Transform helper;

    //current followed path
    private Path path;
    //Stores current waypoint along the path path we are targeting.
    private int currentWayPoint=0;
    //tells if we have reached the end of our path
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        //generate Path with start point, end point and function that is called when path was calculated as parameters in this order.
        //The last parameter of the function which is called when the calculation of the path is terminated is done because depending on the scene
        //calculating the path can take very long, with this aproach it is calculated in the background and we get notified when its done.
        //This way the game is not hold up while calculation.
        //seeker.StartPath(rb.position, target.position, OnPathComplete); //this only calculated the path once.
        //to repead following or creating a path use following script:
        InvokeRepeating("UpdatePath", 0f, 0.5f);//"UpdatePath" function that is called repeadedly. 0f = amount of time to wait until we call the method defined as first parameter (here "UpdatePath") with 0f it is called immediatly, 0.5f = repead rate can also be set to other values like 1f= update every second or 2f = every other second 0.5f repeads it every half second.

    }

    private void UpdatePath()
    {
        if(seeker.IsDone()) //checking if we are currently updating our graph. As we only want to create a new path if no other path is beeing calculated.
            seeker.StartPath(rb.position, target.position, OnPathComplete);
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
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (path == null) //no path currently exists.
            return;
        if(currentWayPoint>= path.vectorPath.Count)//check if we havent reached the end the path. If we reached it we want to stop moving. path.vectorPath.Count describes the total amount of waypoint on our path.
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        //move helper:
        //get direction to the next waypoint along our path:
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;//path.vectorPath[currentWayPoint] finds current waypoint along our path. path.vectorPath[currentWayPoint] - rb.position gives a vector that points from current position rb.position to the next waypoint path.vectorPath[currentWayPoint]. We normilize the direction to have a vector with length 1.
        //Apply force to move helper into the direction direction:
        Vector2 force = direction * speed * Time.deltaTime;

        //adding force to helper:
        rb.AddForce(force);
            
        //Distance to our next waypoint:
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        //Check if current waypoint was reached and move on to the next one:
        if(distance < nextWaypointDistance)
        {
            currentWayPoint++;
        }

        
        flipGameObject(force);

    }

    //flip this game object depending on which direction it is travelling to:
    private void flipGameObject(Vector2 force)
    {
        if (force.x >= 0.01f)
        {
            helper.localScale = new Vector3(-1f, 1f, 1f);
        }else if(force.x <=-0.01f)
        {
            helper.localScale = new Vector3(1f,1f,1f);
        }
    }
}
