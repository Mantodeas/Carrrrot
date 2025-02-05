using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] Vector2 pos;
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
        if (other.gameObject.tag == "Rabbit"){
            GameController.instance.rabbit.transform.position = pos;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
            Destroy(other.gameObject);
        }
    }
}
