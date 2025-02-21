using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Room01 : Room
{
    // Start is called before the first frame update
    protected override void Start()
    {
        // EventManager.StartListening("sensor_001_on", door_01);
        // EventManager.StartListening("sensor_002_on", door_01);
        base.Start();

    }

    // Update is called once per frame
    protected override void Update()
    {
        if(transform.Find("/EnemyList").childCount == 0){
            EventManager.TriggerEvent("Kill_001_on");
        }
    }

    void door_01(){
        
    }


}
