using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;

public class GravityDirectionController : MonoBehaviour
{
    [SerializeField] float theta;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, GameController.instance.rabbit.transform.position) < 2f){
            GetComponent<SpriteRenderer>().color = Color.white;
            active = true;
        }
        else{
            GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
            active = false;
        }

        if(active && Input.GetKeyDown(KeyCode.V)){
            GameController.instance.ChangeTheta(theta);
        }
    }
}
