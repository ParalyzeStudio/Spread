using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * A GridAnchor is an invisible object where the player can snap a node.
 * It holds a reference to all the bridges that pass through this point.
 * **/
public class GridAnchor
{
    public List<Bridge> m_bridges {get; set;}
    public Vector2 m_position { get; set; }
    public bool m_linked { get; set; } //is this grid anchor linked to the network and reachable by a node 
    public bool m_spread { get; set; } //did we spread around this anchor?
    public List<GridAnchor> m_neighbouringAnchors {get; set;}

    public GridAnchor(Vector2 position)
    {
        m_position = position;
        m_bridges = new List<Bridge>();
        m_linked = false;
        m_neighbouringAnchors = new List<GridAnchor>();
    }

    public GridAnchor(Vector2 position, List<Bridge> bridges)
    {
        m_position = position;
        m_bridges = bridges;
        m_linked = false;
        m_neighbouringAnchors = new List<GridAnchor>();
    }

    /**
     * Adds a bridge to this anchor bridges list
     * **/
    public void PushBridge(Bridge bridge)
    {
        //check if bridge is not already in this List 
        for (int bridgeIndex = 0; bridgeIndex != m_bridges.Count; bridgeIndex++)
        {
            if (m_bridges[bridgeIndex].Equals(bridge))
                return;
        }

        m_bridges.Add(bridge);
    }


    /**
     * Remove a bridge from this anchor bridges list
     * **/
    public void RemoveBridge(Bridge bridge)
    {
        //check if bridge is not already in this List 
        for (int bridgeIndex = 0; bridgeIndex != m_bridges.Count; bridgeIndex++)
        {
            if (m_bridges[bridgeIndex].Equals(bridge))
            {
                m_bridges.Remove(bridge);
                return;
            }
        }
    }

    ///**
    // * Retrieves the list of linked anchors that are directly neighbors of this anchor
    // * **/
    //public List<GridAnchor> FindNeighbouringLinkedAnchors()
    //{
    //    List<GridAnchor> neighbouringLinkedAnchors = new List<GridAnchor>();

    //    foreach (BridgeBehaviour bridge in Bridges)
    //    {
    //        if (bridge.m_status == BridgeBehaviour.BridgeStatus.Completed)
    //        {
    //            List<GridAnchor> neighbouringLinkedNodesOnBridge = bridge.GetNeighbouringLinkedAnchors(this);
    //            neighbouringLinkedAnchors.AddRange(neighbouringLinkedNodesOnBridge);
    //        }
    //    }

    //    return neighbouringLinkedAnchors;
    //}

    /**
    * Retrieves the list of linked anchors that are directly neighbors of this anchor and cache them
    * **/
    public void InitNeighbouringAnchors()
    {
        foreach (Bridge bridge in m_bridges)
        {
            List<GridAnchor> neighbouringAnchorsOnBridge = bridge.GetNeighbouringAnchors(this);
            m_neighbouringAnchors.AddRange(neighbouringAnchorsOnBridge);
        }
    }

    /*
     * Spread around this anchor in all possible directions
     * **/
    public void Spread()
    {
        if (!m_spread)
        {
            foreach (Bridge bridge in m_bridges)
            {
                BridgeSpread bridgeSpreadComponent = bridge.gameObject.GetComponent<BridgeSpread>();
                if (bridge.m_type == Bridge.BridgeType.Faded && bridgeSpreadComponent.m_spreadBridge == null) //don't spread on bridges that are done or already spreading
                    bridgeSpreadComponent.SpreadAroundAnchorPoint(this);
            }

            this.m_spread = true;
        }
    }
}