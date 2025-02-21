using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWave : MonoBehaviour
{
    [SerializeField] float damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 无效攻击，销毁自身
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Box")){
            GameObject eff = Pool.instance.Get("bulletEff", transform.Find("eff"));
            eff.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.7f);
            eff.transform.localScale = new Vector3(6f, 6f ,1f);
            Destroy(gameObject);
        }

        // 对敌人造成伤害
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            other.GetComponent<Enemy>().GetComponent<Status>().TakeDamage(damage);
            GameObject eff = Pool.instance.Get("bulletEff", transform.Find("eff"));
            eff.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.5f, 0.7f);
            eff.transform.localScale = new Vector3(6f, 6f ,1f);
            Destroy(gameObject);
        }
    }
}
