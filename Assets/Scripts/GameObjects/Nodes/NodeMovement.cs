using UnityEngine;
using System.Collections;

public class NodeMovement : MonoBehaviour 
{
    public const float NODE_MOVE_SPEED = 300.0f;

    private GridAnchor m_moveStartAnchor; //the anchor this node starts from when moving
    private GridAnchor m_moveEndAnchor; //the anchor this node ends to when moving
    private bool m_moving; //is this node moving or not?
    public int m_movementPoints; //a simple node can only make a finite number of moves

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

    void Update()
    {
        NodeTouchHandler nodeTouchHandler = GetComponent<NodeTouchHandler>();
        if (nodeTouchHandler != null && nodeTouchHandler.m_attachedAnchor != null)
        {
            Debug.Log("m_attachedAnchor X:" + nodeTouchHandler.m_attachedAnchor.m_position.x +
                      " Y:" + nodeTouchHandler.m_attachedAnchor.m_position.y);
        }

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
            }
            else
                transform.position = new Vector3(positionVec2.x, positionVec2.y, fZValue);
        }
    }
}
