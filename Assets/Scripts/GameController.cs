using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public GameObject rabbit;
    public static float theta;

    [SerializeField] public static float gravityNormal = 3.5f;

    public static float gravityScale;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)){
            ChangeTheta(90);
        }
    }

    public void ChangeTheta(float delta){
        StartCoroutine("GravityChange", delta);
    }

    IEnumerator GravityChange(float delta){
        int n=12;
        for(int i=0; i<n; i++){
            theta += delta / n;
            Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
            yield return new WaitForSeconds(0.01f);
        }

    }

    public static void GravityScaleChange(float scale){
        gravityScale = scale;
        Physics2D.gravity = new Vector2(9.8f * gravityScale * Mathf.Sin(theta * Mathf.Deg2Rad), -9.8f * gravityScale * Mathf.Cos(theta * Mathf.Deg2Rad));
    }
}
