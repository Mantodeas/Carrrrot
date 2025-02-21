using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] float HPMax;
    [SerializeField] float HP;
    public bool invincible;
    // Start is called before the first frame update
    void Start()
    {
        HP = HPMax;
        invincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    // 受伤
    public void TakeDamage(float damage){
        // 无敌时间
        if(invincible){
            return ;
        }

        if(damage < 0f){    // 负数值为百分比伤害
            HP += HPMax * damage;
        }
        else{
            HP -= damage;
        }
        if (HP < 0)
            Die();

        if(this.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            if(this.gameObject.tag == "Boss"){  //Boss除了有普通受击动画还有无敌时间
                
            }
            else{                               //杂兵只有普通受击动画
                StartCoroutine("Hurt", false);
                GetComponent<Enemy>().discovered = true;
            }
        }
        else if(this.gameObject.layer == LayerMask.NameToLayer("Rabbit")){   //主角无论何时都有无敌时间，还需更改血量显示
            GetComponent<Rabbit>().SetHP(HP / HPMax);
            GameController.SoundPlay("hurt", false, 1f);
            StartCoroutine(Hurt(true));
        }
    }

    // 死亡
    private void Die(){
        if(gameObject.tag != "Rabbit"){
            Destroy(GetComponent<Enemy>().trans.gameObject);
        }
        else{
            Debug.Log("YOU DIED!!");
        }
    }

    // 受伤特效
    IEnumerator Hurt(bool heavy){
        GameController.SoundPlay("damage", false, 1f);
        invincible = true;
        transform.GetComponent<SpriteRenderer>().color = new Color(1f, 0.4f, 0.4f);
        yield return new WaitForSeconds(0.2f);
        if(heavy){
            int n = 12;
            for(int i = 0; i < n; i++) {
                transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                yield return new WaitForSeconds(0.08f);
                transform.GetComponent<SpriteRenderer>().color = new Color(1f, 0.8f, 0.8f);
                yield return new WaitForSeconds(0.08f);
            }
        }
        transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        invincible = false;
    }

    // 重置
    public void OnEnable(){
        HP = HPMax;
    }

}
