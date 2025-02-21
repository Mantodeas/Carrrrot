using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gate : MonoBehaviour
{
    [SerializeField] string roomName;
    [SerializeField] Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Rabbit"){  //传送
            SceneManager.LoadScene(roomName);
            Rabbit.instance.transform.position = pos;
            GameController.cameraController.transform.position = new Vector3(pos.x, pos.y, GameController.cameraController.transform.position.z);
            GameController.instance.ChangeTheta(0);
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")){  //敌人掉出世界就销毁
            Destroy(other.gameObject);
        }
    }
}
