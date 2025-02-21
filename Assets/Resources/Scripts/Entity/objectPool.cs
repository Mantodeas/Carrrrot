using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectPool{
    string name;
    GameObject prefab;
    List<GameObject> prefabList;

    // 获取对象
    public GameObject Get(){
        GameObject res = null;
        if(prefabList.Count > 0){
            res = prefabList[0];
            res.SetActive(true);
            prefabList.Remove(res);
            return res;
        }
        else{
            return null;
        }
    }

    // 释放对象
    public void Release(GameObject obj){
        obj.SetActive(false);
        prefabList.Add(obj);
    }

    // 初始化对象池
    public void init(string name){
        this.name = name;
        prefab = Resources.Load<GameObject>("Prefab/" + name);
        prefabList = new List<GameObject>();
    }

    // 获取预制体
    public GameObject GetPrefab(){
        return prefab;
    }

    public void GetCount(){
        Debug.Log("count:" + prefabList.Count);
    }
}
