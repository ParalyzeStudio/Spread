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
        clonedNodePosition.z = NodeBehaviour.NODE_Z_VALUE;
        GameObject clonedNode = (GameObject)Instantiate(m_nodePrefab, clonedNodePosition, Quaternion.identity);
        NodeBehaviour clonedNodeBehaviour = clonedNode.GetComponent<NodeBehaviour>();
        clonedNodeBehaviour.m_nodeType = NodeBehaviour.NodeType.Simple;
        clonedNodeBehaviour.m_movementPoints = this.GetComponent<NodeBehaviour>().m_movementPoints;

        //deactivate the gui element
        this.gameObject.SetActive(false);
        clonedNodeBehaviour.m_deactivatedGUIItem = this.gameObject;
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
