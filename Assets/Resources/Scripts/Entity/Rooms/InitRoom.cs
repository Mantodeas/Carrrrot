using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitRoom : MonoBehaviour
{
    [SerializeField] string roomName;
    [SerializeField] Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1080, 720, false);
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(roomName);
        Rabbit.instance.transform.position = startPos;
        GameController.cameraController.transform.position = new Vector3(startPos.x, startPos.y, GameController.cameraController.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
