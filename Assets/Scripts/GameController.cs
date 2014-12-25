using UnityEngine;
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
	}

	void Update () 
    {

	}

    /**
     * Anchors are points where nodes can be attached to. 
     * They are obtained calculating the intersections between pending bridges
     **/
    void ObtainAnchors()
    {
        m_anchors = new List<GridAnchor>();

        GameObject[] fadedBridges = GameObject.FindGameObjectsWithTag("FadedBridge");
        for (int fadedBridgeIndex1 = 0; fadedBridgeIndex1 != fadedBridges.Length; fadedBridgeIndex1++)
        {
            GameObject fadedBridgeObject1 = fadedBridges[fadedBridgeIndex1];
            BridgeBehaviour fadedBridge1 = fadedBridgeObject1.GetComponent<BridgeBehaviour>();

            for (int fadedBridgeIndex2 = fadedBridgeIndex1 + 1; fadedBridgeIndex2 != fadedBridges.Length; fadedBridgeIndex2++)
            {
                if (fadedBridgeIndex1 == 0 && fadedBridgeIndex2 == 3)
                {
                    int a = 1;
                }

                GameObject fadedBridgeObject2 = fadedBridges[fadedBridgeIndex2];

                BridgeBehaviour fadedBridge2 = fadedBridgeObject2.GetComponent<BridgeBehaviour>();
                bool bIntersect;
                Vector2 intersection;
                MathUtils.TwoSegmentsIntersection(fadedBridge1.m_startPoint, fadedBridge1.m_endPoint,
                                                  fadedBridge2.m_startPoint, fadedBridge2.m_endPoint,
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
                    anchor.PushBridge(fadedBridge1);
                    anchor.PushBridge(fadedBridge2);
                    fadedBridge1.PushAnchor(anchor);
                    fadedBridge2.PushAnchor(anchor);
                }
            }
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
            if (MathUtils.ArePointsEqual(anchor.Position, anchorAtIndex.Position))
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
            if (MathUtils.ArePointsEqual(position, anchorAtIndex.Position))
                return anchorIndex;
        }
        return -1;
    }
}
