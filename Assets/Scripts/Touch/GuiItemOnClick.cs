using UnityEngine;
using System.Collections;

public class GuiItemOnClick : MonoBehaviour 
{
    public GameObject m_nodePrefab;

    /**
     * Drag a playable item onto the scene by destroying the GUI item itself and creating a game object
     * **/
    public void DragOntoScene()
    {
        //create a cloned game object
        Vector2 guiNodePosition = this.GetComponent<RectTransform>().anchoredPosition;
        Vector3 clonedNodePosition = CoordinatesUtils.SharedInstance.GetScreenCoordinatesInWorldPoint(guiNodePosition);
        GameObject clonedNode = (GameObject)Instantiate(m_nodePrefab, clonedNodePosition, Quaternion.identity);

        //destroy the gui element
        Destroy(this.gameObject);
    }

    /**
     * Action done when player has clicked the quit button
     * **/
    public void ClickOnQuit()
    {

    }

    /**
     * Action done when player has clicked the restart button
     * **/
    public void ClickOnRestart()
    {

    }

    /**
     * Action done when player has clicked the help button
     * **/
    public void ClickOnHelp()
    {


    }
}
