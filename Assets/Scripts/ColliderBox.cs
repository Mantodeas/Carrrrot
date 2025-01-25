using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBox : MonoBehaviour
{
    [SerializeField] bool contact;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    // void OnTriggerEnter2D(Collider2D box){
    //     Debug.Log(box.name + "enter");
    //     contact ++;
    // }

    // void OnTriggerLeave2D(Collider2D box){
    //     Debug.Log(box.name + "leave");
    //     contact --;
    // }
    

    void OnTriggerStay2D(Collider2D box){
        if(box.gameObject.layer == 10){   //地面
            contact = true;
            Debug.Log(box.name);
        }
    }

    public void SetContact(bool contact){
        this.contact = contact;
    }

    public bool isContact(){
        Debug.Log(contact);
        return contact;
    }
}
