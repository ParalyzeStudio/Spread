﻿using UnityEngine;
using System.Collections;

public class NodeProperties : MonoBehaviour 
{
    public const float NODE_Z_VALUE = -200.0f;
    public const float TARGET_ANCHOR_NODE_Z_VALUE = -201.0f;

    public enum NodeType
    {
        Simple = 1,
        TargetIndicator
    }

    public NodeType m_nodeType { get; set; }

    //GUI item that has been deactivated for creating this node
    public GameObject m_deactivatedGUIItem { get; set; }

	// Use this for initialization
	void Start () 
    {
	
	}

    public float GetZValue()
    {
        if (m_nodeType == NodeType.Simple)
            return NODE_Z_VALUE;
        else if (m_nodeType == NodeType.TargetIndicator)
            return TARGET_ANCHOR_NODE_Z_VALUE;

        return 0;
    }

    
}
