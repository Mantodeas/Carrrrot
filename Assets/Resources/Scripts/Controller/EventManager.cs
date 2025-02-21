using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class EventManager : MonoBehaviour
{
    private static Dictionary<string, Action> events;

    private static EventManager instance;

    void Awake()
    {
        if(instance == null){
            instance = GetComponent<EventManager>();
        }
        else{
            Destroy(gameObject);
        }
        events = new Dictionary<string, Action>();
    }

    // 开始监听事件
    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            events[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            events.Add(eventName, thisEvent);
        }
    }

    // 结束监听事件
    public static void StopListening(string eventName, Action listener)
    {
        if (instance == null) return;
        Action thisEvent;
        if (events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            events[eventName] = thisEvent;
        }
    }

    // 触发事件
    public static void TriggerEvent(string eventName)
    {
        Action thisEvent;
        if (events.TryGetValue(eventName, out thisEvent))
        {
            thisEvent?.Invoke();
        }
    }
}