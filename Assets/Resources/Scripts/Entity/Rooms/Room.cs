using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public static Room instance;
    public static BoxCollider2D boxCollider2D;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
//        transform.Find("/Canvas").GetComponent<Canvas>().render
    }
    virtual protected void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        GameController.SetCameraCollider();
    }

    // Update is called once per frame
    virtual protected void Update()
    {

    }

}
