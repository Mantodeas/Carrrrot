using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pool : MonoBehaviour
{
    public static Pool instance;
    public Dictionary<string, objectPool> pools;
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
        }

        pools = new Dictionary<string, objectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 获取对象
    public GameObject Get(string name, Transform trans){
        objectPool pool;
        if(pools.TryGetValue(name, out pool)){
            GameObject res = pool.Get();

            // 对象池中无剩余对象
            if(!res){
                res = Instantiate(pool.GetPrefab(), trans.position, trans.rotation);                
                res.name = name;
                DontDestroyOnLoad(res);
                return res;
            }
            else{
                res.transform.position = trans.position;
                res.transform.rotation = trans.rotation;
            }
            return res;
        }

        // 无此对象池，新建
        else{
            pool = new objectPool();
            pool.init(name);
            pools.Add(name, pool);
            GameObject res = Instantiate(pool.GetPrefab(), trans.position, trans.rotation);
            res.name = name;
            DontDestroyOnLoad(res);
            return res;
        }
    }

    // 释放对象
    public void Release(GameObject obj){
        objectPool pool;
        if(!pools.TryGetValue(obj.name, out pool)){
            pool = new objectPool();
            pool.init(name);
            pools.Add(name, pool);
        }
        pool.Release(obj);
    }

}
