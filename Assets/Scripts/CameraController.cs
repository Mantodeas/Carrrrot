using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rabbitTransform;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rabbitTransform = GameController.instance.rabbit.transform;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(rabbitTransform.position.x, rabbitTransform.position.y, cam.transform.position.z);
        transform.rotation = Quaternion.Euler(0, 0, GameController.theta);
        transform.Find("/forest").transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Find("/forest").transform.rotation = transform.rotation;
    }
}
