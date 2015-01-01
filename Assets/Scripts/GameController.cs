﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
    public Vector2 m_designScreenSize;

    private List<GridAnchor> m_anchors;
    public List<GridAnchor> Anchors
    {
        get
        {
            return m_anchors;
        }
    }

    void Awake()
    {
        //set the correct size for camera (m_designScreenSize height / 2.0f)
        Camera.main.orthographicSize = m_designScreenSize.y / 2.0f;
    }

	void Start () 
    {
        ObtainAnchors();
        CacheNeighbouringAnchors();
	}

	void Update () 
    {
        bool bVictory = CheckForVictory();
        if (bVictory)
        {
            Debug.Log("+++VICTORY+++");
        }

        bool bDefeat = CheckForDefeat();
        if (bDefeat)
        {
            Debug.Log("+++DEFEAT+++");
        }
	}

    /**
     * Anchors are points where nodes can be attached to. 
     * They are obtained calculating the intersections between pending bridges
     **/
    void ObtainAnchors()
    {
        m_anchors = new List<GridAnchor>();

        GameObject[] allBridges = GameObject.FindGameObjectsWithTag("Bridge");
        for (int fadedBridgeIndex1 = 0; fadedBridgeIndex1 != allBridges.Length; fadedBridgeIndex1++)
        {
            GameObject bridgeObject1 = allBridges[fadedBridgeIndex1];
            BridgeBehaviour bridge1 = bridgeObject1.GetComponent<BridgeBehaviour>();

            for (int bridgeIndex2 = fadedBridgeIndex1 + 1; bridgeIndex2 != allBridges.Length; bridgeIndex2++)
            {
                GameObject bridgeObject2 = allBridges[bridgeIndex2];

                BridgeBehaviour bridge2 = bridgeObject2.GetComponent<BridgeBehaviour>();
                bool bIntersect;
                Vector2 intersection;
                MathUtils.TwoSegmentsIntersection(bridge1.m_startPoint, bridge1.m_endPoint,
                                                  bridge2.m_startPoint, bridge2.m_endPoint,
                                                  out bIntersect, out intersection);

                if (bIntersect)
                {
                    int anchorExistsIndex = AnchorExists(intersection);
                    GridAnchor anchor;
                    if (anchorExistsIndex >= 0)
                        anchor = m_anchors[anchorExistsIndex];
                    else
                    {
                        anchor = new GridAnchor(intersection);
                        m_anchors.Add(anchor);
                    }
                    anchor.PushBridge(bridge1);
                    anchor.PushBridge(bridge2);
                    bridge1.PushAnchor(anchor);
                    bridge2.PushAnchor(anchor);
                }
            }
        }
    }
    
    /**
     * Cache the neighbouring anchors for each anchor that has been obtained in the ObtainAnchors() function
     * **/
    private void CacheNeighbouringAnchors()
    {
        foreach (GridAnchor anchor in m_anchors)
        {
            anchor.InitNeighbouringAnchors();
        }
    }

    /**
     * Check if an anchor is already set in the global collection of anchors.
     * Returns the index of the anchor in this collection.
     * **/
    int AnchorExists(GridAnchor anchor)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            GridAnchor anchorAtIndex = m_anchors[anchorIndex];
            if (MathUtils.ArePointsEqual(anchor.m_position, anchorAtIndex.m_position))
                return anchorIndex;
        }
        return -1;
    }

    /**
     * Same as int AnchorExists(GridAnchor anchor) but this time search with the position as parameter
     * Returns the index of the anchor in this collection.
     * **/
    int AnchorExists(Vector2 position)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            GridAnchor anchorAtIndex = m_anchors[anchorIndex];
            if (MathUtils.ArePointsEqual(position, anchorAtIndex.m_position))
                return anchorIndex;
        }
        return -1;
    }

    /**
     * Returns true if all bridges are marked as completed
     * **/
    public bool CheckForVictory()
    {
        GameObject[] allBridges = GameObject.FindGameObjectsWithTag("Bridge");
        foreach (GameObject bridge in allBridges)
        {
            BridgeBehaviour bridgeBehaviour = bridge.GetComponent<BridgeBehaviour>();
            if (bridgeBehaviour.m_status != BridgeBehaviour.BridgeStatus.Completed)
                return false;
        }
        return true;
    }

    /**
     * Returns true if player cannot make any more moves and at least one bridge is not completed
     * **/
    public bool CheckForDefeat()
    {
        return false;
    }
}
