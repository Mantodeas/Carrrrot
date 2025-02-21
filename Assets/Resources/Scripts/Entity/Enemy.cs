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
    public bool discovered;
    protected float discoverTime;
    protected int moving;
    //[SerializeField] protected float fric;
    [SerializeField] protected float speed;
    protected Vector2 velocity;
    protected MoveController move;
    protected Rigidbody2D rb;

    protected Vector3 startPos;
    public Transform trans;

    [Header("攻击")]
    [SerializeField] protected int attackModeMax;
    protected bool isAttack;
    [SerializeField] protected int attackPortion;
    [SerializeField] protected float attackDistance;

    [Header("动画")]
    public Animator ani;
    protected int aniStatu;
    [SerializeField] protected GameObject warningPrefab;

    [Header("房间逻辑")]
    [SerializeField] protected GameObject room;
    [SerializeField] protected bool duplicate;

    virtual protected void Awake()
    {
        transform.tag = "Enemy";
        rb = transform.GetComponent<Rigidbody2D>();
        trans = transform;
        if(rb == null){
            trans = transform.parent;
            rb = trans.GetComponent<Rigidbody2D>();
        }
        move = trans.GetComponent<MoveController>();   
        
        startPos = transform.position;
    }
    // Start is called before the first frame update
    virtual protected void Start()
    {
        direction = 1;
        discovered = false;
        currentAction = 1;
        moving = 0;

    }

    // Update is called once per frame
    virtual protected void Update()
    {
        
        velocity = move.ToRelativeVelocity(rb.velocity);
        Ray ray = new Ray(transform.position, move.ToRelativeVelocity(trans.forward));
        RaycastHit2D hitInfo;
        hitInfo = Physics2D.Raycast(transform.position, move.ToRelativeVelocity(new Vector2(direction, 0)), 5, LayerMask.GetMask("Rabbit"));
        // Debug.DrawLine(transform.position, (Vector2)transform.position + move.ToRelativeVelocity(new Vector3(direction * 10, 0, 0)), Color.blue);
        if(hitInfo){    // 发现目标
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
                    moving = -1;    //随机移动
                }

                if(discovered){
                    currentAction = 2;
                    waitTime = 0;
                    moving = 1;     //立即移动一次
                    StartCoroutine(Warning(false));
                }

                break;

            case 2: //追逐
                waitTime += Time.deltaTime;
                discoverTime -= Time.deltaTime;

                if(discoverTime <= 0){
                    currentAction = 1;
                    discovered = false;

                } 

                direction = Rabbit.instance.transform.position.x > transform.position.x ? 1 : -1;

                if ((float)(Random.Range(0, 1000)) / 1000 < waitTime / waitTimeMax && waitTime / waitTimeMax > 0.2f){   //等待时间越久越容易行动
                    float portion = Mathf.Abs(transform.position.x - Rabbit.instance.transform.position.x) / attackDistance;
                    portion = portion > 1 ? portion : 1;

                    int n = Random.Range(0, 100);
                    if(n > (float)attackPortion / portion){ //距离过远增加移动欲望
                        moving = 1;         //朝主角方向移动
                    }
                    else{
                        currentAction = 3;
                    }
                    waitTime = 0;
                }
                break;

            case 3: //攻击
                StartCoroutine(Warning(true));
                break;

            default:
                break;
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
        
    }


    // 移动
    virtual protected void Move(bool isChasing){
        waitTime = 0;

        if(!isChasing){
            direction = Random.Range(0, 2) == 0 ? 1 : -1;
        }
        else{
            direction = Rabbit.instance.transform.position.x > transform.position.x ? 1 : -1;
        }
        trans.localScale = new Vector2(direction, 1);
        velocity = move.SetSpeed(new Vector2(direction * speed, rb.velocity.y));
    }

    // 发现目标
    protected IEnumerator Warning(bool isAttack){
        GameObject warning = Instantiate(warningPrefab, transform.position, transform.rotation);
        warning.transform.parent = trans;
        warning.transform.localPosition = new Vector3(0.6f, 0.6f, 0);
        warning.transform.localScale = new Vector2(2, 2);

        if(!isAttack){
            yield return new WaitForSeconds(2);
            Destroy(warning);
        }
        else{
            yield return new WaitForSeconds(1);
            Destroy(warning);
            Attack(Random.Range(0, attackModeMax));
        }
    }

    // 动画控制
    protected void SetStatus(int status){
        ani.SetInteger("status", status);
    }

    // 攻击模式
    virtual protected void Attack(int mode){
        currentAction = 2;
    }
    
    // 重生
    protected void OnEnable(){
        transform.position = startPos;
        //GetComponent<Status>().Relive();
        gameObject.SetActive(true);
    }

}
