using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Gun instance;
    static GameObject gunParent;
    [SerializeField] GameObject rabbit;
    [SerializeField] GameObject muzzle; // 枪口位置
    [SerializeField] float bulletSpeed; // 子弹速度
    Vector3 mouse;
    bool shoot;
    [SerializeField] float shootCDMax;
    float shootCD;  // 射击冷却


    static float angleDeg;
    // Start is called before the first frame update
    void Start()
    {
        gunParent = transform.parent.gameObject;
        shoot = false;
    }

    void Update(){
        if((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && shootCD <= 0){
            shoot = true;
            shootCD = shootCDMax;
        }
        if(shootCD > 0)
            shootCD -= Time.deltaTime;  
    }

    // Update is called once per frame
    public void GunUpdate()
    {    
        gunParent.transform.position = rabbit.transform.position;   //跟随本体

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouse - gunParent.transform.position;   //相对位置
        float angle = Mathf.Atan2(direction.y, direction.x);
        angleDeg = Mathf.Rad2Deg * angle;

        // 更新角度
        gunParent.transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);

        // 开枪
        if(shoot){
            GameObject bullet = Pool.instance.Get("bullet", muzzle.transform);
            bullet.transform.position = muzzle.transform.position;
            bullet.GetComponent<Bullet>().SetEnemy(false);
            //bullet.GetComponent<Bullet>().ReLive();

            // 速度分解
            float x = bulletSpeed * Mathf.Cos(angle);
            float y = bulletSpeed * Mathf.Sin(angle);

            bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(x, y, 0);
            shoot = false;

            GameController.SoundPlay("shoot", false, 1f);
            StartCoroutine(ShootAnimation());
        }
    }

    // 射击动画
    IEnumerator ShootAnimation(){
        int n=10;
        transform.localScale = new Vector3(0.9f, transform.localScale.y, 1);
        yield return new WaitForSeconds(0.05f);    

        for(int i = n; i > 0; i--) {
            transform.localScale = new Vector3(1 - 0.1f * transform.localScale.x * i / n, transform.localScale.y, 1);
            yield return new WaitForSeconds((shootCDMax-0.05f) / n);    
        }

    }

}
