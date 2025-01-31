using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using UnityEditor;
using UnityEngine.Experimental.GlobalIllumination;

public class Rabbit : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 velocity;
    [SerializeField] float playerSpeed;
    private float horizontal;
    private bool facingRight = true;
    [SerializeField] float jumpSpeed;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask GroundLayer;

    [SerializeField] Vector2 vecGravity;
    [SerializeField] float fallMultiplier;

    [SerializeField] float jumpTimeMax;
    [SerializeField] float jumpBufferTimeMax;   //跳跃缓冲
    float jumpBufferTime;
    float touchBufferTime;
    float jumpCounter;
    bool isJumping;
    bool nowJump;
    bool stopJump;
    [SerializeField] bool doubleJump;

    [SerializeField] float rushSpeed;
    [SerializeField] float rushTimeMax;
    [SerializeField] float rushCounter;
    bool isRushing;

    [SerializeField] float fallSpeedMax;


    public Animator ani;
    int aniStatu;

    Vector3 ResetPosition;
    // Start is called before the first frame update
    void Awake(){
        ResetPosition = transform.position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            Reset();
            //GameController.Reset();
            //GetComponent<GameController>().ColliderCheck();
        }
        horizontal = 0;
        aniStatu = 1;     //stand

        
        velocity = GetComponent<MoveController>().ToRelativeVelocity(rb.velocity);
        //  Debug.Log("velocity:" + velocity);

        if(Input.GetKey(KeyCode.A)){
            horizontal -= 1;
            if(velocity.x < -0.01)
                aniStatu = 2; //walk
        }
        if(Input.GetKey(KeyCode.D)){
            horizontal += 1;
            if(velocity.x > 0.01)
                aniStatu = 2; //walk
        }

        if(velocity.y > 0 && !isGround()){       //地面不平
            aniStatu = 3; //Jump
        }
        else if(velocity.y < 0 && !isGround()){  //地面不平
            aniStatu = 4; //Fall
        }

        if((Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1)) && isGround() && !isRushing){
            isRushing = true;
            rushCounter = rushTimeMax;
        }
        
        //if(Input.GetButtonDown("Jump")){
        if(Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Space)){ 
            if(rushCounter < 0.08f || !isRushing){
                if(rushCounter < 0.08f){    //冲刺末端即可跳
                    isRushing = false;
                    rushCounter = rushTimeMax;
                }

                if(!isJumping && (isGround() || touchBufferTime > 0)){
                    //GetComponent<Status>().AddSpeed(true, 0.1f);
                    isJumping = true;
                    nowJump = true;
                    stopJump = false;
                    jumpCounter = 0;
                    doubleJump = true;
                }
                else if(!isJumping && doubleJump){
                    //GetComponent<Status>().AddSpeed(true, 0.1f);
                    isJumping = true;
                    nowJump = true;
                    stopJump = false;
                    jumpCounter = 0;
                    doubleJump = false;
                }
            }
            
        }

        //if(Input.GetButtonUp("Jump")){
         if(Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Space)){
            stopJump = true;
         }
        //     if(isJumping){
        //         GetComponent<Status>().SetSpeed(new Vector2(velocity.x, velocity.y / 3f));
        //     }
        //     Debug.Log("JumpingFall" + isJumping);
        //     isJumping = false;
        // }

        if(isRushing)
            aniStatu = 5; //Rush
        
        

        ani.SetInteger("status", aniStatu);
        if(touchBufferTime > 0)
            touchBufferTime -= Time.deltaTime;

        if(Input.GetKey(KeyCode.LeftShift)){
            if(Input.GetKeyDown(KeyCode.W)){
                GameController.GravityScaleChange(0.8f);
            }
            else if(Input.GetKeyDown(KeyCode.S)){
                GameController.GravityScaleChange(1.5f);
            }
            else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)){
                GameController.GravityScaleChange(1f);
            }

        }
        
    }

    private void FixedUpdate(){
        //float theta = GameController.theta;        

        Move();
        if(isGround())
            doubleJump = true;
        //transform.Find("ColliderBox").GetComponent<ColliderBox>().SetContact(false);
        
        
        if(isRushing){
            Rush();
            Fall();
        }
        else if(isJumping)
            Jump();
        else
            Fall();

        transform.Find("ColliderBox").GetComponent<ColliderBox>().SetContact(false);

        

        
            
            
        //rb.velocity = new Vector2(velocity.x*Mathf.Cos(theta*Mathf.Deg2Rad) + velocity.y*Mathf.Sin(theta*Mathf.Deg2Rad), -velocity.x*Mathf.Sin(theta*Mathf.Deg2Rad) + velocity.y*Mathf.Cos(theta*Mathf.Deg2Rad));

        
        
    }

    private void Move(){
        if(velocity.y < -fallSpeedMax * GameController.gravityScale * GameController.gravityNormal){
            velocity.y = -fallSpeedMax * GameController.gravityScale * GameController.gravityNormal;
            //Debug.Log("FALL" + -fallSpeedMax * GameController.gravityScale);
        }
        velocity = GetComponent<MoveController>().SetSpeed(new Vector2(horizontal * playerSpeed, velocity.y));
        if(((facingRight && horizontal < 0) || (!facingRight && horizontal > 0)) && !isRushing){
            facingRight = ! facingRight;
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
        }
        
    }

    private bool isGround(){
        //  Physics2D.OverlapCapsule(GroundCheck.position, new Vector2(0.9f, 0.03f), CapsuleDirection2D.Horizontal, 0, GroundLayer) && 
        if(transform.Find("ColliderBox").GetComponent<ColliderBox>().isContact()){
            touchBufferTime = jumpBufferTimeMax;
            return true;
        }

        return false;
    }

    private void Jump(){
        //if(Input.GetButtonUp("Jump")){
        //if(Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Space)){
        
        if(stopJump){
            if(isJumping)
                velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, velocity.y / 3f));
            //Debug.Log("JumpingFall");
            isJumping = false;
            return ;
        }
        if(velocity.y <= 0 && !nowJump){
            isJumping = false;        
        
        }
        
        if((velocity.y > 0 && isJumping) || nowJump){
            jumpCounter += Time.deltaTime;
            velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, jumpSpeed / GameController.gravityScale));
            if(jumpCounter > jumpTimeMax){
                //Debug.Log("MAX!!");
                velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, velocity.y / 1.5f));
                isJumping = false;
            }
            nowJump = false;
        }
        
    }

    private void Fall(){
        
    }

    private void Rush(){
        //Debug.Log("Rushing:" + rushCounter + " x:" + rb.velocity.x);
        if(isGround())
            rushCounter -= Time.deltaTime;
        else
            rushCounter -= Time.deltaTime / 2;
        if(rushCounter > 0){
            velocity = GetComponent<MoveController>().SetSpeed(new Vector2(rushCounter * rushSpeed * (facingRight?1:-1), velocity.y));
        }
        else
            isRushing = false;
    }

    public void Reset(){
        transform.position = ResetPosition;
        velocity = GetComponent<MoveController>().SetSpeed(new Vector2(0, 0));
        //GetComponent<Numbers>().Reset();
    }
}
