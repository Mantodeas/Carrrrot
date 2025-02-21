using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Brush : Enemy
{
    // Start is called before the first frame update
    protected override void Move(bool isChasing){
        waitTime = 0;

        // 追逐模式固定方向移动，巡逻模式随机移动
        if(!isChasing){
            direction = Random.Range(0, 2) == 0 ? 1 : -1;
        }
        else{
            direction = Rabbit.instance.transform.position.x > transform.position.x ? 1 : -1;
        }
        transform.parent.localScale = new Vector2(direction, 1);
        velocity = move.SetSpeed(new Vector2(direction * speed, rb.velocity.y));
    }

    protected override void Attack(int mode){
        switch(mode){
            case 0:            
                direction = Rabbit.instance.transform.position.x > transform.position.x ? 1 : -1;
                transform.parent.localScale = new Vector2(direction, 1);    
            
                SetStatus(1);
                velocity = move.SetSpeed(new Vector2(-6 * direction, 10 / GameController.gravityScale));
                break;
            case 1:
                break;
            default:
                break;
        }
        isAttack = false;
        
    }

    void AttackMode1(){
        velocity = move.SetSpeed(new Vector2(velocity.x/2, velocity.y));

        int n = 10;
        float dispersion = 10 * Mathf.PI / 180;
        Vector3 dir = Rabbit.instance.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x);
        angle -= (n-1) * dispersion / 2.0f;

        // 散射子弹
        for(int i=0; i<n; i++){
            GameObject bullet = Pool.instance.Get("brush-attack", transform);
            bullet.transform.position = transform.position;
            bullet.GetComponent<Bullet>().SetEnemy(true);
            //bullet.GetComponent<Bullet>().ReLive();
            
            float x = bulletSpeed * Mathf.Cos(angle);
            float y = bulletSpeed * Mathf.Sin(angle);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector3(x, y, 0);

            Vector3 temp = bullet.transform.position;
            temp.x += Mathf.Cos(angle) * 0.5f;
            temp.y += Mathf.Sin(angle) * 0.5f;
            bullet.transform.position = temp;

            angle += dispersion;

        }
        currentAction = 2;
    }
}
