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

public class HeroMove : MonoBehaviour
{
    private Rigidbody2D rb;
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
    [SerializeField] bool doubleJump;

    [SerializeField] float rushSpeed;
    [SerializeField] float rushTimeMax;
    [SerializeField] float rushCounter;
    bool isRushing;

    public Animator ani;
    int aniStatu;

    Vector3 ResetPosition;
    // Start is called before the first frame update
    void Awake(){
        //GetComponent<Numbers>().SetNum(2);
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

        if(Input.GetKey(KeyCode.A)){
            horizontal -= 1;
            if(rb.velocity.x < -0.01)
                aniStatu = 2; //walk
        }
        if(Input.GetKey(KeyCode.D)){
            horizontal += 1;
            if(rb.velocity.x > 0.01)
                aniStatu = 2; //walk
        }

        if(rb.velocity.y > 0 && !isGround()){       //地面不平
            aniStatu = 3; //Jump
        }
        else if(rb.velocity.y < 0 && !isGround()){  //地面不平
            aniStatu = 4; //Fall
        }

        if((Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1))&& isGround()){
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
                    SetSpeed(rb.velocity.x, jumpSpeed, false);
                    isJumping = true;
                    jumpCounter = 0;
                    doubleJump = true;
                }
                else if(!isJumping && doubleJump){
                    SetSpeed(rb.velocity.x, jumpSpeed*0.9f, false);
                    isJumping = true;
                    jumpCounter = 0;
                    doubleJump = false;
                }
            }
            
        }
        //if(Input.GetButtonUp("Jump")){
        if(Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Space)){
            if(isJumping)
                SetSpeed(rb.velocity.x, rb.velocity.y / 1.5f, false);
            isJumping = false;
        }
    }

    private void FixedUpdate(){
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

        ani.SetInteger("status", aniStatu);
        transform.Find("ColliderBox").GetComponent<ColliderBox>().SetContact(false);
        if(touchBufferTime > 0)
            touchBufferTime -= Time.deltaTime;
    }

    private void Move(){
        SetSpeed(horizontal * playerSpeed, rb.velocity.y, false);
        if((facingRight && horizontal < 0) || (!facingRight && horizontal > 0)){
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
        if(rb.velocity.y > 0 && isJumping){
            jumpCounter += Time.deltaTime;
            if(jumpCounter > jumpTimeMax){
                SetSpeed(rb.velocity.x, rb.velocity.y / 1.5f, false);
                isJumping = false;
            }
            //SetSpeed(vecGravity * jumpMultiplier * Time.deltaTime, true);
        }
        if(rb.velocity.y <= 0)
            isJumping = false;        
    }

    private void Fall(){
        SetSpeed(-vecGravity * fallMultiplier * Time.deltaTime, true);
    }

    private void Rush(){
        Debug.Log("Rushing:" + rushCounter + " x:" + rb.velocity.x);
        if(isGround())
            rushCounter -= Time.deltaTime;
        else
            rushCounter -= Time.deltaTime / 2;
        if(rushCounter > 0){
            aniStatu = 5; //Rush
            SetSpeed(rushCounter * rushSpeed * (facingRight?1:-1), rb.velocity.y, false);
        }
        else
            isRushing = false;
    }

    private void SetSpeed(float x, float y, bool relative){
        Vector2 temp = new Vector2(x, y);
        if(relative){
            rb.velocity += temp;
        }
        else{
            rb.velocity = temp;
        }
    }

    private void SetSpeed(Vector2 speed, bool relative){
        if(relative){
            rb.velocity += speed;
        }
        else{
            rb.velocity = speed;
        }
    }

    public void Reset(){
        transform.position = ResetPosition;
        SetSpeed(0, 0, false);
        //GetComponent<Numbers>().Reset();
    }
}
