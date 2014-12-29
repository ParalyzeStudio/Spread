using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * A GridAnchor is an invisible object where the player can snap a node.
 * It holds a reference to all the bridges that pass through this point.
 * **/
public class GridAnchor
{
    public List<BridgeBehaviour> Bridges {get; set;}
    public Vector2 Position { get; set; }
    public bool isLinked { get; set; } //is this grid anchor linked to the network and reachable by a node 
    public bool isSpread { get; set; } //did we spread around this anchor?
    public List<GridAnchor> NeighbouringAnchors {get; set;}

    public GridAnchor(Vector2 position)
    {
        Position = position;
        Bridges = new List<BridgeBehaviour>();
        isLinked = false;
        NeighbouringAnchors = new List<GridAnchor>();
    }

    public GridAnchor(Vector2 position, List<BridgeBehaviour> bridges)
    {
        Position = position;
        Bridges = bridges;
        isLinked = false;
        NeighbouringAnchors = new List<GridAnchor>();
    }

    /**
     * Adds a bridge to this anchor bridges list
     * **/
    public void PushBridge(BridgeBehaviour bridge)
    {
        //check if bridge is not already in this List 
        for (int bridgeIndex = 0; bridgeIndex != Bridges.Count; bridgeIndex++)
        {
            if (Bridges[bridgeIndex].Equals(bridge))
                return;
        }

        Bridges.Add(bridge);
    }


    /**
     * Remove a bridge from this anchor bridges list
     * **/
    public void RemoveBridge(BridgeBehaviour bridge)
    {
        //check if bridge is not already in this List 
        for (int bridgeIndex = 0; bridgeIndex != Bridges.Count; bridgeIndex++)
        {
            if (Bridges[bridgeIndex].Equals(bridge))
            {
                Bridges.Remove(bridge);
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
        foreach (BridgeBehaviour bridge in Bridges)
        {
            List<GridAnchor> neighbouringAnchorsOnBridge = bridge.GetNeighbouringAnchors(this);
            NeighbouringAnchors.AddRange(neighbouringAnchorsOnBridge);
        }
    }

    /*
     * Spread around this bridge in all possible directions
     * **/
    public void Spread()
    {
        if (!isSpread)
        {
            foreach (BridgeBehaviour bridge in Bridges)
            {
                if (bridge.m_status == BridgeBehaviour.BridgeStatus.Faded) //don't spread on bridges that are done or already spreading
                    bridge.SpreadAroundAnchorPoint(this);
            }

            this.isSpread = true;
        }
    }
}