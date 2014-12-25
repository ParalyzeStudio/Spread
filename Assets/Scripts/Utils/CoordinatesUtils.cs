using UnityEngine;

public class CoordinatesUtils
{
    private static CoordinatesUtils m_sharedInstance;
    public static CoordinatesUtils SharedInstance
    {
        get
        {
            if (m_sharedInstance == null)
                m_sharedInstance = new CoordinatesUtils();

            return m_sharedInstance;
        }
    }

    /**
     * Returns the coordinates of the mouse in screen coordinates.
     * (0,0) are the coordinates of the center of the screen.
     * **/
    public Vector2 GetMousePositionInScreenCoordinates()
    {
        Vector2 mouseRectRelativeCoordinates = GetMousePositionInWorldCoordinates();
        Vector2 cameraPosition = Camera.main.transform.position;
        Vector2 mouseWorldPosition = mouseRectRelativeCoordinates - cameraPosition; //the HUD canvas is centered in the screen, so offset the mouseWorld position by the camera position

        return mouseWorldPosition;
    }

    /**
     * Returns the coordinates of the mouse in world coordinates.
     * **/
    public Vector2 GetMousePositionInWorldCoordinates()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseRectRelativeCoordinates = Camera.main.ScreenToWorldPoint(mousePosition);

        return mouseRectRelativeCoordinates;
    }

    /**
     * Returns the world position of a point in screen coordinates.
     * **/
    public Vector2 GetScreenCoordinatesInWorldPoint(Vector2 screenCoords)
    {
        Vector2 cameraPosition = Camera.main.transform.position;
        Vector2 worldPoint = screenCoords + cameraPosition;

        return worldPoint;
    }
}
