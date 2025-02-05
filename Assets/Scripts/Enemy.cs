using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Enemy : MonoBehaviour
{
    [Header("子弹")]
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected GameObject bulletPrefab;

    [Header("状态机")]
    [SerializeField] protected int currentAction;

    [Header("等待与行动")]
    [SerializeField] protected float waitTimeMax;
    protected float waitTime;
    [SerializeField] protected int direction;
    protected bool discovered;
    protected float discoverTime;
    protected int moving;
    [SerializeField] protected float fric;
    [SerializeField] protected float speed;
    protected Vector2 velocity;
    protected MoveController move;
    protected Rigidbody2D rb;

    [Header("攻击")]
    [SerializeField] protected int attackModeMax;
    protected bool isAttack;
    [SerializeField] protected int attackPortion;
    [SerializeField] protected float attackDistance;

    [Header("动画")]
    public Animator ani;
    protected int aniStatu;
    [SerializeField] protected GameObject warningPrefab;
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "Enemy";
        rb = transform.parent.GetComponent<Rigidbody2D>();
        move = transform.parent.GetComponent<MoveController>();

        direction = 1;
        discovered = false;
        currentAction = 1;
        moving = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        velocity = move.ToRelativeVelocity(rb.velocity);
        //Debug.Log(Random.Range(0, 2));
        Ray ray = new Ray(transform.position, move.ToRelativeVelocity(transform.parent.forward));
        RaycastHit2D hitInfo;
        hitInfo = Physics2D.Raycast(transform.position, move.ToRelativeVelocity(new Vector2(direction, 0)), 5, LayerMask.GetMask("Rabbit"));
        Debug.DrawLine(transform.position, (Vector2)transform.position + move.ToRelativeVelocity(new Vector3(direction * 10, 0, 0)), Color.blue);
        if(hitInfo){
            discovered = true;
            discoverTime = 60;
        }
        else{
            discovered = false;
        }

        switch(currentAction){
            case 0: //等待
                break;

            case 1: //巡逻
                waitTime += Time.deltaTime;
                if ((float)(Random.Range(0, 1000)) / 1000 < waitTime / waitTimeMax && waitTime / waitTimeMax > 0.2f){
                    //Debug.Log((float)action / 1000 + " " +  waitTime + " " + waitTimeMax);
                    moving = -1;    //随机移动
                }

                if(discovered){
                    currentAction = 2;
                    waitTime = 0;
                    moving = 1;     //立即移动一次
                    StartCoroutine("Warning");
                }

                break;

            case 2: //追逐
                waitTime += Time.deltaTime;
                discoverTime -= Time.deltaTime;

                if(discoverTime <= 0){
                    currentAction = 1;
                    discovered = false;

                } 

                direction = GameController.instance.rabbit.transform.position.x > transform.position.x ? 1 : -1;

                if ((float)(Random.Range(0, 1000)) / 1000 < waitTime / waitTimeMax && waitTime / waitTimeMax > 0.2f){
                    float portion = Mathf.Abs(transform.position.x - GameController.instance.rabbit.transform.position.x) / attackDistance;
                    portion = portion > 1 ? portion : 1;
                    //Debug.Log(portion);
                    
                    if(Random.Range(0, 100) > attackPortion / portion){ //距离过远增加移动欲望
                        moving = 1;         //朝主角方向移动
                    }
                    else{
                        currentAction = 3;
                    }
                    waitTime = 0;
                }
                break;

            case 3: //攻击
                Attack(Random.Range(0, attackModeMax));
                break;

            default:
                break;
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            currentAction = 3;
        }
        if(Input.GetKeyDown(KeyCode.E)){
            //Move(true);
            moving = 1;
        }
    }

    void FixedUpdate()
    {
        if(moving != 0){
            if(moving < 0)
                Move(false);
            else
                Move(true);
            moving = 0;
        }
        
        velocity.x -= velocity.x * fric * Time.deltaTime;
        velocity = move.SetSpeed(velocity);
        
    }

    virtual protected void Move(bool isChasing){
        waitTime = 0;

        if(!isChasing){
            direction = Random.Range(0, 2) == 0 ? 1 : -1;
        }
        else{
            direction = GameController.instance.rabbit.transform.position.x > transform.position.x ? 1 : -1;
        }
        transform.parent.localScale = new Vector2(direction, 1);
        velocity = move.SetSpeed(new Vector2(direction * speed, rb.velocity.y));
    }

    protected IEnumerator Warning(){
        //Debug.Log("Warning");
        GameObject warning = Instantiate(warningPrefab, transform.position, transform.rotation);
        warning.transform.parent = transform.parent;
        warning.transform.localPosition = new Vector3(0.6f, 0.6f, 0);
        warning.transform.localScale = new Vector2(2, 2);

        // Vector2 temp = warning.transform.position;
        // for(int i=0; i<10; i++){
        //     temp.y += 0.1f;
        // }
        yield return new WaitForSeconds(2);
        Destroy(warning);
    }

    protected void SetStatus(int status){
        ani.SetInteger("status", status);
    }

    virtual protected void Attack(int mode){
        currentAction = 2;
    }
}
