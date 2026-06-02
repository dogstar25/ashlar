using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] private bool deleteOldFourTriangleVisuals = true;

#if UNITY_EDITOR
    private const string MeshFolderPath = "Assets/Generated/BrickMeshes";
#endif

    [ContextMenu("Rebuild Two Triangle Visuals")]
    public void RebuildTwoTriangleVisuals()
    {
        if (deleteOldFourTriangleVisuals)
        {
            DeleteChildIfExists("TriangleTop");
            DeleteChildIfExists("TriangleRight");
            DeleteChildIfExists("TriangleBottom");
            DeleteChildIfExists("TriangleLeft");
        }

        DeleteChildIfExists(triangleAName);
        DeleteChildIfExists(triangleBName);

        Mesh triangleAMesh = CreateTriangleAMesh();
        Mesh triangleBMesh = CreateTriangleBMesh();

#if UNITY_EDITOR
        triangleAMesh = SaveMeshAsset(triangleAMesh, "BrickTriangleA.asset");
        triangleBMesh = SaveMeshAsset(triangleBMesh, "BrickTriangleB.asset");
#endif

        Renderer triangleARenderer = CreateTriangleChild(triangleAName, triangleAMesh);
        Renderer triangleBRenderer = CreateTriangleChild(triangleBName, triangleBMesh);

        if (disableRootMeshRenderer && TryGetComponent<MeshRenderer>(out MeshRenderer rootRenderer))
        {
            rootRenderer.enabled = false;
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
        EditorUtility.SetDirty(triangleARenderer.gameObject);
        EditorUtility.SetDirty(triangleBRenderer.gameObject);
#endif

        Debug.Log(
            $"Built two triangle visuals for {name}. Assign TriangleA and TriangleB to Brick.cs.",
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
        GetFacePoints(
            out Vector3 topLeft,
            out Vector3 topRight,
            out Vector3 bottomRight,
            out Vector3 bottomLeft);

        return CreateTriangleMesh(
            "Brick Triangle A",
            topLeft,
            bottomRight,
            bottomLeft);
    }

    private Mesh CreateTriangleBMesh()
    {
        GetFacePoints(
            out Vector3 topLeft,
            out Vector3 topRight,
            out Vector3 bottomRight,
            out Vector3 bottomLeft);

        return CreateTriangleMesh(
            "Brick Triangle B",
            topLeft,
            topRight,
            bottomRight);
    }

    private Mesh CreateTriangleMesh(string meshName, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        Mesh mesh = new Mesh();
        mesh.name = meshName;

        mesh.vertices = new[]
        {
            vertex0,
            vertex1,
            vertex2
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
            ToUV(vertex0),
            ToUV(vertex1),
            ToUV(vertex2)
        };

        mesh.RecalculateBounds();

        return mesh;
    }

    private void GetFacePoints(
        out Vector3 topLeft,
        out Vector3 topRight,
        out Vector3 bottomRight,
        out Vector3 bottomLeft)
    {
        float left = -width * 0.5f;
        float right = width * 0.5f;
        float bottom = -height * 0.5f;
        float top = height * 0.5f;
        float z = -(depth * 0.5f + faceOffset);

        topLeft = new Vector3(left, top, z);
        topRight = new Vector3(right, top, z);
        bottomRight = new Vector3(right, bottom, z);
        bottomLeft = new Vector3(left, bottom, z);
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

#if UNITY_EDITOR
    private Mesh SaveMeshAsset(Mesh mesh, string assetFileName)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Generated"))
        {
            AssetDatabase.CreateFolder("Assets", "Generated");
        }

        if (!AssetDatabase.IsValidFolder(MeshFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Generated", "BrickMeshes");
        }

        string assetPath = $"{MeshFolderPath}/{assetFileName}";

        Mesh existingMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        if (existingMesh != null)
        {
            EditorUtility.CopySerialized(mesh, existingMesh);
            EditorUtility.SetDirty(existingMesh);
            AssetDatabase.SaveAssets();
            return existingMesh;
        }

        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
    }
#endif
}