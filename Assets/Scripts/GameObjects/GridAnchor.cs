using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * A GridAnchor is an invisible object where the player can snap a node.
 * It holds a reference to all the bridges that pass through this point.
 * **/
public class GridAnchor
{
    //private List<BridgeBehaviour> m_bridges; //the bridges that intersect to produce this node
    private Vector2 m_position; //the position of the node

    public List<BridgeBehaviour> Bridges {get; set;}
    public Vector2 Position { get; set; }
    public bool Linked { get; set; } //is this grid anchor linked to the network and reachable by a node 

    public GridAnchor(Vector2 position)
    {
        Position = position;
        Bridges = new List<BridgeBehaviour>();
        Linked = false;
    }

    public GridAnchor(Vector2 position, List<BridgeBehaviour> bridges)
    {
        Position = position;
        Bridges = bridges;
        Linked = false;
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
}