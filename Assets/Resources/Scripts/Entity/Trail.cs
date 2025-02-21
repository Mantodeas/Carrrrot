using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] float waitTime;    // 残影存在时间
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator KillSelf(){
        int n=10;
        for(int i = n; i >= 0; i--) {
            Color color = transform.GetComponent<SpriteRenderer>().color;
            color.a = 0.7f * i / n;
            transform.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(waitTime / n);   
        }
        Pool.instance.Release(gameObject);
    }

    public void Init(){
        StartCoroutine(KillSelf());
    }
}
