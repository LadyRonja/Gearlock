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

        caster = gameObject.GetComponent<GraphicRaycaster>();
        if(caster == null)
        {
            Debug.LogError("No GraphicRaycaster found on GRCA, this will cause issues");
        }
        eventSystem= FindObjectOfType<EventSystem>();
    }

    public static GraphicsRayCastAssistance GetInstance()
    {
        if (instance != null) return instance;

        Debug.LogError("Had to create a new GraphicsRayCastAssistance instance, this might cause problems");
        GameObject newManager = new GameObject();
        instance = newManager.AddComponent<GraphicsRayCastAssistance>();
        newManager.transform.name = "Graphics RayCast Manager";
        return instance;

    }

}
