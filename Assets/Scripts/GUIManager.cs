using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour
{

    public static GUIManager instance = null;

    public const uint NORMAL_CURSOR = 1;
    public const uint ADD_CURSOR = 2;
    public const uint SELECTABLE_CURSOR = 3;
    public const uint DRAGBLE_CURSOR = 4;
    public const uint DRAGING_CURSOR = 5;

    public Texture2D normalTexture;
    public Vector2 normalHotSpot = Vector2.zero;
    public Texture2D addTexture;
    public Vector2 addlHotSpot = Vector2.zero;
    public Texture2D selectableTexture;
    public Vector2 selectableHotSpot = Vector2.zero;
    public Texture2D dragbleTexture;
    public Vector2 dragbleHotSpot = Vector2.zero;
    public Texture2D dragingTexture;
    public Vector2 dragingHotSpot = Vector2.zero;

    public CursorMode cursorMode = CursorMode.Auto;

    // Use this for initialization
    void Awake ()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
	}

    public void changeCursor(uint cursor)
    {
        switch(cursor)
        {
            case NORMAL_CURSOR:
                Cursor.SetCursor(normalTexture, normalHotSpot, cursorMode);
                break;
            case ADD_CURSOR:
                Cursor.SetCursor(addTexture, addlHotSpot, cursorMode);
                break;
            case SELECTABLE_CURSOR:
                Cursor.SetCursor(selectableTexture, selectableHotSpot, cursorMode);
                break;
            case DRAGBLE_CURSOR:
                Cursor.SetCursor(dragbleTexture, dragbleHotSpot, cursorMode);
                break;
            case DRAGING_CURSOR:
                Cursor.SetCursor(dragingTexture, dragingHotSpot, cursorMode);
                break;

        }
    }
}
