using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] bool enemy;
    Rigidbody2D rb;
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
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Debug.Log(other.name + " " + enemy + " " + other.tag);
        if(other.tag == "Ground"){
            Destroy(this.gameObject);
        }
        else if((other.tag == "Enemy" && !enemy) || (other.tag == "Rabbit" && enemy)){
            other.GetComponent<Status>().TakeDamage(damage);
            //Debug.Log("attack" + other.name );
            //播放动画音效
            Destroy(this.gameObject);
        }
    }

    public void SetEnemy(bool enemy){
        this.enemy = enemy;
    }
}
