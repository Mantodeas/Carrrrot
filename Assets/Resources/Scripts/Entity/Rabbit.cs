using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Rabbit : MonoBehaviour
{
    [Header("组件")]
    public static Rabbit instance;
    Rigidbody2D rb;

    [Header("移动控制")]
    Vector2 velocity;
    [SerializeField] float playerSpeed;         // 移动速度
    private float horizontal;                   // 移动方向
    private bool facingRight = true;            // 面朝方向

    [Header("跳跃控制")]
    [SerializeField] float jumpSpeed;           // 跳跃速度
    [SerializeField] float jumpTimeMax;         // 最大跳跃时间
    [SerializeField] float jumpBufferTimeMax;   // 跳跃缓冲
    float jumpBufferTime;
    float touchBufferTime;                      // 跳跃补偿时间
    float jumpCounter;                          // 当前跳跃时间
    [SerializeField] GameObject jumpEff;        // 跳跃特效
    bool isJumping;                             // 是否正在跳跃
    bool nowJump;                               // 是否开始跳跃
    bool stopJump;                              // 是否结束跳跃
    [SerializeField] bool doubleJump;           // 是否有二段跳

    [Header("滑铲控制")]
    [SerializeField] float rushSpeed;           // 滑铲时间
    [SerializeField] float rushTimeMax;         // 最大滑铲时间
    [SerializeField] float rushCounter;         // 当前滑铲时间
    bool isRushing;                             // 是否正在滑铲

    [Header("UI控制")]
    [SerializeField] GameObject HPBar;          // 血量条
    [SerializeField] GameObject MPBar;          // 能量条
    [SerializeField] GameObject GPBar;          // 控制能力条
    float HPLength;                             // 血量条长度

    [SerializeField] float MPMax;               // 最大能量
    float MPLength;                             // 能量条长度
    public float MP;                            // 能量值
    [SerializeField] int MPStatus;              // 是否满能
    [SerializeField] float MPDecreaseTimeMax;   // 最大能量衰减延迟值
    float MPDecreaseTime;                       // 能量衰减时间
    [SerializeField] float MPDecreaseSpeed;     // 能量衰减速度
    
    [SerializeField] float GPMax;               // 最大控制能力值
    float GP;                                   // 控制能力值
    float GPLength;                             // 控制能力条长度

    [Header("动画控制")]
    public Animator ani;                        // 动画控制
    int aniStatu;                               // 动画状态
    [SerializeField] float trailTime;           // 残影生成时间

    [Header("抓取控制")]
    GameObject catchObj;                        // 抓取物
    Transform catchSprite;                      // 抓取物图像
    [SerializeField] float releaseSpeed;        // 扔出速度

    [Header("逻辑控制")]
    Vector3 ResetPosition;                      // 重启位置
    bool isAttacking;                           // 是否攻击

    [Header("互斥控制")]
    bool lockSome;                              // 跳跃、滑铲锁
    bool moveable;                              // 移动锁
    // Start is called before the first frame update
    void Awake(){
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }
        ResetPosition = transform.position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        catchSprite = transform.Find("Catch").Find("CatchSprite");

        GP = 0;
        MPStatus = 0;
        MP = 0;
        MPDecreaseTime = 0;
        moveable = true;
        MPLength = MPBar.transform.localScale.x;
        HPLength = HPBar.transform.localScale.x;
        GPLength = GPBar.transform.localScale.x;

        SetHP(1);
        ChangeMP(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            Reset();
        }
        horizontal = 0;
        aniStatu = 1;     // 站立

        
        velocity = GetComponent<MoveController>().ToRelativeVelocity(rb.velocity);

        if(Input.GetKey(KeyCode.A)){
            horizontal -= 1;
            if(velocity.x < -0.01)
                aniStatu = 2; // 移动
        }
        if(Input.GetKey(KeyCode.D)){
            horizontal += 1;
            if(velocity.x > 0.01)
                aniStatu = 2; // 移动
        }


        if(velocity.y > 0 && !isGround()){
            aniStatu = 3; // 跳跃
        }
        else if(velocity.y < 0 && !isGround()){
            aniStatu = 4; // 坠落
        }

        // 滑铲
        if((Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1)) && isGround() && !lockSome){
            isRushing = true;
            lockSome = true;
            rushCounter = rushTimeMax;
        }
        
        if(Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Space)){ 
            if(rushCounter < 0.08f || !isRushing){
                // 冲刺末端即可跳
                if(rushCounter < 0.08f){    
                    isRushing = false;
                    lockSome = false;
                    rushCounter = rushTimeMax;
                }

                // 一段跳
                if(!isJumping && (isGround() || touchBufferTime > 0)){
                    isJumping = true;
                    nowJump = true;
                    stopJump = false;
                    jumpCounter = 0;
                    doubleJump = true;
                    StartCoroutine(JumpEff(Color.white));

                    touchBufferTime = 0;    //防止连跳
                }

                // 二段跳
                else if(!isJumping && doubleJump){
                    isJumping = true;
                    nowJump = true;
                    stopJump = false;
                    jumpCounter = 0;
                    doubleJump = false;
                    StartCoroutine(JumpEff(new Color(0.5f, 0.3f, 0.7f)));
                }
            }
            
        }

        // 短跳
        if(Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Space)){
            stopJump = true;
        }

        // 重击
        if(Input.GetKeyDown(KeyCode.C) && !lockSome){
            isAttacking = true;
        }

        if(isRushing)
            aniStatu = 5; // 滑铲

        if(isAttacking)
            aniStatu = 6; // 重击
        
        // 重力控制结束
        if(GP <= 0){
            if(Input.GetKeyDown(KeyCode.W) && !lockSome && isGround()){
                lockSome = true;
                StartCoroutine(GravityControlLight(0.8f));
            }
            else if(Input.GetKeyDown(KeyCode.S) && !lockSome && !isGround()){
                lockSome = true;
                StartCoroutine(GravityControlHeavy(1.5f));
            }   
        }
        // 重力控制中
        else{
            GP -= Time.deltaTime;
            if(GameController.gravityScale == 1)
                SetGP(1 - GP / GPMax);
            else
                SetGP(GP / GPMax);
        }

        // 动画
        ani.SetInteger("status", aniStatu);

        // 跳跃补偿
        if(touchBufferTime > 0)
            if(velocity.y > 0)
                touchBufferTime = 0;
            else
                touchBufferTime -= Time.deltaTime;

        // 能量控制
        if(MPDecreaseTime > 0){
            MPDecreaseTime -= Time.deltaTime;
        }
        if(MPDecreaseTime < 0){
            ChangeMP(-MPDecreaseSpeed * Time.deltaTime);
        }
        if(MP >= MPMax){
            MP = MPMax;
            if(MPStatus == 0){
                MPStatus = 1;
                StartCoroutine(Overload());
            }
        }
        if(MP <= 0){
            MP = 0;
            if(MPStatus == 1){
                MPStatus = 0;
            }
        }

        // 抓取
        if(Input.GetKeyDown(KeyCode.E)){
            if(catchObj == null)
                Catch();
            else{
                Release();
            }
        }
    }

    // 能量满后出现残影并给予增益
    IEnumerator Overload(){
        while(MPStatus != 0){
            GameObject trail = Pool.instance.Get("Trail", transform);
            trail.GetComponent<SpriteRenderer>().sprite = transform.GetComponent<SpriteRenderer>().sprite;
            trail.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1f, 0.5f);
            trail.transform.localScale = transform.localScale;
            trail.transform.position = transform.position;
            trail.transform.rotation = transform.rotation;
            trail.GetComponent<Trail>().Init();
            yield return new WaitForSeconds(trailTime);
        }
    }

    // 重力控制-弱
    IEnumerator GravityControlLight(float gravity){
        ani.SetBool("gravityChange", true);
        moveable = false;
        GameController.ShakeCamera(0.05f, 0.5f);
        velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x / 20, 0));
        yield return new WaitForSeconds(0.5f);

        // 自动一段跳
        {
            isJumping = true;
            nowJump = true;
            stopJump = false;
            jumpCounter = 0;
            doubleJump = true;
            StartCoroutine(JumpEff(Color.white));

            touchBufferTime = 0;
        }

        ani.SetBool("gravityChange", false);
        lockSome = false;
        moveable = true;

        GP = GPMax;
        GameController.GravityScaleChange(gravity);
        yield return new WaitForSeconds(GPMax);
        GameController.GravityScaleChange(1f);
        GP = GPMax;
    }

    // 重力控制-强
    IEnumerator GravityControlHeavy(float gravity){
        moveable = false;
        velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, 10));
        while(true){
            if(isGround()){
                ani.SetBool("gravityChange", true);
                GameController.ShakeCamera(0.05f, 0.5f);
                GameController.GravityScaleChange(gravity);
                GP = GPMax;
                velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x / 20, 0));
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        ani.SetBool("gravityChange", false);
        lockSome = false;
        moveable = true;
        yield return new WaitForSeconds(GPMax);
        GameController.GravityScaleChange(1f);
        GP = GPMax;
    }

    // 跳跃特效
    IEnumerator JumpEff(Color color){
        GameController.SoundPlay("jump", false, 1f);

        GameObject jumpEffImage = Instantiate(jumpEff, transform.position, transform.rotation);
        jumpEffImage.transform.position = transform.position;
        jumpEffImage.transform.rotation = transform.rotation;
        jumpEffImage.transform.localScale = new Vector3(1, 1, 1);
        jumpEffImage.GetComponent<SpriteRenderer>().color = color;
        jumpEffImage.GetComponent<SpriteRenderer>().enabled = true;
        int n=24;
        for(int i=0; i<n; i++){
            jumpEffImage.transform.localScale = new Vector3(1 + 3 * (float)i / n, 1 + (float)i / n, 1);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(jumpEffImage);
    }

    private void FixedUpdate(){
        Move();
        if(isGround())
            doubleJump = true;
        
        if(isRushing)
            Rush();
        else if(isJumping)
            Jump();
    }

    // 移动
    private void Move(){
        if(moveable)
            velocity = GetComponent<MoveController>().SetSpeed(new Vector2(horizontal * playerSpeed, velocity.y));
        if(((facingRight && horizontal < 0) || (!facingRight && horizontal > 0)) && !lockSome){
            facingRight = ! facingRight;
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
        }
        
    }

    // 是否位于地面
    private bool isGround(){
        // Physics2D.OverlapCapsule(GroundCheck.position, new Vector2(0.9f, 0.03f), CapsuleDirection2D.Horizontal, 0, GroundLayer) && 
        //if(transform.Find("GroundCheck").GetComponent<GroundCheck>().isContact()){
        if(GetComponent<MoveController>().isGround()){
            touchBufferTime = jumpBufferTimeMax;
            return true;
        }

        return false;
    }

    // 跳跃
    private void Jump(){
        // 松手短跳
        if(stopJump){
            if(isJumping)
                velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, velocity.y / 3f));
            isJumping = false;
            return ;
        }

        // 碰头
        if(velocity.y <= 0 && !nowJump){
            isJumping = false;        
        
        }
        
        // 长跳
        if((velocity.y > 0 && isJumping) || nowJump){
            jumpCounter += Time.deltaTime;
            velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, jumpSpeed / GameController.gravityScale));

            // 达到最大时间
            if(jumpCounter > jumpTimeMax){
                velocity = GetComponent<MoveController>().SetSpeed(new Vector2(velocity.x, velocity.y / 1.5f));
                isJumping = false;
            }
            nowJump = false;

        }
        
    }

    // 滑铲
    private void Rush(){
        // 空中可以滑更远
        if(isGround())
            rushCounter -= Time.deltaTime;
        else
            rushCounter -= Time.deltaTime / 2;

        if(rushCounter > 0){
            velocity = GetComponent<MoveController>().SetSpeed(new Vector2(rushCounter * rushSpeed * (facingRight?1:-1), velocity.y));
        }
        // 结束
        else{
            isRushing = false;
            lockSome = false;
        }
    }

    // 重置
    public void Reset(){
        transform.position = ResetPosition;
        velocity = GetComponent<MoveController>().SetSpeed(new Vector2(0, 0));
    }

    // 血条设置
    public void SetHP(float portion){
        HPBar.transform.localScale = new Vector3(portion * HPLength, HPBar.transform.localScale.y, 1);
    }

    // 能量条设置
    public void ChangeMP(float value){
        MP += value;
        if(value > 0)
            MPDecreaseTime = MPDecreaseTimeMax;
        if(MP > MPMax)
            MP = MPMax;
        MPBar.transform.localScale = new Vector3(MP / MPMax * MPLength, MPBar.transform.localScale.y, 1);
    }

    // 控制能力跳设置
    public void SetGP(float portion){
        GPBar.transform.localScale = new Vector3(portion * GPLength, GPBar.transform.localScale.y, 1);
    }

    // 抓取
    void Catch(){
        catchObj = transform.Find("Catch").GetComponent<Catch>().GetItem();
        if(catchObj == null)
            return ;

        catchSprite.GetComponent<SpriteRenderer>().sprite = catchObj.GetComponent<SpriteRenderer>().sprite;
        catchObj.SetActive(false);
        catchObj.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, catchObj.transform.position.z);
    }

    // 丢出抓取物体
    void Release(){
        catchObj.transform.position = catchSprite.position;
        catchObj.SetActive(true);
        catchObj.GetComponent<MoveController>().SetSpeed(new Vector2(releaseSpeed * (facingRight?1:-1), 10));
        catchSprite.GetComponent<SpriteRenderer>().sprite = null;
        catchObj = null;
    }

    // 重击动画与逻辑
    void SetAttackTrue(){
        HeavyAttack attack = transform.Find("Attack").GetComponent<HeavyAttack>();
        attack.SetActiveTrue();

        if(attack.AttackGround()){
            GameController.SoundPlay("thump", false, 0.5f);
            GameController.ShakeCamera(0.05f, 0.1f);
            GameObject heavyHit = Instantiate(Resources.Load<GameObject>("Prefab/HeavyHit"), transform.position, transform.rotation);
            if(!facingRight)
                heavyHit.GetComponent<SpriteRenderer>().flipX = true;
            else
                heavyHit.GetComponent<SpriteRenderer>().flipX = false;

            if(GameController.gravityScale > 1){
                GameObject hitWave = Instantiate(Resources.Load<GameObject>("Prefab/HitWave"), transform.position, transform.rotation);
                hitWave.GetComponent<MoveController>().SetSpeed(new Vector2(15 * (facingRight?1:-1), 0));
                if(!facingRight)
                    hitWave.transform.localScale = new Vector3(-3, 3, 1);
                else
                    hitWave.transform.localScale = new Vector3(3, 3, 1);
            }
        }
    }

    void SetAttackFalse(){
        transform.Find("Attack").GetComponent<HeavyAttack>().SetActiveFalse();
    }

    void SetGunActiveFalse(){
        GameController.gun.SetActive(false);
        playerSpeed /= 5;
        lockSome = true;
    }

    void SetGunActiveTrue(){
        isAttacking = false;
        GameController.gun.SetActive(true);
        playerSpeed *= 5;
        lockSome = false;
    }

}
