using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] string controlSensorId;
    [SerializeField] bool duplicate;

    int openStatus;
    float openPortion;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(controlSensorId + "_on", Open);
        if(duplicate)
            EventManager.StartListening(controlSensorId + "_off", Close);

        openPortion = 0;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 开关门
        if(openStatus != 0){
            openPortion += (openStatus>0?1:-1) * Time.deltaTime * 5;
            if(openPortion > 1){
                openPortion = 1;
                openStatus = 0;
            }
            if(openPortion < 0){
                openPortion = 0;
                openStatus = 0;
            }
        }

        transform.position = new Vector3(startPos.x, startPos.y + 2f * openPortion, startPos.z);
    }

    void Open(){
        openStatus = 1;
    }

    void Close(){
        openStatus = -1;
    }
}
