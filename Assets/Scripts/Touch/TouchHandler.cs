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
