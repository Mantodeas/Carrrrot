using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody2D rb;
    float t = 0.1f;
    float scale;    // 重力大小
    public float spriteScale;   // 图片大小
    bool stay;  // 默认状态
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        scale = 1;
        spriteScale = 1;
        stay = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(scale != GameController.gravityScale){
            StartCoroutine(WaitRandomTime());
        }

        // 根据重力位置调整状态和创建与回收位置
        if(scale == 1){
            if(transform.localPosition.x > 10)
                Pool.instance.Release(gameObject);
            GetComponent<MoveController>().SetSpeed(Vector2.Lerp(GetComponent<MoveController>().ToRelativeVelocity(rb.velocity), new Vector2(15, 0) / spriteScale, t));
            if(!stay)
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(3, 0, 1) * spriteScale, t);
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(1f, 1f, 1f, 1f), t);
        }
        else if(scale < 1){
            if(transform.localPosition.y > 6)
                Pool.instance.Release(gameObject);
            GetComponent<MoveController>().SetSpeed(Vector2.Lerp(GetComponent<MoveController>().ToRelativeVelocity(rb.velocity), new Vector2(0, 5) / spriteScale, 5));
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(3, 3, 1) * spriteScale, t / 5);
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(0.8f, 0.8f, 1f, 1f), t);
        }
        else{
            if(transform.localPosition.y < -6)
                Pool.instance.Release(gameObject);
            GetComponent<MoveController>().SetSpeed(Vector2.Lerp(GetComponent<MoveController>().ToRelativeVelocity(rb.velocity), new Vector2(0, -5) / spriteScale, 5));
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(3, -3, 1) * spriteScale, t / 5);
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, new Color(1f, 0.8f, 0.8f, 1f), t);
        }

        // 改变形态
        if(Mathf.Abs(transform.localScale.y) <= spriteScale * 0.1f && scale == 1 && !stay){
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Scene/arrow_normal");
            transform.localScale = new Vector3(transform.localScale.x, 0.4f, 1);
            GetComponent<MoveController>().SetSpeed(new Vector2(15, 0));
            stay = true;
        }
        else if(scale != 1 && stay){
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Scene/arrow");
            transform.localScale = new Vector3(transform.localScale.x, spriteScale * 0.1f * (scale > 1 ? 1 : -1), 1);
            //transform.localScale = new Vector3(transform.localScale.x, 0, 1);
            stay = false;
        }
    }

    // 随机等待时间再变化
    IEnumerator WaitRandomTime(){
        float time = (float)Random.Range(0, 500) / 100;
        Debug.Log("Time:" + time);
        yield return new WaitForSeconds(time);
        scale = GameController.gravityScale;
    }
}
