﻿using UnityEngine;
using System.Collections;

public class NodeBehaviour : MonoBehaviour 
{
    public const float NODE_Z_VALUE = -200.0f;
    public const float TARGET_ANCHOR_NODE_Z_VALUE = -201.0f;
    public const float NODE_MOVE_SPEED = 300.0f;

    public enum NodeType
    {
        Simple = 1,
        TargetIndicator
    }

    public NodeType m_nodeType { get; set; }

    public GameObject m_simpleNodePrefab;
    //public GameObject m_targetIndicatorNodePrefab; //not used for the moment, currently using the simple node prefab with a different tint color on the shader

    private GridAnchor m_moveStartAnchor; //the anchor this node starts from when moving
    private GridAnchor m_moveEndAnchor; //the anchor this node ends to when moving
    private bool m_moving; //is this node moving or not?

	// Use this for initialization
	void Start () 
    {
	
	}

    public void MoveParentNodeToAnchor(GridAnchor anchor)
    {
        m_moveStartAnchor = this.GetComponent<NodeTouchHandler>().m_attachedAnchor;
        m_moveEndAnchor = anchor;
        m_moving = true;
    }

    public float GetZValue()
    {
        if (m_nodeType == NodeType.Simple)
            return NODE_Z_VALUE;
        else if (m_nodeType == NodeType.TargetIndicator)
            return TARGET_ANCHOR_NODE_Z_VALUE;

        return 0;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (m_moving)
        {
            float fNodeZValue = this.transform.position.z;
            Vector2 positionVec2 = transform.position;
            Vector2 direction = (m_moveEndAnchor.m_position - m_moveStartAnchor.m_position);
            float fTotalSqrDistance = direction.sqrMagnitude;
            direction.Normalize();
            positionVec2 += (direction * NODE_MOVE_SPEED * dt);

            float fCoveredSqrDistance = (positionVec2 - m_moveStartAnchor.m_position).sqrMagnitude;
            if (fCoveredSqrDistance > fTotalSqrDistance)
            {
                transform.position = new Vector3(m_moveEndAnchor.m_position.x, m_moveEndAnchor.m_position.y, GetZValue());
                m_moving = false;
                m_moveEndAnchor.Spread();
            }
            else
                transform.position = new Vector3(positionVec2.x, positionVec2.y, GetZValue());
        }
    }
}
