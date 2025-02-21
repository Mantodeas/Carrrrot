using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;

public class GravityDirectionController : MonoBehaviour
{
    float theta;    // 重力角度控制
    bool change;    // 是否触发
    // Start is called before the first frame update
    void Start()
    {
        change = false;
        theta = transform.rotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(change){
            change = false;
            GameController.instance.ChangeTheta(theta);
        }
    }

    // 改变重力
    public void Change(){
        change = true;
        StartCoroutine(GravityChanging());
    }

    // 闪动特效
    IEnumerator GravityChanging(){
        GetComponent<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.5f);    
        GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }
}
