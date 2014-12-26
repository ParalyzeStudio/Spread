using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeTouchHandler : TouchHandler
{
    public float m_snapDistance;
    private GameController m_gameController;
    private bool m_snapped; //is the node snapped to an anchor
    private bool m_attached; //has the node been attached to an anchor previously or still being dragged from GUI item list
    private GridAnchor m_snappedAnchor;
    private BridgeBehaviour m_snapBridge;

    public override void Start()
    {
        base.Start();

        //retrieve anchors from the game controller
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        m_gameController = gameControllerObject.GetComponent<GameController>();

        m_snapped = false;
        m_attached = false;
        m_snappedAnchor = null;
    }

    protected override void OnPointerDown(Vector2 pointerLocation)
    {
        base.OnPointerDown(pointerLocation);

        //draw a circle for each of the neighbouring anchors
        List<GridAnchor> neighbouringLinkedAnchors = m_snappedAnchor.FindNeighbouringLinkedAnchors();
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        if (m_snapBridge)
        {
            if (m_snapped)
                TryToUnsnap();
        }

        if (!m_snapped)
        {
            Vector3 deltaVec3 = delta;
            this.transform.position += deltaVec3;

            SnapToClosestAnchor();
        }
        else
        {
            List<GridAnchor> neighbouringLinkedAnchors = m_snappedAnchor.FindNeighbouringLinkedAnchors();
            //find the direction where to drag the node based on the value of the dot product of normalized delta and direction of a neighbouring anchor
            //the max value of this product (i.e smallest angle between vectors) determines the anchor we have to head for
            Vector2 normalizedDelta = delta;
            normalizedDelta.Normalize();

            float maxDotProduct = int.MinValue;
            GridAnchor targetAnchor = null;
            foreach (GridAnchor neighbouringLinkedAnchor in neighbouringLinkedAnchors)
            {
                Vector2 anchorDirection = (neighbouringLinkedAnchor.Position - m_snappedAnchor.Position);
                anchorDirection.Normalize();
                float dotProduct = Vector2.Dot(normalizedDelta, anchorDirection);
                if (dotProduct > maxDotProduct)
                {
                    maxDotProduct = dotProduct;
                    targetAnchor = neighbouringLinkedAnchor;
                }
            }
            
            return false;
        }

        return true;
    }

    /**
     * Try to snap a node to an anchor if the distance to it is small enough ( <= m_snapDistance)
     * **/
    bool SnapToClosestAnchor()
    {
        List<GridAnchor> allAnchors = m_gameController.Anchors;

        for (int anchorIndex = 0; anchorIndex != allAnchors.Count; anchorIndex++)
        {
            GridAnchor snapAnchor = allAnchors[anchorIndex];

            //float fDistanceToAnchor = (m_rectTransform.anchoredPosition - snapAnchor.Position).magnitude;
            Vector2 position = this.transform.position;
            float fDistanceToAnchor = (position - snapAnchor.Position).magnitude;

            if (snapAnchor != m_snappedAnchor)
            {
                if (fDistanceToAnchor <= m_snapDistance)
                {
                    m_snapped = true;
                    m_attached = true; //if this is the first snap, we attach the node to the scene
                    m_snappedAnchor = snapAnchor;
                    this.transform.position = snapAnchor.Position;

                    List<BridgeBehaviour> anchorBridges = snapAnchor.Bridges;
                    foreach (BridgeBehaviour anchorBridge in anchorBridges)
                    {
                        anchorBridge.SpreadAroundAnchorPoint(m_snappedAnchor);
                        m_snappedAnchor.Linked = true; //this anchor became active
                    }
                    //anchorBridges[1].SpreadAroundAnchorPoint(m_snappedAnchor);
                    return true;
                }
            }
        }

        return false;
    }

    /**
     * Try to unsnap a node to an anchor if the distance to it is big enough ( > m_snapDistance)
     * **/
    void TryToUnsnap()
    {
        Vector2 positionVec2 = this.transform.position;
        float fDistanceFromMouseToAnchor = (positionVec2 - m_snappedAnchor.Position).magnitude;
        if (fDistanceFromMouseToAnchor > m_snapDistance)
        {
            m_snapped = false;
            m_snappedAnchor = null;
        }
    }
}
