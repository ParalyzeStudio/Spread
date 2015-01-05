using UnityEngine;
using System.Collections;

public class GuiItemOnClick : MonoBehaviour 
{
    public GameObject m_simpleNodePrefab;

    /**
     * Drag a playable item onto the scene by destroying the GUI item itself and creating a game object
     * **/
    public void DragOntoScene()
    {
        //create a cloned game object
        Vector2 guiNodePosition = this.GetComponent<RectTransform>().anchoredPosition;
        Vector3 clonedNodePosition = CoordinatesUtils.SharedInstance.GetScreenCoordinatesInWorldPoint(guiNodePosition);
        clonedNodePosition.z = NodeProperties.NODE_Z_VALUE;
        GameObject clonedNode = (GameObject)Instantiate(m_simpleNodePrefab, clonedNodePosition, Quaternion.identity);
        NodeProperties clonedNodeBehaviour = clonedNode.GetComponent<NodeProperties>();
        clonedNodeBehaviour.m_nodeType = NodeProperties.NodeType.Simple;
        NodeMovement clonedNodeMovement = clonedNode.GetComponent<NodeMovement>();
        clonedNodeMovement.m_movementPoints = this.GetComponent<NodeMovement>().m_movementPoints;

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
        Debug.Log("restart");
        Application.LoadLevel(Application.loadedLevel);
    }

    /**
     * Action done when player has clicked the help button
     * **/
    public void ClickOnHelp()
    {


    }
}
