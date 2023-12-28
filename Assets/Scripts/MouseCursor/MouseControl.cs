using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public enum Cursors
{
    Default,
    Walk,
    Dig,
    Fight,
    Pickup,
    Construct,
    Dynamite
}
public class MouseControl : MonoBehaviour
{
    public Texture2D defaultCursor, walkCursor, digCursor, fightCursor, pickupCursor, constructCursor, dynamiteCursor;
    public Texture2D defaultCursorRed, walkCursorRed, digCursorRed, fightCursorRed, pickupCursorRed, constructCursorRed, dynamiteCursorRed;

    public static MouseControl Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if(dynamiteCursor== null)
            dynamiteCursor = Resources.Load<Texture2D>("Graphics/dynamite_Cursor");

        if (dynamiteCursorRed == null)
            dynamiteCursorRed = Resources.Load<Texture2D>("Graphics/dynamite_Cursor_Red");


        SetCursor(Cursors.Default, true);
    }

    public void SetCursor(Cursors cursorMode, bool white)
    {
        switch (cursorMode)
        {
            case Cursors.Default:
                if(white)
                    Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(defaultCursorRed, Vector2.zero, CursorMode.Auto);

                break;
            case Cursors.Walk:
                if (white)
                    Cursor.SetCursor(walkCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(walkCursorRed, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.Dig:
                if (white)
                    Cursor.SetCursor(digCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(digCursorRed, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.Fight:
                if (white)
                    Cursor.SetCursor(fightCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(fightCursorRed, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.Pickup:
                if (white)
                    Cursor.SetCursor(pickupCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(pickupCursorRed, Vector2.zero, CursorMode.Auto);
                break;
            case Cursors.Construct:
                if (white)
                    Cursor.SetCursor(constructCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(constructCursorRed, Vector2.zero, CursorMode.Auto);
           break;
            case Cursors.Dynamite:
                if(white)
                    Cursor.SetCursor(dynamiteCursor, Vector2.zero, CursorMode.Auto);
                else
                    Cursor.SetCursor(dynamiteCursorRed, Vector2.zero, CursorMode.Auto);
                break;

            default:
                break;
        }
    }
}
