using UnityEngine;
using UnityEngine.InputSystem;

public class BrickDragController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera gameplayCamera;

    [Header("Drag Settings")]
    [SerializeField] private float brickPlaneZ = 0f;
    [SerializeField] private bool straightenBrickOnGrab = true;

    [Header("Temporary Placement Test")]
    [SerializeField] private float placementLineY = 0f;
    [SerializeField] private bool stickIfReleasedAboveLine = true;

    private Brick selectedBrick;
    private Rigidbody selectedRigidbody;
    private Vector3 grabOffset;

    private void Awake()
    {
        if (gameplayCamera == null)
        {
            gameplayCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryBeginDrag();
        }

        if (selectedBrick != null && Mouse.current.leftButton.isPressed)
        {
            UpdateDragPosition();
        }

        if (selectedBrick != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            EndDrag();
        }
    }

    private void TryBeginDrag()
    {
        if (gameplayCamera == null)
        {
            Debug.LogWarning($"{name} has no gameplay camera assigned.");
            return;
        }

        Ray ray = gameplayCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            return;
        }

        Brick brick = hit.collider.GetComponentInParent<Brick>();

        if (brick == null)
        {
            return;
        }

        Rigidbody brickRigidbody = brick.GetComponent<Rigidbody>();

        if (brickRigidbody == null)
        {
            Debug.LogWarning($"{brick.name} has no Rigidbody.");
            return;
        }

        if (!TryGetMouseWorldPosition(out Vector3 mouseWorldPosition))
        {
            return;
        }

        selectedBrick = brick;
        selectedRigidbody = brickRigidbody;
        grabOffset = selectedBrick.transform.position - mouseWorldPosition;
        grabOffset.z = 0f;

        BeginPhysicsDrag();
    }

    private void BeginPhysicsDrag()
    {
        selectedRigidbody.linearVelocity = Vector3.zero;
        selectedRigidbody.angularVelocity = Vector3.zero;

        selectedRigidbody.useGravity = false;
        selectedRigidbody.isKinematic = true;

        if (straightenBrickOnGrab)
        {
            selectedBrick.transform.rotation = Quaternion.identity;
        }
    }

    private void UpdateDragPosition()
    {
        if (!TryGetMouseWorldPosition(out Vector3 mouseWorldPosition))
        {
            return;
        }

        Vector3 targetPosition = mouseWorldPosition + grabOffset;
        targetPosition.z = brickPlaneZ;

        selectedBrick.transform.position = targetPosition;

        if (straightenBrickOnGrab)
        {
            selectedBrick.transform.rotation = Quaternion.identity;
        }
    }

    private void EndDrag()
    {
        bool shouldStick = stickIfReleasedAboveLine &&
                           selectedBrick.transform.position.y >= placementLineY;

        if (shouldStick)
        {
            StickBrickInPlace();
        }
        else
        {
            ReleaseBrickToPhysics();
        }

        selectedBrick = null;
        selectedRigidbody = null;
        grabOffset = Vector3.zero;
    }

    private void StickBrickInPlace()
    {
        Vector3 position = selectedBrick.transform.position;
        position.z = brickPlaneZ;

        selectedBrick.transform.position = position;
        selectedBrick.transform.rotation = Quaternion.identity;

        selectedRigidbody.linearVelocity = Vector3.zero;
        selectedRigidbody.angularVelocity = Vector3.zero;
        selectedRigidbody.useGravity = false;
        selectedRigidbody.isKinematic = true;
    }

    private void ReleaseBrickToPhysics()
    {
        Vector3 position = selectedBrick.transform.position;
        position.z = brickPlaneZ;

        selectedBrick.transform.position = position;

        selectedRigidbody.linearVelocity = Vector3.zero;
        selectedRigidbody.angularVelocity = Vector3.zero;
        selectedRigidbody.useGravity = true;
        selectedRigidbody.isKinematic = false;
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
    {
        Ray ray = gameplayCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane brickPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, brickPlaneZ));

        if (brickPlane.Raycast(ray, out float distance))
        {
            worldPosition = ray.GetPoint(distance);
            worldPosition.z = brickPlaneZ;
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }
}