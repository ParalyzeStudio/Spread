using UnityEngine;
using System.Collections;

public class BridgeMaterialSwitcher : MonoBehaviour 
{
    private Bridge.BridgeType m_previousType;

	/**
     * Switchs over available materials at runtime and in editor depending on the type of the bridge
     * **/
	void Update () 
    {
        Bridge.BridgeType newBridgeType = this.gameObject.GetComponent<Bridge>().m_type;
        if (m_previousType != newBridgeType)
        {
            m_previousType = newBridgeType;
            MaterialHolder materialHolder = GetComponent<MaterialHolder>();
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (newBridgeType == Bridge.BridgeType.Completed || newBridgeType == Bridge.BridgeType.Spreading)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(0); //white texture
            }
            else if (newBridgeType == Bridge.BridgeType.Faded)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(1); //white texture with 0.12f opacity
            }
            else if (newBridgeType == Bridge.BridgeType.TargetIndicator)
            {
                meshRenderer.sharedMaterial = materialHolder.GetMaterialAtIndex(2); //red texture
            }
        }
	}
}
