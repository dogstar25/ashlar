using UnityEngine;
using UnityEngine.InputSystem;

public class BrickSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Key spawnKey = Key.B;
    [SerializeField] private float spawnZ = 0f;

    [Header("Rotation")]
    [SerializeField] private bool randomizeZRotation = true;

    private void Awake()
    {
        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
        {
            return;
        }

        if (Keyboard.current[spawnKey].wasPressedThisFrame)
        {
            SpawnBrickAtMousePosition();
        }
    }

    private void SpawnBrickAtMousePosition()
    {
        if (brickPrefab == null)
        {
            Debug.LogWarning($"{name} has no brick prefab assigned.");
            return;
        }

        if (gameplayCamera == null)
        {
            Debug.LogWarning($"{name} has no gameplay camera assigned.");
            return;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        if (!TryGetMouseWorldPosition(mouseScreenPosition, out Vector3 worldPosition))
        {
            return;
        }

        Quaternion rotation = Quaternion.identity;

        if (randomizeZRotation)
        {
            float zRotation = Random.Range(0f, 360f);
            rotation = Quaternion.Euler(0f, 0f, zRotation);
        }

        Instantiate(brickPrefab, worldPosition, rotation, spawnParent);
    }

    private bool TryGetMouseWorldPosition(Vector2 mouseScreenPosition, out Vector3 worldPosition)
    {
        Ray ray = gameplayCamera.ScreenPointToRay(mouseScreenPosition);

        Plane brickPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, spawnZ));

        if (brickPlane.Raycast(ray, out float distance))
        {
            worldPosition = ray.GetPoint(distance);
            worldPosition.z = spawnZ;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }
}