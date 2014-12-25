using UnityEngine;
using System.Collections;


/**
 * Base Class that handles touches or clicks on the scene
 * **/
public class TouchHandler : MonoBehaviour
{
    public const float MOVE_EPSILON = 0.5f;

    public Vector2 m_touchArea;
    private bool m_selected;
    private Vector2 m_prevPointerLocation;

    public virtual void Start()
    {

    }

    /**
     * Player touched this object
     * **/
    protected virtual void OnPointerDown(Vector2 pointerLocation)
    {
        m_selected = true;
    }

    /**
     * Player moved the pointer with object selected
     * **/
    protected virtual bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!m_selected)
            return false;

        delta = pointerLocation - m_prevPointerLocation;
        if (delta.sqrMagnitude < MOVE_EPSILON)
            return false;

        return true;
    }

    /**
     * Player released the pointer
     * **/
    protected virtual void OnPointerUp()
    {
        m_selected = false;
    }

    /**
     * Returns the coordinates of the mouse in screen coordinates.
     * (0,0) are the coordinates of the center of the screen.
     * **/
    protected Vector2 GetMousePositionInScreenCoordinates()
    {
        Vector2 mouseRectRelativeCoordinates = GetMousePositionInWorldCoordinates();
        Vector2 cameraPosition = Camera.main.transform.position;
        Vector2 mouseWorldPosition = mouseRectRelativeCoordinates - cameraPosition; //the HUD canvas is centered in the screen, so offset the mouseWorld position by the camera position

        return mouseWorldPosition;
    }

    /**
     * Returns the coordinates of the mouse in world coordinates.
     * **/
    protected Vector2 GetMousePositionInWorldCoordinates()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseRectRelativeCoordinates = Camera.main.ScreenToWorldPoint(mousePosition);

        return mouseRectRelativeCoordinates;
    }

    /**
     * Returns the world position of a point in screen coordinates.
     * **/
    protected Vector2 GetScreenCoordinatesInWorldPoint(Vector2 screenCoords)
    {
        Vector2 cameraPosition = Camera.main.transform.position;
        Vector2 worldPoint = screenCoords + cameraPosition;

        return worldPoint;
    }

    /**
     * Handles the touches/click on nodes to drag them properly or on scene to slide it
     * -1 touch: drag a node or slide the scene
     * -2 touches: zoom in/out the scene
     * **/
    void Update()
    {
        Vector2 touchLocation;
#if UNITY_IPHONE || UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            touchLocation = Camera.main.ScreenToWorldPoint(touch.position);
            if (touch.phase.Equals(TouchPhase.Began))
            {
                Rect touchAreaRect = new Rect();
                Vector2 position = transform.position;
                touchAreaRect.position = position - 0.5f * m_touchArea;
                touchAreaRect.width = m_touchArea.x;
                touchAreaRect.height = m_touchArea.y;
                if (touchAreaRect.Contains(touchLocation))
                {
                    OnPointerDown(touchLocation);
                }
            }
            else if (touch.phase.Equals(TouchPhase.Moved))
            {
                OnPointerMove(touchLocation);
            }
            else if (touch.phase.Equals(TouchPhase.Ended))
            {
                if (m_selected)
                    OnPointerUp();
            }
        }
        //TODO handle the case of 2 touches
        else if (Input.touchCount == 2)
        {

        }
#else
        if (Input.GetMouseButton(0))
        {
            touchLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!m_selected)
            {
                Rect touchAreaRect = new Rect();
                Vector2 position = transform.position;
                touchAreaRect.position = position - 0.5f * m_touchArea;
                touchAreaRect.width = m_touchArea.x;
                touchAreaRect.height = m_touchArea.y;
                if (touchAreaRect.Contains(touchLocation))
                {
                    OnPointerDown(touchLocation);
                }
            }
            else
            {
                Vector2 delta = Vector2.zero;
                OnPointerMove(touchLocation, ref delta);
            }

            m_prevPointerLocation = touchLocation;
        }
        else
        {
            if (m_selected)
                OnPointerUp();
        }
#endif
    }
}
