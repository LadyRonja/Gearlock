using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicsRayCastAssistance : MonoBehaviour
{
    public static GraphicsRayCastAssistance Instance { get => GetInstance(); private set => instance = value; }
    private static GraphicsRayCastAssistance instance;

    public GraphicRaycaster caster;
    public EventSystem eventSystem;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        caster = FindObjectOfType<GraphicRaycaster>();
        eventSystem= FindObjectOfType<EventSystem>();
    }

    public static GraphicsRayCastAssistance GetInstance()
    {
        if (instance != null) return instance;

        GameObject newManager = new GameObject();
        instance = newManager.AddComponent<GraphicsRayCastAssistance>();
        newManager.transform.name = "Graphics RayCast Manager";
        return instance;

    }

}
