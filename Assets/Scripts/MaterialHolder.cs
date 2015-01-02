using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialHolder : MonoBehaviour 
{
    public List<Material> materials;

    public Material GetMaterialAtIndex(int index)
    {
        return materials[index];
    }
}
