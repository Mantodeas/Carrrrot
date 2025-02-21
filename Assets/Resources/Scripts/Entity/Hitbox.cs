using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            Debug.Log(other.gameObject.name + " " + LayerMask.LayerToName(other.gameObject.layer));
            Physics2D.IgnoreCollision(transform.parent.GetComponent<BoxCollider2D>(), other, true);
            transform.parent.GetComponent<Status>().TakeDamage(-0.2f);   //特殊值，代表失去20%的血量
        }
    }
}
