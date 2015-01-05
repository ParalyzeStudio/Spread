using UnityEngine;
using System.Collections;

public class NodeProperties : MonoBehaviour 
{
    public const float NODE_Z_VALUE = -200.0f;
    public const float TARGET_ANCHOR_NODE_Z_VALUE = -199.0f;

    public enum NodeType
    {
        Simple = 1,
        TargetIndicator
    }

    public NodeType m_nodeType { get; set; }

    //GUI item that has been deactivated for creating this node
    public GameObject m_deactivatedGUIItem { get; set; }

    /**
     * Returns the correct value that should be assigned to a node depending on its type
     * **/
    public float GetZValue()
    {
        if (m_nodeType == NodeType.Simple)
            return NODE_Z_VALUE;
        else if (m_nodeType == NodeType.TargetIndicator)
            return TARGET_ANCHOR_NODE_Z_VALUE;

        return 0;
    }    
}
