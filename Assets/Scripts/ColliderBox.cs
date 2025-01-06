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

    void OnTriggerEnter2D(Collider2D box){
        contact = true;
    }

    void OnTriggerLeave2D(Collider2D box){
        contact = false;
    }

    public void SetContact(bool contact){
        this.contact = contact;
    }

    public bool isContact(){
        return contact;
    }
}
