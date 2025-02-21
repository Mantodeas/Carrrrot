using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static GameObject gun;
    public static float theta;  // 重力角度

    [SerializeField] public static float gravityNormal = 3.5f;  // 原重力大小

    public static float gravityScale;   // 重力大小
    public bool gravityChange;  // 重力改变
    public float thetaChange;   // 角度改变
    public static Vector2 moveDirection, jumpDirection; // 重力对应的x，y方向

    public static CameraController cameraController;    // 摄像机控制
    public static AudioManager audioManager;    // 音频控制

    public static Room room;    // 房间控制
    //public static BoxCollider2D roomCollider;

    void Awake()
    {
        if(instance == null){
            instance = this;
            //DontDestroyOnLoad(gameObject); // 如果需要跨场景持久化
        }
        else{
            Destroy(gameObject);
        }

        gun = transform.Find("/Base/Gun/GunSprite").gameObject;
        cameraController = transform.Find("/Base/Main Camera").GetComponent<CameraController>();
        audioManager = transform.Find("AudioManager").gameObject.GetComponent<AudioManager>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        gravityScale = 1;
        gravityChange = false;   

        jumpDirection = new Vector2(0, 1);
        moveDirection = new Vector2(1, 0);

        room = Room01.instance;
        //Room01.instance.OnEnter();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug
        if(Input.GetKeyDown(KeyCode.O)){
            gravityChange = true;
            thetaChange = 90;
        }
        if(Input.GetKeyDown(KeyCode.P)){
            gravityChange = true;
            thetaChange = -90;
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            Time.timeScale = 1 - Time.timeScale;
        }
    }

    void FixedUpdate()
    {
        if(gravityChange){
            gravityChange = false;
            ChangeTheta(theta + thetaChange);
        }
    }

    // 改变重力角度
    public void ChangeTheta(float delta){
        delta -= theta;
        if(delta > 180){ delta -= 360;}
        GravityChange(delta);
    }

    // 重力改变
    public void GravityChange(float delta){
        theta += delta;
        theta %= 360;
        
        Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));

        jumpDirection = -Physics2D.gravity.normalized;
        moveDirection = new Vector2(jumpDirection.y, -jumpDirection.x);

        SetCameraCollider();
    }

    // 重力大小改变
    public static void GravityScaleChange(float scale){
        gravityScale = scale;
        Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
    }

    // 更新发射装置
    public static void AfterCamareUpdate(){
        gun.GetComponent<Gun>().GunUpdate();
    }

    // 设置摄像头边界
    public static void SetCameraCollider(){
        cameraController.GetComponent<CameraController>().SetCollider();
    }

    // 音效播放
    public static void SoundPlay(string name, bool isLoop, float volume){
        audioManager.Play(name, isLoop, volume);
    }

    // 摄像头震动
    public static void ShakeCamera(float shakeStrength, float shakeDuration){
        cameraController.GetComponent<CameraController>().ShakeCamera(shakeStrength, shakeDuration);
    }
}
