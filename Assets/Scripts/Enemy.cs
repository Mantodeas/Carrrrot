using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Enemy : MonoBehaviour
{
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] int currentAction;

    [SerializeField] float waitTimeMax;
    float waitTime;
    float discoverTime;
    [SerializeField] int attackPortion;
    [SerializeField] float attackDistance;

    public Animator ani;
    int aniStatu;

    [SerializeField] int direction;
    bool discovered;

    int moving;
    bool isAttack;
    [SerializeField] float fric;

    Rigidbody2D rb;
    [SerializeField] float speed;

    [SerializeField] GameObject warningPrefab;
    Vector2 velocity;
    MoveController move;
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
                        isAttack = true;
                    }
                    waitTime = 0;
                }
                break;

            case 3: //攻击
                Attack(0);
                break;

            default:
                break;
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            isAttack = true;
        }
        if(Input.GetKeyDown(KeyCode.E)){
            //Move(true);
            moving = 1;
        }
    }

    void FixedUpdate()
    {
        if(isAttack){
            Attack(0);
        }
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

    void Move(bool isChasing){
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

    IEnumerator Warning(){
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

    void SetStatus(int status){
        ani.SetInteger("status", status);
    }

    void Attack(int mode){
        switch(mode){
            case 0:            
                direction = GameController.instance.rabbit.transform.position.x > transform.position.x ? 1 : -1;
                transform.parent.localScale = new Vector2(direction, 1);    
            
                SetStatus(1);
                velocity = move.SetSpeed(new Vector2(-6 * direction, 10 / GameController.gravityScale));
                break;
            case 1:
                break;
            default:
                break;
        }
        isAttack = false;
        
    }

    void AttackMode1(){
        int n = 10;
        float dispersion = 10 * Mathf.PI / 180;
        Vector3 dir = GameController.instance.rabbit.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        angle -= (n-1) * dispersion / 2.0f;
        for(int i=0; i<n; i++){
            float angleDeg = angle * Mathf.Rad2Deg;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().SetEnemy(true);
            
            float x = bulletSpeed * Mathf.Cos(angle);
            float y = bulletSpeed * Mathf.Sin(angle);
            //Debug.Log(direction + " " + angle + " " + Mathf.Sin(angle) + " " + Mathf.Cos(angle));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(x, y, 0);

            Vector3 temp = bullet.transform.position;
            temp.x += Mathf.Cos(angle) * 0.5f;
            temp.y += Mathf.Sin(angle) * 0.5f;
            bullet.transform.position = temp;

            angle += dispersion;
            //Debug.Log(angle);
        }
    }
}
