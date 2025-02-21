using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

public class PaperAirplane : Enemy
{
    float angle;
    override protected void Start()
    {
        base.Start();
        Physics2D.IgnoreCollision(transform.GetComponent<BoxCollider2D>(), transform.Find("/Grid/Tilemap").GetComponent<CompositeCollider2D>());
        move.lockRotation = false;


        velocity = move.SetSpeed(new Vector2(1, 0));
        //StartCoroutine(Moving());
    }

    override protected void Update(){
        angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
    }

    void FixedUpdate()
    {
        Move(true);
    }

    // 移动
    protected override void Move(bool isChasing){
        float turnSpeed = speed;
        float flySpeed = 5f;

        // 重力不同机动性不同
        if(GameController.gravityScale < 1){
            turnSpeed *= 30;
            flySpeed *= 3;
        }

        if(GameController.gravityScale > 1){
            rb.gravityScale = 5;
            flySpeed *= 2  ;
        }
        else{
            rb.gravityScale = 0;
        }

        waitTime = 0;

        // 向主角方向移动
        Vector2 dir = Rabbit.instance.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        velocity = move.AddSpeed(new Vector2(turnSpeed * Mathf.Cos(angle), turnSpeed * Mathf.Sin(angle)));

        // 限速
        velocity = move.SetSpeed(velocity.normalized * flySpeed);
    }

}
