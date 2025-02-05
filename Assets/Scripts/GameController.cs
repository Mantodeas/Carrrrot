using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject rabbit;
    public static GameObject gun;
    public static float theta;

    [SerializeField] public static float gravityNormal = 3.5f;

    public static float gravityScale;

    public static CameraController cameraController;

    void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject); // 如果需要跨场景持久化
        }
        else{
            Destroy(gameObject);
        }

        gravityScale = 1;
    }
    // Start is called before the first frame update
    void Start()
    {
        gun = transform.Find("/Gun").Find("GunSprite").gameObject;
        cameraController = transform.Find("/Main Camera").GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            ChangeTheta(theta + 90);
        }
    }

    public void ChangeTheta(float delta){
        delta -= theta;
        if(delta > 180){ delta -= 360;}
        StartCoroutine("GravityChange", delta);
    }

    IEnumerator GravityChange(float delta){
        int n=12;
        for(int i=0; i<n; i++){
           theta += delta / n;
            //theta = delta;
            Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
            yield return new WaitForSeconds(0.01f);
        }
        SetCameraCollider(instance.transform.Find("RoomCollider").GetComponent<Collider2D>());
    }

    // public void GravityChange(float delta){
    //     theta = delta;
    //     Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
    //     SetCameraCollider(instance.transform.Find("RoomCollider").GetComponent<Collider2D>());
    // }

    public static void GravityScaleChange(float scale){
        gravityScale = scale;
        Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
    }

    public static void AfterCamareUpdate(){
        gun.GetComponent<Gun>().GunUpdate();
    }

    public static void SetCameraCollider(Collider2D collider){
        cameraController.GetComponent<CameraController>().SetCollider(collider);
    }

}
