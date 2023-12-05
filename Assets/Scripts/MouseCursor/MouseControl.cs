using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public Texture2D defaultCursor, walkCursor, digCursor, fightCursor, pickupCursor;

    public static MouseControl instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        Default();
    }

    public void Default()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void Walk()
    {
        Cursor.SetCursor(walkCursor, Vector2.zero, CursorMode.Auto);
    }

    public void Dig()
    {
        Cursor.SetCursor(digCursor, Vector2.zero, CursorMode.Auto);
    }
    public void Fight()
    {
        Cursor.SetCursor(fightCursor, Vector2.zero, CursorMode.Auto);
    }


    public void Pickup()
    {
        Cursor.SetCursor(pickupCursor, Vector2.zero, CursorMode.Auto);
    }
}
