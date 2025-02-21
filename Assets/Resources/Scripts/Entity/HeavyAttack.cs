using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttack : MonoBehaviour
{
    [SerializeField] float damage;
    List<GameObject> targets;
    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 伤害敌人
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !targets.Contains(other.gameObject)){
            other.GetComponent<Status>().TakeDamage(damage);
            other.GetComponent<Enemy>().trans.GetComponent<MoveController>().AddSpeed(new Vector2(2 * (Rabbit.instance.transform.localScale.x>0?1:-1), 0));
            GameController.SoundPlay("damage", false, 0.2f);
            targets.Add(other.gameObject);
        }

        // 消除子弹
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy Bullet") && !targets.Contains(other.gameObject)){
            Rabbit.instance.ChangeMP(20);
            other.GetComponent<Bullet>().Effects();
            targets.Add(other.gameObject);
        }
    }

    public void SetActiveTrue(){
        GetComponent<PolygonCollider2D>().enabled = true;
    }

    public void SetActiveFalse(){
        targets.Clear();
        GetComponent<PolygonCollider2D>().enabled = false;
    }
    public bool AttackGround(){
        return transform.Find("GroundCheck").GetComponent<GroundCheck>().isContact();
    }
}
