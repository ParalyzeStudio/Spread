using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NodeTouchHandler : TouchHandler
{
    public float m_snapDistance;
    private GameController m_gameController;
    private GridAnchor m_snappedAnchor; //the current anchor the node is snapped to
    public GridAnchor m_attachedAnchor { get; set; } //the anchor the node is attached too and where it can move to its neighbouring anchors
    private GridAnchor m_targetAnchor; //the anchor we target when moving the node from the attached anchor (determined in the first OnPointerMove)
    private GameObject m_targetIndicatorNode; //node we can drag onto a target anchor
    private GameObject m_parentNode; //the parent node of the target indicator node

    public void Awake()
    {
        m_snappedAnchor = null;
        m_attachedAnchor = null;
        m_targetAnchor = null;
        m_targetIndicatorNode = null;
        m_parentNode = null;
    }

    public override void Start()
    {
        base.Start();

        //retrieve anchors from the game controller
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        m_gameController = gameControllerObject.GetComponent<GameController>();
    }

    protected override bool OnPointerMove(Vector2 pointerLocation, ref Vector2 delta)
    {
        if (!base.OnPointerMove(pointerLocation, ref delta))
            return false;

        NodeBehaviour nodeBehaviour = this.GetComponent<NodeBehaviour>();
             
        if (nodeBehaviour.m_nodeType == NodeBehaviour.NodeType.Simple)  //SIMPLE NODES
        {
            if (m_attachedAnchor == null)//node can move freely (i.e it has been dragged from GUI item list)
            {
                SnapToClosestAnchor();
                if (m_snappedAnchor != null)
                {
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
                if (nodeBehaviour.m_movementPoints > 0 && m_targetIndicatorNode == null)
                {
                    //instantiate a special node that will serve as a indicator that can snap on neighbouring reachable anchors
                    Vector3 targetIndicatorPosition = transform.position;
                    targetIndicatorPosition.z = NodeBehaviour.TARGET_ANCHOR_NODE_Z_VALUE;
                    m_targetIndicatorNode = (GameObject) Instantiate(nodeBehaviour.m_simpleNodePrefab,
                                                                     targetIndicatorPosition,
                                                                     Quaternion.identity);

                    m_targetIndicatorNode.GetComponent<NodeBehaviour>().m_nodeType = NodeBehaviour.NodeType.TargetIndicator;
                    m_targetIndicatorNode.GetComponent<NodeTouchHandler>().m_attachedAnchor = this.m_attachedAnchor;
                    m_targetIndicatorNode.GetComponent<NodeTouchHandler>().m_parentNode = this.gameObject;

                    //change the color of the node
                    MeshRenderer nodeRenderer = m_targetIndicatorNode.GetComponent<MeshRenderer>();
                    Material targetIndicatorMaterial = (Material)Resources.LoadAssetAtPath("Materials/target_indicator_material", typeof(Material));
                    if (targetIndicatorMaterial != null)
                    {
                        Debug.Log("found material");
                        nodeRenderer.sharedMaterial = targetIndicatorMaterial;
                    }
                    else
                        Debug.Log("NOT found material");
                    //nodeRenderer.material.SetVector("_Color", new Vector4(1, 0, 0, 1));
                }
            }
        }
        else if (nodeBehaviour.m_nodeType == NodeBehaviour.NodeType.TargetIndicator)
        {
            SnapToClosestAnchor();
            bool bMoveTargetIndicator = false;
            if (m_snappedAnchor != null)
            {
                TryToUnsnap();
                if (m_snappedAnchor == null) //unsnap succeeded
                    bMoveTargetIndicator = true;
            }
            else
                bMoveTargetIndicator = true;

            if (bMoveTargetIndicator)
            {
                FindTargetAnchor();

                Vector2 attachedAnchorToMouseDirection = CoordinatesUtils.SharedInstance.GetMousePositionInWorldCoordinates() - m_attachedAnchor.m_position;
                Vector2 attachedAnchorToTargetAnchorDirection = m_targetAnchor.m_position - m_attachedAnchor.m_position;
                float fDistanceToTargetAnchor = attachedAnchorToTargetAnchorDirection.magnitude;
                attachedAnchorToTargetAnchorDirection.Normalize();

                //Project the first vector on the second one
                float fProjectionLength = Vector2.Dot(attachedAnchorToMouseDirection, attachedAnchorToTargetAnchorDirection);

                if (fProjectionLength <= fDistanceToTargetAnchor)
                {
                    //set the new position of target indicator node
                    float targetIndicatorNodeZValue = transform.position.z;
                    transform.position = m_attachedAnchor.m_position + attachedAnchorToTargetAnchorDirection * fProjectionLength;
                    transform.position = new Vector3(transform.position.x, transform.position.y, targetIndicatorNodeZValue);
                }
                else
                {
                    Vector3 targetAnchorPosition = m_targetAnchor.m_position;
                    targetAnchorPosition.z = NodeBehaviour.TARGET_ANCHOR_NODE_Z_VALUE;
                    transform.position = targetAnchorPosition;
                }
            }
        }

        return true;
    }

    protected override void OnPointerUp()
    {
        base.OnPointerUp();

        NodeBehaviour nodeBehaviour = this.GetComponent<NodeBehaviour>();
        if (nodeBehaviour.m_nodeType == NodeBehaviour.NodeType.Simple)
        {
            if (m_attachedAnchor == null)
            {
                if (m_snappedAnchor != null)
                {
                    m_attachedAnchor = m_snappedAnchor;
                    m_snappedAnchor.Spread();
                    Destroy(nodeBehaviour.m_deactivatedGUIItem);
                }
                else
                {
                    Destroy(this.gameObject);
                    nodeBehaviour.m_deactivatedGUIItem.SetActive(true);
                }
            }
        }
        else if (nodeBehaviour.m_nodeType == NodeBehaviour.NodeType.TargetIndicator)
        {
            if (m_snappedAnchor != null)
            {
                this.m_parentNode.GetComponent<NodeBehaviour>().MoveParentNodeToAnchor(m_snappedAnchor);               
            }
            Destroy(this.gameObject);
        }
    }

    /**
     * Try to snap a node to an anchor if the distance to it is small enough ( <= m_snapDistance)
     * **/
    bool SnapToClosestAnchor()
    {
        List<GridAnchor> allAnchors = m_gameController.m_anchors;

        for (int anchorIndex = 0; anchorIndex != allAnchors.Count; anchorIndex++)
        {
            GridAnchor snapAnchor = allAnchors[anchorIndex];

            Vector2 position = this.transform.position;
            float fDistanceToAnchor = (position - snapAnchor.m_position).magnitude;

            if (snapAnchor != m_snappedAnchor)
            {
                if (fDistanceToAnchor <= m_snapDistance)
                {
                    m_snappedAnchor = snapAnchor;
                    transform.position = new Vector3(snapAnchor.m_position.x, snapAnchor.m_position.y, transform.position.z);

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
        float fDistanceFromMouseToAnchor = (mouseWorldPosition - m_snappedAnchor.m_position).magnitude;
        if (fDistanceFromMouseToAnchor > m_snapDistance)
        {
            this.transform.position = mouseWorldPosition;

            //reset the zvalue
            float nodeZValue = this.GetComponent<NodeBehaviour>().GetZValue();
            transform.position = new Vector3(transform.position.x, transform.position.y, nodeZValue);

            m_snappedAnchor = null;
        }
    }

    /*
     * Find the target Anchor where the pointer is heading for when trying to move one of his nodes
     * **/
    private void FindTargetAnchor()
    {
        Vector2 attachedAnchorToMouseDirection = CoordinatesUtils.SharedInstance.GetMousePositionInWorldCoordinates() - m_attachedAnchor.m_position;
        List<GridAnchor> neighbouringAnchors = m_attachedAnchor.m_neighbouringAnchors;

        //find the direction where to drag the node based on the value of the dot product of mouseToAttachedAnchor direction and direction form the attached anchor ta neighbouring anchor
        //the max value of this product (i.e smallest angle between vectors) determines the anchor we have to head for
        float maxDotProduct = float.MinValue;
        foreach (GridAnchor neighbouringAnchor in neighbouringAnchors)
        {
            Vector2 attachedAnchorToNeighbouringAnchorDirection = neighbouringAnchor.m_position - m_attachedAnchor.m_position;
            attachedAnchorToNeighbouringAnchorDirection.Normalize();

            float dotProduct = Vector2.Dot(attachedAnchorToNeighbouringAnchorDirection, attachedAnchorToMouseDirection);
            if (dotProduct > maxDotProduct)
            {
                maxDotProduct = dotProduct;
                m_targetAnchor = neighbouringAnchor;
            }
        }
    }
}
