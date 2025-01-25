using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [SerializeField] float HPMax;
    [SerializeField] float HP;
    // Start is called before the first frame update
    void Start()
    {
        HP = HPMax;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage){
        HP -= damage;
        if (HP < 0)
            Die();
    }

    private void Die(){
        if(this.gameObject.tag != "Rabbit"){
            Destroy(this.gameObject);
        }
        else{
            Debug.Log("YOU DIED!!");
        }
    }
}
