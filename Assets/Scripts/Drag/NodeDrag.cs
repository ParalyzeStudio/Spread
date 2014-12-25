using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class NodeDrag : TouchHandler
{
    public float m_snapDistance;
    public GameObject m_nodePrefab;
    private GameController m_gameController;
    private bool m_snapped;
    private GridAnchor m_snappedAnchor;

    public override void Start()
    {
        base.Start();

        //retrieve anchors from the game controller
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        m_gameController = gameControllerObject.GetComponent<GameController>();

        m_snapped = false;
        m_snappedAnchor = null;
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
                    m_snappedAnchor = snapAnchor;
                    this.transform.position = snapAnchor.Position;

                    List<BridgeBehaviour> anchorBridges = snapAnchor.Bridges;
                    foreach (BridgeBehaviour anchorBridge in anchorBridges)
                    {
                        anchorBridge.SpreadAroundAnchorPoint(m_snappedAnchor);
                        m_snappedAnchor.Linked = true; //this anchor became active
                    }
                    return true;
                }
            }
        }

        return false;
    }

    ///**
    // * Try to unsnap a node to an anchor if the distance to it is big enough ( > m_snapDistance)
    // * **/
    //void TryToUnsnap()
    //{
    //    float fDistanceFromMouseToAnchor = (GetMouseWorldPosition() - m_snappedAnchor.Position).magnitude;
    //    if (fDistanceFromMouseToAnchor > m_snapDistance)
    //    {
    //        m_snapped = false;
    //        m_snappedAnchor = null;
    //    }
    //}
}
