using UnityEngine;

[ExecuteAlways]
public class BrickVisualBuilder : MonoBehaviour
{
    [Header("Brick Dimensions")]
    [SerializeField] private float width = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private float depth = 0.2f;
    [SerializeField] private float faceOffset = 0.003f;

    [Header("Child Names")]
    [SerializeField] private string triangleAName = "TriangleA";
    [SerializeField] private string triangleBName = "TriangleB";

    [Header("Material")]
    [SerializeField] private Material sharedMaterial;

    [Header("Options")]
    [SerializeField] private bool disableRootMeshRenderer = true;

    [ContextMenu("Rebuild Triangle Visuals")]
    public void RebuildTriangleVisuals()
    {
        DeleteChildIfExists(triangleAName);
        DeleteChildIfExists(triangleBName);

        Renderer triangleARenderer = CreateTriangleChild(triangleAName, CreateTriangleAMesh());
        Renderer triangleBRenderer = CreateTriangleChild(triangleBName, CreateTriangleBMesh());

        if (disableRootMeshRenderer && TryGetComponent<MeshRenderer>(out MeshRenderer rootRenderer))
        {
            rootRenderer.enabled = false;
        }

        Debug.Log(
            $"Built triangle visuals for {name}. Assign {triangleARenderer.name} and {triangleBRenderer.name} to Brick.cs.",
            this);
    }

    private Renderer CreateTriangleChild(string childName, Mesh mesh)
    {
        GameObject child = new GameObject(childName);
        child.transform.SetParent(transform, false);
        child.layer = gameObject.layer;

        MeshFilter meshFilter = child.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();

        if (sharedMaterial != null)
        {
            meshRenderer.sharedMaterial = sharedMaterial;
        }
        else
        {
            meshRenderer.sharedMaterial = CreateFallbackMaterial();
        }

        return meshRenderer;
    }

    private Mesh CreateTriangleAMesh()
    {
        float left = -width * 0.5f;
        float right = width * 0.5f;
        float bottom = -height * 0.5f;
        float top = height * 0.5f;
        float z = -(depth * 0.5f + faceOffset);

        Vector3 topLeft = new Vector3(left, top, z);
        Vector3 bottomLeft = new Vector3(left, bottom, z);
        Vector3 bottomRight = new Vector3(right, bottom, z);

        Mesh mesh = new Mesh();
        mesh.name = "Brick Triangle A";

        mesh.vertices = new[]
        {
            topLeft,
            bottomRight,
            bottomLeft
        };

        mesh.triangles = new[]
        {
            0, 1, 2
        };

        mesh.normals = new[]
        {
            Vector3.back,
            Vector3.back,
            Vector3.back
        };

        mesh.uv = new[]
        {
            ToUV(topLeft),
            ToUV(bottomRight),
            ToUV(bottomLeft)
        };

        mesh.RecalculateBounds();

        return mesh;
    }

    private Mesh CreateTriangleBMesh()
    {
        float left = -width * 0.5f;
        float right = width * 0.5f;
        float bottom = -height * 0.5f;
        float top = height * 0.5f;
        float z = -(depth * 0.5f + faceOffset);

        Vector3 topLeft = new Vector3(left, top, z);
        Vector3 topRight = new Vector3(right, top, z);
        Vector3 bottomRight = new Vector3(right, bottom, z);

        Mesh mesh = new Mesh();
        mesh.name = "Brick Triangle B";

        mesh.vertices = new[]
        {
            topLeft,
            topRight,
            bottomRight
        };

        mesh.triangles = new[]
        {
            0, 1, 2
        };

        mesh.normals = new[]
        {
            Vector3.back,
            Vector3.back,
            Vector3.back
        };

        mesh.uv = new[]
        {
            ToUV(topLeft),
            ToUV(topRight),
            ToUV(bottomRight)
        };

        mesh.RecalculateBounds();

        return mesh;
    }

    private Vector2 ToUV(Vector3 vertex)
    {
        float u = (vertex.x / width) + 0.5f;
        float v = (vertex.y / height) + 0.5f;

        return new Vector2(u, v);
    }

    private void DeleteChildIfExists(string childName)
    {
        Transform existingChild = transform.Find(childName);

        if (existingChild == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(existingChild.gameObject);
        }
        else
        {
            DestroyImmediate(existingChild.gameObject);
        }
    }

    private Material CreateFallbackMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        Material material = new Material(shader);
        material.name = "Generated Brick Visual Material";

        return material;
    }
}