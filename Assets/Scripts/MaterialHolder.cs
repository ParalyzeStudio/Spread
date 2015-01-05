using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * DEPRECATED - needs removal
 * **/
public class MaterialHolder : MonoBehaviour 
{
    public List<Material> materials;

    public Material GetMaterialAtIndex(int index)
    {
        return materials[index];
    }
}
