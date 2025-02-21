using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    Rigidbody2D rb;
    public bool ready;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }
    void Start()
    {
        ready = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.gravityScale == 1){
            rb.gravityScale = 0;
        }
        else if(GameController.gravityScale > 1){
            rb.gravityScale = 1;
        }
        else{
            rb.gravityScale = -1;
        }
        
        
    }

    public void SetReady(bool ready){
        this.ready = ready;
        if(ready)
            rb.bodyType = RigidbodyType2D.Dynamic;
        else
            rb.bodyType = RigidbodyType2D.Static;
    }
}
