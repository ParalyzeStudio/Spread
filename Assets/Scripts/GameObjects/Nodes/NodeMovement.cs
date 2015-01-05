using UnityEngine;
using System.Collections;

public class NodeMovement : MonoBehaviour 
{
    public const float NODE_MOVE_SPEED = 350.0f;

    public bool m_isGUIItem; //if it is only a guit item we just need to use the m_movement points variables
    private GridAnchor m_moveStartAnchor; //the anchor this node starts from when moving
    private GridAnchor m_moveEndAnchor; //the anchor this node ends to when moving
    private bool m_moving; //is this node moving or not?
    public int m_movementPoints; //a simple node can only make a finite number of moves
    public GameObject m_movementPointsPrefab; //the text mesh prefab that holds the movement points text
    private GameObject m_movementPointsTextMesh; //the text mesh itself
    public float m_movementPointsTextMeshYOffset; //the text needs to be offset to be centered in the node

    public void Start()
    {
        if (!m_isGUIItem)
        {
            Vector3 movementPointsTextMeshPosition = transform.position;
            movementPointsTextMeshPosition += new Vector3(0, m_movementPointsTextMeshYOffset, -1); //set the position of the text mesh just above the node one
            m_movementPointsTextMesh = (GameObject)Instantiate(m_movementPointsPrefab, movementPointsTextMeshPosition, Quaternion.identity);
            InvalidateMovementPointsTextMeshValue();
        }
    }

    public void MoveParentNodeToAnchor(GridAnchor anchor)
    {
        if (m_movementPoints > 0)
        {
            m_moveStartAnchor = this.GetComponent<NodeTouchHandler>().m_attachedAnchor;
            m_moveEndAnchor = anchor;
            m_moving = true;
            m_movementPoints--;
        }
    }

    void InvalidateMovementPointsTextMeshValue()
    {
        m_movementPointsTextMesh.GetComponent<TextMesh>().text = m_movementPoints.ToString();
    }

    void Update()
    {
        if (m_isGUIItem)
            return;

        float dt = Time.deltaTime;

        if (m_moving)
        {
            Vector2 positionVec2 = transform.position;
            Vector2 direction = (m_moveEndAnchor.m_position - m_moveStartAnchor.m_position);
            float fTotalSqrDistance = direction.sqrMagnitude;
            direction.Normalize();
            positionVec2 += (direction * NODE_MOVE_SPEED * dt);

            float fCoveredSqrDistance = (positionVec2 - m_moveStartAnchor.m_position).sqrMagnitude;
            float fZValue = GetComponent<NodeProperties>().GetZValue();
            if (fCoveredSqrDistance > fTotalSqrDistance)
            {
                transform.position = new Vector3(m_moveEndAnchor.m_position.x, m_moveEndAnchor.m_position.y, fZValue);
                m_moving = false;
                m_moveEndAnchor.Spread();
                this.GetComponent<NodeTouchHandler>().m_attachedAnchor = m_moveEndAnchor;
                InvalidateMovementPointsTextMeshValue();
            }
            else
                transform.position = new Vector3(positionVec2.x, positionVec2.y, fZValue);
        }

        float textMeshZValue = m_movementPointsTextMesh.transform.position.z;
        m_movementPointsTextMesh.transform.position = new Vector3(transform.position.x, transform.position.y + m_movementPointsTextMeshYOffset, textMeshZValue);
    }
}
