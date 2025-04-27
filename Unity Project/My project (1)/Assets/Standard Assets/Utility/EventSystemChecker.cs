using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    void Awake()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
    }
}
