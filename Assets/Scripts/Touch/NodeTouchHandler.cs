using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeTouchHandler : TouchHandler
{
    public float m_snapDistance;
    private GameController m_gameController;
    private GridAnchor m_snappedAnchor; //the current anchor the node is snapped to
    private GridAnchor m_attachedAnchor; //the anchor the node is attached too and where it can move to its neighbouring anchors
    private GridAnchor m_targetAnchor; //the anchor we target when moving the node from the attached anchor (determined in the first OnPointerMove)

    public override void Start()
    {
        base.Start();

        //retrieve anchors from the game controller
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        m_gameController = gameControllerObject.GetComponent<GameController>();

        m_snappedAnchor = null;
        m_attachedAnchor = null;
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        if (m_attachedAnchor == null) //node can move freely (i.e it has been dragged from GUI item list)
        {
            bool bSnap = SnapToClosestAnchor();
            if (m_snappedAnchor != null)
            {
                if (!bSnap) //we don't try to unsnap if we just snap to an anchor
                    TryToUnsnap();
            }
            else
            {
                Vector3 deltaVec3 = delta;
                this.transform.position += deltaVec3;
            }
        }
        else
        {

        }

        //if (m_snappedAnchor == null)
        //{
        //    bool bSnap = SnapToClosestAnchor();
        //    if (bSnap)
        //        Debug.Log("SNAP");
        //    if (bSnap && m_snappedAnchor == m_attachedAnchor) //we snapped to the attached anchor, reset the targetAnchor for fresh start
        //    {
                
        //        m_targetAnchor = null;
        //    }
        //}
        //else
        //{
        //    //if we are far enough from the snapped anchor try to detach the node from it
        //    TryToUnsnap();

        //    //we head for a certain target anchor if not set and the node is snapped to an anchor (i.e not moving on bridge)
        //    if (m_snappedAnchor != null && m_targetAnchor == null)
        //    {
        //        List<GridAnchor> neighbouringLinkedAnchors = m_snappedAnchor.FindNeighbouringLinkedAnchors();
        //        //find the direction where to drag the node based on the value of the dot product of normalized delta and direction of a neighbouring anchor
        //        //the max value of this product (i.e smallest angle between vectors) determines the anchor we have to head for
        //        Vector2 normalizedDelta = delta;
        //        normalizedDelta.Normalize();

        //        float maxDotProduct = int.MinValue;
        //        foreach (GridAnchor neighbouringLinkedAnchor in neighbouringLinkedAnchors)
        //        {
        //            Vector2 anchorDirection = (neighbouringLinkedAnchor.Position - m_snappedAnchor.Position);
        //            anchorDirection.Normalize();
        //            float dotProduct = Vector2.Dot(normalizedDelta, anchorDirection);
        //            if (dotProduct > maxDotProduct)
        //            {
        //                maxDotProduct = dotProduct;
        //                m_targetAnchor = neighbouringLinkedAnchor;
        //            }
        //        }
        //    }
        //}

        ////finally move the node
        ////if we head for a target anchor, calculate the projection of the delta vector onto the segment [m_attachedAnchor;m_targetAnchor]
        //if (m_attachedAnchor != null)
        //{
        //    if (m_targetAnchor != null && m_snappedAnchor == null)
        //    {
        //        Vector2 targetAnchorDirection = (m_targetAnchor.Position - m_attachedAnchor.Position);
        //        targetAnchorDirection.Normalize();
        //        float fDeltaProjection = Vector2.Dot(delta, targetAnchorDirection);
        //        delta = targetAnchorDirection * fDeltaProjection;
        //    }
        //    else
        //        delta = Vector2.zero;
        //}
        //else if (m_snappedAnchor != null)
        //{
        //    delta = Vector2.zero;
        //}
        //Vector3 deltaVec3 = delta;

        //this.transform.position += deltaVec3;

        return true;
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();
        if (m_snappedAnchor != null)
        {
            m_attachedAnchor = m_snappedAnchor;
            m_snappedAnchor.Spread();
        }
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

            Vector2 position = this.transform.position;
            float fDistanceToAnchor = (position - snapAnchor.Position).magnitude;

            Debug.Log("snapAnchor X:" + snapAnchor.Position.x + "Y:" + snapAnchor.Position.y);
            if (m_snappedAnchor != null)
                Debug.Log("m_snappedAnchor X:" + m_snappedAnchor.Position.x + "Y:" + m_snappedAnchor.Position.y);
            if (snapAnchor != m_snappedAnchor)
            {
                if (fDistanceToAnchor <= m_snapDistance)
                {
                    m_snappedAnchor = snapAnchor;
                    this.transform.position = snapAnchor.Position;

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
        Vector2 mouseWorldPosition = CoordinatesUtils.SharedInstance.GetMousePositionInWorldCoordinates();
        float fDistanceFromMouseToAnchor = (mouseWorldPosition - m_snappedAnchor.Position).magnitude;
        if (fDistanceFromMouseToAnchor > 2 * m_snapDistance)
        {
            Debug.Log("UNSNAP");
            if (m_attachedAnchor != null)
            {
                if (m_targetAnchor != null)
                {
                    Debug.Log("UNSNAP 11111");
                    Vector2 targetAnchorDirection = (m_targetAnchor.Position - m_snappedAnchor.Position);
                    targetAnchorDirection.Normalize();
                    Vector2 delta = mouseWorldPosition - m_snappedAnchor.Position;
                    float fDeltaProjection = Vector2.Dot(delta, targetAnchorDirection);
                    this.transform.position = m_snappedAnchor.Position + targetAnchorDirection * fDeltaProjection;
                }
                else
                    Debug.Log("UNSNAP 99999");
            }
            else if (m_attachedAnchor == null)
            {
                Debug.Log("UNSNAP 22222");
                this.transform.position = mouseWorldPosition;
            }
            else if (m_targetAnchor == null)
            {
                Debug.Log("UNSNAP 33333");
                this.transform.position = mouseWorldPosition;
            }
            m_snappedAnchor = null;
        }
    }

    private void FindTargetAnchor(Vector2 pointerMoveDelta)
    {
        List<GridAnchor> neighbouringAnchors = m_snappedAnchor.NeighbouringAnchors;
        //find the direction where to drag the node based on the value of the dot product of normalized delta and direction of a neighbouring anchor
        //the max value of this product (i.e smallest angle between vectors) determines the anchor we have to head for
        Vector2 normalizedDelta = pointerMoveDelta;
        normalizedDelta.Normalize();

        float maxDotProduct = int.MinValue;
        foreach (GridAnchor neighbouringLinkedAnchor in neighbouringAnchors)
        {
            Vector2 anchorDirection = (neighbouringLinkedAnchor.Position - m_snappedAnchor.Position);
            anchorDirection.Normalize();
            float dotProduct = Vector2.Dot(normalizedDelta, anchorDirection);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                m_targetAnchor = neighbouringLinkedAnchor;
            }
        }
    }
}
