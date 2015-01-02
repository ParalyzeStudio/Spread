using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BridgeBehaviour : UVQuad 
{
    public const float SOLID_BRIDGE_Z_VALUE = -100.0f;
    public const float FADED_BRIDGE_Z_VALUE = -100.0f;
    public const float SPREADING_BRIDGE_Z_VALUE = -150.0f;
    public const float BRIDGE_THICKNESS = 10.0f;
    public const float BRIDGE_SPREADING_SPEED = 300.0f;

    public enum BridgeType
    {
        Completed = 1,
        Faded = 2,
        Spreading = 3,
        TargetIndicator = 4
    }

    public Vector2 m_startPoint;
    public Vector2 m_endPoint;
    public GameObject m_solidBridgePrefab;
    public BridgeType m_type;
    private BridgeType m_prevType;
    public BridgeBehaviour m_spreadBridge { get; set; }
    public BridgeBehaviour m_coveredBridge { get; set; }
    private List<GridAnchor> m_anchors; //all anchors this bridge pass through
    private GridAnchor m_spreadAnchor;

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

    protected override void Update()
    {
        if (m_prevType != m_type)
        {
            m_prevType = m_type;
            MaterialHolder materialHolder = GetComponent<MaterialHolder>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (m_type == BridgeType.Completed || m_type == BridgeType.Spreading)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(0); //white texture
            }
            else if (m_type == BridgeType.Faded)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(1); //white texture with 0.12f opacity
            }
            else if (m_type == BridgeType.TargetIndicator)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(2); //red texture
            }
        }

        float dt = Time.deltaTime;

        //handle the special case of a bridge that is currently spreading
        if (m_type == BridgeType.Spreading)
        {
            //Check if the bridge spreading is done or increase its size otherwise
            float dx = BRIDGE_SPREADING_SPEED * dt;

            Vector2 director = (m_coveredBridge.m_endPoint - m_coveredBridge.m_startPoint);
            director.Normalize();

            float fDistanceFromAnchorToSpreadBridgeStartPoint = (m_startPoint - m_spreadAnchor.m_position).magnitude;
            float fDistanceFromAnchorToSpreadBridgeEndPoint = (m_endPoint - m_spreadAnchor.m_position).magnitude;
            float fDistanceFromAnchorToCoveredBridgeStartPoint = (m_coveredBridge.m_startPoint - m_spreadAnchor.m_position).magnitude;
            float fDistanceFromAnchorToCoveredBridgeEndPoint = (m_coveredBridge.m_endPoint - m_spreadAnchor.m_position).magnitude;

            bool bStartPointReached = false;
            bool bEndPointReached = false;
            if (fDistanceFromAnchorToSpreadBridgeStartPoint == fDistanceFromAnchorToCoveredBridgeStartPoint) //bridge already hit the border
                bStartPointReached = true;
            else
            {
                if (fDistanceFromAnchorToSpreadBridgeStartPoint > fDistanceFromAnchorToCoveredBridgeStartPoint) //we exceeded the length of the bridge
                {
                    bStartPointReached = true;
                    m_startPoint = m_coveredBridge.m_startPoint;
                }
                else
                    m_startPoint -= (director * dx);
            }


            if (fDistanceFromAnchorToSpreadBridgeEndPoint == fDistanceFromAnchorToCoveredBridgeEndPoint) //bridge already hit the border
                bEndPointReached = true;
            else
            {
                if (fDistanceFromAnchorToSpreadBridgeEndPoint > fDistanceFromAnchorToCoveredBridgeEndPoint)
                {
                    bEndPointReached = true;
                    m_endPoint = m_coveredBridge.m_endPoint;
                }
                else
                    m_endPoint += (director * dx);
            }

            //Link every anchor that has been reached by this spreading bridge
            foreach (GridAnchor anchor in m_coveredBridge.m_anchors)
            {
                Vector2 anchorPosition = anchor.m_position;

                if (!anchor.m_linked && MathUtils.isLinePointContainedInSegment(anchorPosition, m_startPoint, m_endPoint))
                {
                    PushAnchor(anchor);
                    anchor.m_linked = true;
                }
            }

            //we finish building the spreading bridge
            if (bStartPointReached && bEndPointReached) 
            {
                m_type = BridgeType.Completed;
                NotifyAnchorsOfBridgeAddition(this);
                NotifyAnchorsOfBridgeRemoval(m_coveredBridge);
                Destroy(m_coveredBridge.gameObject);
                this.transform.position = new Vector3(transform.position.x, transform.position.y, SOLID_BRIDGE_Z_VALUE);
            }
        }

        //set the correct position
        Vector2 bridgeCenter = (m_startPoint + m_endPoint) / 2.0f;
        float zPosition;
        if (m_type == BridgeType.Completed)
            zPosition = SOLID_BRIDGE_Z_VALUE;
        else if (m_type == BridgeType.Faded)
            zPosition = FADED_BRIDGE_Z_VALUE;
        else if (m_type == BridgeType.Spreading)
            zPosition = SPREADING_BRIDGE_Z_VALUE;
        else
            zPosition = SOLID_BRIDGE_Z_VALUE;
        this.transform.position = new Vector3(bridgeCenter.x, bridgeCenter.y, zPosition);

        //set the correct rotation
        float fRotationAngleRad = Mathf.Atan2((m_endPoint.y - m_startPoint.y), (m_endPoint.x - m_startPoint.x));
        this.transform.rotation = Quaternion.Euler(0, 0, fRotationAngleRad * Mathf.Rad2Deg);

        //and the length
        float fBridgeLength = (m_endPoint - m_startPoint).magnitude;
        this.transform.localScale = new Vector3(fBridgeLength, BRIDGE_THICKNESS, this.transform.localScale.z);
        
        base.Update();
    }

    /**
     * Create solid bridges that will scale and cover all faded bridges around this anchor
     * **/
    public void SpreadAroundAnchorPoint(GridAnchor anchor)
    {
        Vector3 spreadBridgePosition = anchor.m_position;
        spreadBridgePosition.z = -150.0f;

        GameObject clonedObject = (GameObject)Instantiate(m_solidBridgePrefab, spreadBridgePosition, this.transform.rotation);
        Transform transform = clonedObject.GetComponent<Transform>();
        transform.localScale = new Vector3(0.0f, transform.localScale.y, transform.localScale.z);
        m_spreadBridge = clonedObject.GetComponent<BridgeBehaviour>();
        m_spreadBridge.m_type = BridgeType.Spreading;
        m_spreadBridge.m_spreadAnchor = anchor;
        m_spreadBridge.m_coveredBridge = this;
        m_spreadBridge.m_startPoint = anchor.m_position;
        m_spreadBridge.m_endPoint = anchor.m_position;
        m_spreadBridge.PushAnchor(anchor);
        anchor.m_linked = true;
    }

    public void NotifyAnchorsOfBridgeAddition(BridgeBehaviour bridge)
    {
        for (int anchorIndex = 0; anchorIndex != m_anchors.Count; anchorIndex++)
        {
            m_anchors[anchorIndex].PushBridge(bridge);
        }
    }

    public void NotifyAnchorsOfBridgeRemoval(BridgeBehaviour bridge)
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

