using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 velocity;
    static float fallSpeedMax = 4f; // 最大坠落速度
    [SerializeField] float fric;    // 摩擦力

    Transform ground;
    bool groundCheck;
    public bool lockRotation;   // 是否锁定方向
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ground = transform.Find("GroundCheck");

        groundCheck = true;
        lockRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(ground != null)
            groundCheck = ground.GetComponent<GroundCheck>().isContact();

        if(lockRotation)
            transform.rotation = Quaternion.Euler(0, 0, GameController.theta);
    }

    void FixedUpdate()
    {
        velocity = ToRelativeVelocity(rb.velocity);

        // 摩擦力
        velocity.x -= velocity.x * fric * Time.deltaTime * (groundCheck?1:0);
        velocity = SetSpeed(velocity);

        // 坠落限速
        if(velocity.y < -fallSpeedMax * GameController.gravityScale * GameController.gravityNormal){
            velocity.y = -fallSpeedMax * GameController.gravityScale * GameController.gravityNormal;
            velocity = SetSpeed(velocity);
        }
    }

    // 速度叠加
    public Vector2 AddSpeed(Vector2 speed){
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(rb.bodyType == RigidbodyType2D.Dynamic)
            rb.velocity += ToAbsoluteVelocity(speed);
        return ToRelativeVelocity(rb.velocity);
    }

    // 速度设置
    public Vector2 SetSpeed(Vector2 speed){
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(rb.bodyType == RigidbodyType2D.Dynamic)
            rb.velocity = ToAbsoluteVelocity(speed);
        return ToRelativeVelocity(rb.velocity);

    }

    // 绝对速度(rb.velocity)转换成新重力相对速度(velocity)
    public Vector2 ToRelativeVelocity(Vector2 velocity){
        Vector2 relativeVelocity = new Vector2(0, 0);
        relativeVelocity.x = Vector3.Project(velocity, GameController.moveDirection).magnitude * (Vector2.Dot(velocity, GameController.moveDirection) < 0 ? -1 : 1);
        relativeVelocity.y = Vector3.Project(velocity, GameController.jumpDirection).magnitude * (Vector2.Dot(velocity, GameController.jumpDirection) < 0 ? -1 : 1);
        return relativeVelocity;
    }

    // 新重力相对速度(velocity)转换成绝对速度(rb.velocity)
    public Vector2 ToAbsoluteVelocity(Vector2 velocity){
        Vector2 absoluteVelocity = new Vector2(0, 0);
        absoluteVelocity = velocity.x * GameController.moveDirection + velocity.y * GameController.jumpDirection;
        return absoluteVelocity;
    }

    // 是否位于地面
    public bool isGround(){
        return groundCheck;
    }
}
