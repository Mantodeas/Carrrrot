using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TrapController : MonoBehaviour
{
    [SerializeField] float waitTimeMax;
    float waitTime;
    public float damagePortion;
    // Start is called before the first frame update
    void Start()
    {
        //waitTime = waitTimeMax;
        transform.tag = "Trap";
        StartCoroutine(name);
        //Debug.Log("Start");
    }

    // Update is called once per frame
    void Update()
    {
    //     if(waitTime > 0){
    //         waitTime -= Time.deltaTime;
    //     }
    //     else{
    //         Attack();
    //     }
    }

    // public void Attack(){
    //     StartCoroutine(name);
    // }

    IEnumerator Spike(){
        //Debug.Log("Start");
        int n = 10;
        for(int i = 0; i < n; i++) {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1f / n, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < n; i++) {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1f / n, transform.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(name);
        //waitTime = waitTimeMax;
    }
}
