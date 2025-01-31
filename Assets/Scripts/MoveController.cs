using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 velocity;
    public Vector2 moveDirection, jumpDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        jumpDirection = -Physics2D.gravity.normalized;
        moveDirection = new Vector2(jumpDirection.y, -jumpDirection.x);

        transform.rotation = Quaternion.Euler(0, 0, GameController.theta);
    }

    void FixedUpdate()
    {
        //Debug.Log(moveDirection + " " + jumpDirection);
        // rb.velocity += velocity;
        // velocity = new Vector2(0, 0);
        //Debug.Log(rb.velocity);
    }

    public Vector2 AddSpeed(Vector2 speed){
        //Debug.Log(speed);
        //rb.velocity += speed;
        return ToRelativeVelocity(rb.velocity);
    }

    public Vector2 AddSpeed(bool jump, float scale){
        //Debug.Log(speed);
        //rb.velocity += scale * (jump ? jumpDirection : moveDirection);
        return ToRelativeVelocity(rb.velocity);
    }

    public Vector2 SetSpeed(Vector2 speed){
        //Debug.Log(speed);
       rb.velocity = ToAbsoluteVelocity(speed);
       return ToRelativeVelocity(rb.velocity);

    }

    public Vector2 ToRelativeVelocity(Vector2 velocity){
        Vector2 relativeVelocity = new Vector2(0, 0);
        relativeVelocity.x = Vector3.Project(velocity, moveDirection).magnitude * (Vector2.Dot(velocity, moveDirection) < 0 ? -1 : 1);
        relativeVelocity.y = Vector3.Project(velocity, jumpDirection).magnitude * (Vector2.Dot(velocity, jumpDirection) < 0 ? -1 : 1);
        //Debug.Log("velocity" + velocity + " relativeVelocity" + relativeVelocity);
        return relativeVelocity;
    }

    public Vector2 ToAbsoluteVelocity(Vector2 velocity){
        Vector2 absoluteVelocity = new Vector2(0, 0);
        absoluteVelocity = velocity.x * moveDirection + velocity.y * jumpDirection;
        return absoluteVelocity;
    }


}
