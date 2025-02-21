using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignBase : MonoBehaviour
{
    int count;

    [SerializeField] Sprite sensor_off, sensor_on;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    // 触发
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Rabbit") || other.gameObject.layer == LayerMask.NameToLayer("Box")){
            count ++;
            if(count == 1){
                EventManager.TriggerEvent(name + "_on");
                GetComponent<SpriteRenderer>().sprite = sensor_on;
            }
        }
        
    }

    // 松开
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Rabbit") || other.gameObject.layer == LayerMask.NameToLayer("Box")){
            count --;
            if(count == 0){
                EventManager.TriggerEvent(name + "_off");
                GetComponent<SpriteRenderer>().sprite = sensor_off;
            }
        }
    }
}