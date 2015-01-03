using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Bridge : UVQuad 
{
    public const float SOLID_BRIDGE_Z_VALUE = -100.0f;
    public const float FADED_BRIDGE_Z_VALUE = -100.0f;
    public const float SPREADING_BRIDGE_Z_VALUE = -150.0f;
    public const float TARGET_INDICATOR_BRIDGE_Z_VALUE = -151.0f;
    public const float BRIDGE_THICKNESS = 10.0f;

    public enum BridgeType
    {
        Completed = 1,
        Faded = 2,
        Spreading = 3,
        TargetIndicator = 4
    }

    public Vector2 m_startPoint;
    public Vector2 m_endPoint;
    public BridgeType m_type;
    public List<GridAnchor> m_anchors { get; set; } //all anchors this bridge pass through

    protected override void Awake()
    {
        base.Awake();

        float fRotationAngleRad = this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float fBridgeLength = this.transform.localScale.x;
        Vector3 director = new Vector2(Mathf.Cos(fRotationAngleRad), Mathf.Sin(fRotationAngleRad));
        m_startPoint = this.transform.position - 0.5f * fBridgeLength * director;
        m_endPoint = this.transform.position + 0.5f * fBridgeLength * director;
        m_anchors = new List<GridAnchor>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public float GetZValue()
    {
        if (m_type == BridgeType.Completed)
            return SOLID_BRIDGE_Z_VALUE;
        else if (m_type == BridgeType.Faded)
            return FADED_BRIDGE_Z_VALUE;
        else if (m_type == BridgeType.Spreading)
            return SPREADING_BRIDGE_Z_VALUE;
        else if (m_type == BridgeType.TargetIndicator)
            return TARGET_INDICATOR_BRIDGE_Z_VALUE;
        else
            return SOLID_BRIDGE_Z_VALUE;
    }

    protected override void Update()
    {
        float dt = Time.deltaTime;

        //set the correct position
        Vector2 bridgeCenter = (m_startPoint + m_endPoint) / 2.0f;

        this.transform.position = new Vector3(bridgeCenter.x, bridgeCenter.y, GetZValue());

        //set the correct rotation
        float fRotationAngleRad = Mathf.Atan2((m_endPoint.y - m_startPoint.y), (m_endPoint.x - m_startPoint.x));
        this.transform.rotation = Quaternion.Euler(0, 0, fRotationAngleRad * Mathf.Rad2Deg);

        //and the length
        float fBridgeLength = (m_endPoint - m_startPoint).magnitude;
        this.transform.localScale = new Vector3(fBridgeLength, BRIDGE_THICKNESS, this.transform.localScale.z);
        
        base.Update();
    }

    

    public void NotifyAnchorsOfBridgeAddition(Bridge bridge)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            m_anchors[anchorIndex].PushBridge(bridge);
        }
    }

    public void NotifyAnchorsOfBridgeRemoval(Bridge bridge)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            m_anchors[anchorIndex].RemoveBridge(bridge);
        }
    }

    public void PushAnchor(GridAnchor anchor)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            if (m_anchors[anchorIndex].Equals(anchor))
                return;
        }

        m_anchors.Add(anchor);
    }

    public void RemoveAnchor(GridAnchor anchor)
    {
        //check if bridge is not already in this List 
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            if (m_anchors[anchorIndex].Equals(anchor))
            {
                m_anchors.Remove(anchor);
                return;
            }
        }
    }

    /**
     * Retrieve closest anchors to parameter anchor on this bridge (at most 2 anchors)
     * **/
    public List<GridAnchor> GetNeighbouringAnchors(GridAnchor anchor)
    {        
        List<GridAnchor> neighbouringAnchors = new List<GridAnchor>();
        neighbouringAnchors.Capacity = 2; //at most 2 neighbouring anchors

        //find the closest linked anchor on segment [anchor.Position; m_startPoint]
        GridAnchor neighbouringLinkedAnchor = FindClosestAnchorOnSegment(anchor, m_startPoint);
        if (neighbouringLinkedAnchor != null)
            neighbouringAnchors.Add(neighbouringLinkedAnchor);

        //do the same for the segment [anchor.Position; m_endPoint]
        neighbouringLinkedAnchor = FindClosestAnchorOnSegment(anchor, m_endPoint);
        if (neighbouringLinkedAnchor != null)
            neighbouringAnchors.Add(neighbouringLinkedAnchor);

        return neighbouringAnchors;
    }

    ///**
    // * Retrieve closest anchors to parameter anchor and check if they are linked before returning them
    // * **/
    //public List<GridAnchor> GetNeighbouringLinkedAnchors(GridAnchor anchor)
    //{
    //    List<GridAnchor> neighbouringLinkedAnchors = new List<GridAnchor>();
    //    neighbouringLinkedAnchors.Capacity = 2; //at most 2 neighbouring anchors

    //    //find the closest linked anchor on segment [anchor.Position; m_startPoint]
    //    GridAnchor neighbouringLinkedAnchor = FindClosestAnchorOnSegment(anchor, m_startPoint);
    //    if (neighbouringLinkedAnchor != null && neighbouringLinkedAnchor.isLinked) //check if the closes anchor we found is linked
    //        neighbouringLinkedAnchors.Add(neighbouringLinkedAnchor);

    //    //do the same for the segment [anchor.Position; m_endPoint]
    //    neighbouringLinkedAnchor = FindClosestAnchorOnSegment(anchor, m_endPoint);
    //    if (neighbouringLinkedAnchor != null && neighbouringLinkedAnchor.isLinked) //check if the closes anchor we found is linked
    //        neighbouringLinkedAnchors.Add(neighbouringLinkedAnchor);

    //    return neighbouringLinkedAnchors;
    //}

    /**
     * Returns the closest anchor on segment [anchor.Position; segmentEndPoint] or null if no one was found
     * **/
    private GridAnchor FindClosestAnchorOnSegment(GridAnchor anchor, Vector2 segmentEndPoint)
    {
        GridAnchor minDistanceAnchor = null;
        float minDistance = int.MaxValue;
        foreach (GridAnchor segmentAnchor in m_anchors)
        {
            if (segmentAnchor.Equals(anchor))
                continue;

            if (MathUtils.isLinePointContainedInSegment(segmentAnchor.m_position, anchor.m_position, segmentEndPoint))
            {
                float sqrDistanceToAnchor = (segmentAnchor.m_position - anchor.m_position).sqrMagnitude;
                if (sqrDistanceToAnchor < minDistance)
                {
                    minDistanceAnchor = segmentAnchor;
                    minDistance = sqrDistanceToAnchor;
                }
            }
        }
        return minDistanceAnchor;
    }
}

