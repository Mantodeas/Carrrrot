using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class TrapController : MonoBehaviour
{
    //[SerializeField] float waitTimeMax;
    float waitTime; // 初次启动延时
    //public float damagePortion;
    // Start is called before the first frame update
    void Start()
    {
        transform.tag = "Trap";
        if(waitTime > 0)
            Wait(waitTime); 
        else
            StartCoroutine(name);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初始等待时间
    IEnumerator Wait(float time){
        yield return new WaitForSeconds(time);
        StartCoroutine(name);
    }

    // 伸缩刺
    IEnumerator Spike(){
        int n = 10;
        float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        WaitForSeconds waitTimeShort = new WaitForSeconds(0.01f);
        WaitForSeconds waitTimeLong = new WaitForSeconds(1f);
        while(true){
            if(Vector2.Distance(transform.position, Rabbit.instance.transform.position) < 20f){
                for(int i = 0; i < n; i++) {
                    transform.position = transform.position + direction * 1 / n;
                    yield return waitTimeShort;
                }
                yield return waitTimeLong;
                for(int i = 0; i < n; i++) {
                    transform.position = transform.position - direction * 1 / n;
                    yield return waitTimeShort;
                }
                yield return waitTimeLong;
            }
            else
                yield return null;
        }
    }

    // 泡泡机
    IEnumerator BubbleShooter(){
        //bubble.GetComponent<Bullet>().ReLive();
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector3 speed = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        GameObject bubble;
        speed *= 3;
        while(true){
            if(Vector2.Distance(transform.position, Rabbit.instance.transform.position) < 20f){
                GameController.SoundPlay("bubble", false, 1);
                bubble = Pool.instance.Get("Bubble", transform);
                bubble.GetComponent<Bubble>().SetReady(false);
                bubble.transform.position = bubble.transform.position + speed.normalized * 0.8f;

                int n=10;
                for(int i = 0; i < n; i++) {
                    bubble.transform.localScale = new Vector3((float)i / n * 3, (float)i / n * 3, 1);
                    yield return new WaitForSeconds(0.5f / n);
                }
                bubble.GetComponent<Bubble>().SetReady(true);
                bubble.GetComponent<Rigidbody2D>().velocity = speed;
                yield return new WaitForSeconds(0.5f);
            }
            else 
                yield return null;
        }
    }
}

