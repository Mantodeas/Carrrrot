using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] bool enemy;
    [SerializeField] GameObject effects;
    [SerializeField] Color effectColor;
    Rigidbody2D rb;
    float liveTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float angleRad = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angleRad, Vector3.forward);

        // 存活时间结束回收
        liveTime -= Time.deltaTime;
        if(liveTime <= 0){
            Pool.instance.Release(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 重力转换
        if(other.tag == "GravityChanger" && !enemy){
            other.GetComponent<GravityDirectionController>().Change();
            Effects();
        }

        // 无效撞击，回收
        else if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Box")){
            if(Vector2.Distance(transform.position, Rabbit.instance.transform.position) < 20f)
                GameController.SoundPlay("damage", false, 0.2f);
            Effects();
        }

        // 有效撞击，造成伤害
        else if((other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !enemy) || (other.tag == "Rabbit" && enemy)){
            other.GetComponent<Status>().TakeDamage(damage);
            Effects();
            
            if(!enemy){
                Rabbit.instance.GetComponent<Rabbit>().ChangeMP(10);
            }
        }
    }

    // 击打特效
    public void Effects(){
        if(effects != null){
            GameObject eff = Instantiate(effects, transform.position, transform.rotation);
            eff.GetComponent<SpriteRenderer>().color = effectColor;
        }
        //Destroy(this.gameObject);
        Pool.instance.Release(gameObject);
    }

    public void SetEnemy(bool enemy){
        this.enemy = enemy;
    }

    public void OnEnable(){
        liveTime = 10;  //最大存活10秒
    }
}
