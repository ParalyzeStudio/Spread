using UnityEngine;
using System.Collections;

public class BridgeSpread : MonoBehaviour 
{
    public const float BRIDGE_SPREADING_SPEED = 300.0f;

    public GameObject m_bridgePrefab;
    public Bridge m_spreadBridge { get; set; } //the bridge being spread on this one
    public Bridge m_coveredBridge { get; set; } //the bridge being covered by this one
    private GridAnchor m_spreadAnchor; //the anchor where the spread begins

    /**
     * Create solid bridges that will scale and cover all faded bridges around this anchor
     * **/
    public void SpreadAroundAnchorPoint(GridAnchor anchor)
    {
        Vector3 spreadBridgePosition = anchor.m_position;
        spreadBridgePosition.z = -150.0f;

        GameObject clonedObject = (GameObject)Instantiate(m_bridgePrefab, spreadBridgePosition, this.transform.rotation);
        Transform transform = clonedObject.GetComponent<Transform>();
        transform.localScale = new Vector3(0.0f, transform.localScale.y, transform.localScale.z);
        m_spreadBridge = clonedObject.GetComponent<Bridge>();
        m_spreadBridge.m_type = Bridge.BridgeType.Spreading;
        m_spreadBridge.m_startPoint = anchor.m_position;
        m_spreadBridge.m_endPoint = anchor.m_position;
        m_spreadBridge.PushAnchor(anchor);

        BridgeSpread bridgeSpreadComponent = clonedObject.GetComponent<BridgeSpread>();
        bridgeSpreadComponent.m_spreadAnchor = anchor;
        bridgeSpreadComponent.m_coveredBridge = this.gameObject.GetComponent<Bridge>();
        anchor.m_linked = true;
    }

    /**
     * Spreading bridges are scaled in the update function to make their size grow
     * **/
    protected void Update()
    {
        float dt = Time.deltaTime;

        //handle the special case of a bridge that is currently spreading
        if (GetComponent<Bridge>().m_type == Bridge.BridgeType.Spreading)
        {
            //Check if the bridge spreading is done or increase its size otherwise
            float dx = BRIDGE_SPREADING_SPEED * dt;

            Vector2 director = (m_coveredBridge.m_endPoint - m_coveredBridge.m_startPoint);
            director.Normalize();

            Bridge bridgeComponent = GetComponent<Bridge>();

            float fDistanceFromAnchorToSpreadBridgeStartPoint = (bridgeComponent.m_startPoint - m_spreadAnchor.m_position).magnitude;
            float fDistanceFromAnchorToSpreadBridgeEndPoint = (bridgeComponent.m_endPoint - m_spreadAnchor.m_position).magnitude;
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
                    bridgeComponent.m_startPoint = m_coveredBridge.m_startPoint;
                }
                else
                    bridgeComponent.m_startPoint -= (director * dx);
            }


            if (fDistanceFromAnchorToSpreadBridgeEndPoint == fDistanceFromAnchorToCoveredBridgeEndPoint) //bridge already hit the border
                bEndPointReached = true;
            else
            {
                if (fDistanceFromAnchorToSpreadBridgeEndPoint > fDistanceFromAnchorToCoveredBridgeEndPoint)
                {
                    bEndPointReached = true;
                    bridgeComponent.m_endPoint = m_coveredBridge.m_endPoint;
                }
                else
                    bridgeComponent.m_endPoint += (director * dx);
            }

            //Link every anchor that has been reached by this spreading bridge
            foreach (GridAnchor anchor in m_coveredBridge.m_anchors)
            {
                Vector2 anchorPosition = anchor.m_position;

                if (MathUtils.isLinePointContainedInSegment(anchorPosition, bridgeComponent.m_startPoint, bridgeComponent.m_endPoint))
                {
                    bridgeComponent.PushAnchor(anchor);
                    anchor.m_linked = true;
                }
            }

            //we finish building the spreading bridge
            if (bStartPointReached && bEndPointReached)
            {
                bridgeComponent.m_type = Bridge.BridgeType.Completed;
                bridgeComponent.NotifyAnchorsOfBridgeAddition(bridgeComponent);
                bridgeComponent.NotifyAnchorsOfBridgeRemoval(m_coveredBridge);
                Destroy(m_coveredBridge.gameObject);
                this.transform.position = new Vector3(transform.position.x, transform.position.y, Bridge.SOLID_BRIDGE_Z_VALUE);
            }
        }
    }
}
