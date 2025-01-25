using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject rabbit;
    [SerializeField] float bulletSpeed;
    [SerializeField] GameObject bulletPrefab;

    int currentAction;

    [SerializeField] float waitTimeMax;
    float waitTime;

    public Animator ani;
    int aniStatu;

    [SerializeField] int direction;
    bool discovered;

    int moving;
    [SerializeField] float fric;

    Rigidbody2D rb;
    [SerializeField] float speed;

    [SerializeField] GameObject warn;
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "Enemy";
        rb = transform.parent.GetComponent<Rigidbody2D>();

        direction = 1;
        discovered = false;
        currentAction = 1;
        moving = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Random.Range(0, 2));
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit2D hitInfo;
        hitInfo = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 5, LayerMask.GetMask("Rabbit"));
        Debug.DrawLine(transform.position, transform.position + new Vector3(direction * 10, 0, 0), Color.blue);
        if(hitInfo){
            //Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue);
            discovered = true;
        }
        else{
            discovered = false;
        }

        switch(currentAction){
            case 0: //等待
                break;

            case 1: //巡逻
                waitTime += Time.deltaTime;
                int action = Random.Range(0, 1000);
                if ((float)action / 1000 < waitTime / waitTimeMax){
                    //Debug.Log((float)action / 1000 + " " +  waitTime + " " + waitTimeMax);
                    moving = -1;
                }
                else{
                    moving = 0;
                }

                if(discovered){
                    currentAction = 2;
                }

                break;

            case 2: //追逐
                moving = 1;
                break;

            case 3: //攻击
                Attack(0);
                break;

            default:
                break;
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            Debug.Log("Att");
            SetStatus(1);
            rb.velocity = new Vector2(-6 * direction, 10);
                
        }
    }

    void FixedUpdate()
    {
        if(moving != 0){
            if(moving < 0)
                Move(false);
            else
                Move(true);
            
        }
        

        {
            Vector2 temp = rb.velocity;
            temp.x -= temp.x * fric * Time.deltaTime;
            rb.velocity = temp;
        }
    }

    void Move(bool isChasing){
        //Debug.Log("Moving");
        waitTime = 0;
        if(!isChasing){
            direction = Random.Range(0, 2) == 0 ? 1 : -1;
            transform.parent.localScale = new Vector2(direction, 1);
            rb.velocity = new Vector2(direction * speed, rb.velocity.y);
        }
    }

    void SetStatus(int status){
        ani.SetInteger("status", status);
    }

    void Attack(int mode){
        switch(mode){
            case 0:
                break;
            case 1:
                break;
            default:
                break;
        }
        int n = 10;
        float dispersion = 10 * Mathf.PI / 180;
        Vector3 dir = rabbit.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        angle -= (n-1) * dispersion / 2.0f;
        for(int i=0; i<n; i++){
            float angleDeg = angle * Mathf.Rad2Deg;
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            
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
