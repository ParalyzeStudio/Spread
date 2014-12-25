using UnityEngine;

[ExecuteInEditMode]
public class UVQuad : MonoBehaviour
{
    public Vector4 m_textureRange;
    public TextureWrapMode m_textureWrapMode;

    //private variables to prevent unity from calling update functions for nothing
    private Vector4 m_prevTextureRange;
    private TextureWrapMode m_prevTextureWrapMode;

    protected virtual void Awake()
    {
        m_prevTextureRange = m_textureRange;
        m_prevTextureWrapMode = m_textureWrapMode;
    }

    protected virtual void Start()
    {
        InitQuadMesh();
    }

    /**
     * Init a quad mesh (2 triangles, 4 vertices, 6 indices) and set the correct UVs to be able to repeat or clamp a texture over the whole quad
     * **/
    protected void InitQuadMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, 0.5f, 0.0f); //top-left
        vertices[1] = new Vector3(0.5f, 0.5f, 0.0f); //top-right
        vertices[2] = new Vector3(-0.5f, -0.5f, 0.0f); //bottom-left
        vertices[3] = new Vector3(0.5f, -0.5f, 0.0f); //bottom-right
        mesh.vertices = vertices;

        int[] indices = new int[6] { 0, 1, 2, 3, 2, 1 };
        mesh.triangles = indices;

        //uvs are likely to change on every frame, create a function and call it in the Update() method (also useful for updating the scene in Unity in Edit mode)
        UpdateUVs();

        Vector3[] normals = new Vector3[4] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
        mesh.normals = normals;
    }

    /**
     * Simply update the UVs array
     * **/
    private void UpdateUVs()
    {
        if (m_textureRange.Equals(m_prevTextureRange)) //same value do not update
            return;

        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(m_textureRange.x, m_textureRange.w); //top-left
        uvs[1] = new Vector2(m_textureRange.z, m_textureRange.w); //top-right
        uvs[2] = new Vector2(m_textureRange.x, m_textureRange.y); //bottom-left
        uvs[3] = new Vector2(m_textureRange.z, m_textureRange.y); //bottom-right

        GetComponent<MeshFilter>().sharedMesh.uv = uvs;

        m_prevTextureRange = m_textureRange;
    }

    /**
     * Switch between CLAMP and REPEAT wrap modes
     * **/
    private void UpdateWrapMode()
    {
        if (m_textureWrapMode.Equals(m_prevTextureWrapMode)) //same value do not update
            return;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.mainTexture.wrapMode = m_textureWrapMode;

        m_prevTextureWrapMode = m_textureWrapMode;
    }

    protected virtual void Update()
    {
        UpdateUVs();
        UpdateWrapMode();
    }
}

