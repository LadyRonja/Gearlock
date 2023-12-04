using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public Texture2D defaultCursor, walkCursor, digCursor, fightCursor, forbiddenCursor;

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

    public void Forbidden()
    {
        Cursor.SetCursor(forbiddenCursor, Vector2.zero, CursorMode.Auto);
    }
}
