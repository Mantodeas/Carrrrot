using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    GameObject gunParent;
    [SerializeField] GameObject rabbit;
    [SerializeField] GameObject muzzle;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletSpeed;
    Vector3 mouse;
    // Start is called before the first frame update
    void Start()
    {
        gunParent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        gunParent.transform.position = rabbit.transform.position;   //跟随本体

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mouse - gunParent.transform.position;   //相对位置
        float angle = Mathf.Atan2(direction.y, direction.x);
        float angleDeg = Mathf.Rad2Deg * angle;
        gunParent.transform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.forward);

        if(Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)){
            GameObject bullet = Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);
            bullet.GetComponent<Bullet>().SetEnemy(false);
            //float angle = muzzle.transform.rotation.eulerAngles.z;
            float x = bulletSpeed * Mathf.Cos(angle);
            float y = bulletSpeed * Mathf.Sin(angle);

            //Debug.Log(direction + " " + angle + " " + Mathf.Sin(angle) + " " + Mathf.Cos(angle));
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(x, y, 0);
        }
    }
}
