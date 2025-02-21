using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Catch : MonoBehaviour
{
    List<GameObject> item;
    // Start is called before the first frame update
    void Start()
    {
        item = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Box")){
            item.Add(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Box")){
            item.Remove(other.gameObject);
        }
    }

    public GameObject GetItem(){
        if(item.Count == 0){
            return null;
        }
        else{
            return item[0];
        }
    }
}
