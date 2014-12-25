using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BackgroundStaticPosition : MonoBehaviour 
{	
	void Update () 
    {
        Vector3 vCameraPosition = Camera.main.transform.position;
        this.transform.position = new Vector3(vCameraPosition.x, vCameraPosition.y, this.transform.position.z); //background follows the camera
	}
}
