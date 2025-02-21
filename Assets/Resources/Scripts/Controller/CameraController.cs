using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    Transform rabbitTransform;
    Vector3 movePos;    //跟随主角

    float width = 9, height = 5;    //摄像头大小
    [SerializeField] float xMin, xMax, yMin, yMax;  //位置边界

    Coroutine shakeCoroutine;   //抖动协程
    Coroutine bgEffectCoroutine;    //背景特效协程

    [SerializeField] float arrowTime;   //生成箭头时间间隔
    GameObject bgEffect;    //背景特效
    int gravityState;   //重力大小

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rabbitTransform = Rabbit.instance.transform;
        bgEffect = transform.Find("bgEffect").gameObject;

        StartCoroutine(CreateArrow());
    }

    void Update()
    {
        if(GameController.gravityScale < 1){
            gravityState = 1;
        }
        else if(GameController.gravityScale > 1){
            gravityState = -1;
        }
        else{
            gravityState = 0;
        }
        
        if(bgEffectCoroutine == null)
            if(gravityState > 0)
                bgEffectCoroutine = StartCoroutine(BGEffect(new Color(0.5f, 0.7f, 1f, 1f)));    //弱重力蓝色
            else if(gravityState < 0)
                bgEffectCoroutine = StartCoroutine(BGEffect(new Color(1f, 0.3f, 0.3f, 1)));     //强重力红色
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movePos.Set(rabbitTransform.position.x, rabbitTransform.position.y, cam.transform.position.z);
        movePos.x = movePos.x < xMin ? xMin : movePos.x;
        movePos.y = movePos.y < yMin ? yMin : movePos.y;
        movePos.x = movePos.x > xMax ? xMax : movePos.x;
        movePos.y = movePos.y > yMax ? yMax : movePos.y;
        
        cam.transform.position = Vector3.Lerp(cam.transform.position, movePos, 0.1f);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, Quaternion.Euler(0, 0, GameController.theta), 0.25f);

        GameController.AfterCamareUpdate(); //防止抖动
    }

    // 计算边界大小
    public void SetCollider()   
    {
        Collider2D collider = Room.boxCollider2D;
        Vector2 direction = width * GameController.moveDirection + height * GameController.jumpDirection;

        xMin = collider.bounds.center.x - collider.bounds.size.x / 2 + Mathf.Abs(direction.x);
        yMin = collider.bounds.center.y - collider.bounds.size.y / 2 + Mathf.Abs(direction.y);
        xMax = collider.bounds.center.x + collider.bounds.size.x / 2 - Mathf.Abs(direction.x);
        yMax = collider.bounds.center.y + collider.bounds.size.y / 2 - Mathf.Abs(direction.y);
    }

    // 摄像机抖动
    public void ShakeCamera(float shakeStrength, float shakeDuration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeCoroutine(shakeStrength, shakeDuration));
    }

    // 震动协程
    private IEnumerator ShakeCoroutine(float shakeStrength, float shakeDuration)    
    {
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // 计算当前震动偏移
            float x = UnityEngine.Random.Range(-shakeStrength, shakeStrength);
            float y = UnityEngine.Random.Range(-shakeStrength, shakeStrength);

            transform.position += new Vector3(x, y, 0);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        shakeCoroutine = null;
    }

    // 背景箭头生成
    IEnumerator CreateArrow(){
        while(true){
            GameObject arrow = Pool.instance.Get("Arrow", transform);
            float scale = Mathf.Pow(UnityEngine.Random.Range(3, 27) *  UnityEngine.Random.Range(3, 27), 0.5f) / 9;
            arrow.GetComponent<Arrow>().spriteScale = scale;

            if(gravityState == 0){
                arrow.transform.localPosition = new Vector3(-12, (float)UnityEngine.Random.Range(-50, 50) / 10, 10);
            }
            else if(gravityState == 1){
                arrow.transform.localPosition = new Vector3((float)UnityEngine.Random.Range(-90, 90) / 10, -12, 10);
            }
            else
                arrow.transform.localPosition = new Vector3((float)UnityEngine.Random.Range(-90, 90) / 10, +12, 10);
            arrow.transform.parent = transform;
            arrow.name = "Arrow";
            yield return new WaitForSeconds(arrowTime * scale);
        }
    }

    // 背景特效控制
    IEnumerator BGEffect(Color color){
        float startTime = 0;
        float f = MathF.PI * 10;
        bgEffect.GetComponent<SpriteRenderer>().enabled = true;
        if(gravityState == 1){
            bgEffect.transform.localPosition = new Vector3(0, -5f, 10);
            bgEffect.transform.rotation = Quaternion.Euler(0, 0, 0 + transform.rotation.eulerAngles.z);
        }
        else{
            bgEffect.transform.localPosition = new Vector3(0, 5f, 10);
            bgEffect.transform.rotation = Quaternion.Euler(0, 0, 180 + transform.rotation.eulerAngles.z);
        }

        int n=20;
        for(int i = 0; i <= n; i++) {
            color.a = (float)i / n / 2;
            bgEffect.GetComponent<SpriteRenderer>().color = color;
            bgEffect.transform.localScale = new Vector3(1.8f, 1.7f * i / n, 1);  
            yield return new WaitForSeconds(0.2f / n);
        }
        
        while(true){
            startTime += Time.deltaTime;
            color.a = (1 - Mathf.Cos(startTime * f)) / 4 + 0.5f;
            bgEffect.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.05f);
            if(gravityState == 0)
                break;
        }

        float alpha = color.a;
        for(int i = n; i > 0; i--) {
            color.a = alpha * i / n;
            bgEffect.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.2f / n);
        }

        bgEffect.GetComponent<SpriteRenderer>().enabled = false;
        bgEffectCoroutine = null;
    }
}
