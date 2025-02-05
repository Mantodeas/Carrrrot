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

    public void TakeDamage(float damage){
        if(invincible){
            Debug.Log("INVINCIBLE");
            return ;
        }
        if(damage == -0.2f){    //特殊值
            HP -= HPMax * 0.2f;
        }
        else{
            HP -= damage;
        }
        if (HP < 0)
            Die();
            Debug.Log(transform.name + " " + transform.gameObject.layer + " " + LayerMask.NameToLayer("Rabbit"));
        if(this.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            if(this.gameObject.tag == "Boss"){  //Boss除了有普通受击动画还有无敌时间
                
            }
            else{                               //杂兵只有普通受击动画
                StartCoroutine("Hurt", false);
            }
        }
        else if(this.gameObject.layer == LayerMask.NameToLayer("Rabbit")){   //主角无论何时都有无敌时间，还需更改血量显示
            GetComponent<Rabbit>().ChangeHPBar(HP / HPMax);
            StartCoroutine("Hurt", true);
            //Debug.Log("Current HP:" + HP);
        }
    }

    private void Die(){
        if(this.gameObject.tag != "Rabbit"){
            Destroy(this.gameObject);
        }
        else{
            Debug.Log("YOU DIED!!");
        }
    }

    IEnumerator Hurt(bool heavy){
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

}
