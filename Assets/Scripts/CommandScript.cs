using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CommandScript : MonoBehaviour
{
    [SerializeField] private float commandSphereScale = 0.2f;

    private Transform movePointsObject;
    private Transform commandSphere;
    private Camera thisCamera;

    private InputAction clickAction;

    private List<Transform> movePoints = new List<Transform>();

    private GameObject highlightedMovePoint = null;

    private void Awake()
    {
        clickAction = InputSystem.actions.FindAction("MouseClick");

        commandSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        commandSphere.localScale = new Vector3(commandSphereScale, commandSphereScale, commandSphereScale);
        commandSphere.GetComponent<Renderer>().material.color = Color.green;
        commandSphere.GetComponent<SphereCollider>().isTrigger = true;
        commandSphere.gameObject.layer = LayerMask.NameToLayer("MovePoint");
        commandSphere.tag = "MovePoint";
        commandSphere.gameObject.SetActive(false);

        thisCamera = transform.GetComponent<Camera>();
        movePointsObject = new GameObject("MovePoints").transform;
    }

    private void Update()
    {
        HiglightMovePoints();
        HandleClick();
    }

    public List<Transform> GetMovePoints()
    {
        return new List<Transform>(movePoints);
    }

    public void RemoveMovePoint(Transform point)
    {
        if (movePoints.Contains(point))
        {
            movePoints.Remove(point);
            Destroy(point.gameObject);
        }
    }

    private void OnDisable()
    {
        if (highlightedMovePoint)
        {
            highlightedMovePoint.GetComponent<Renderer>().material.color = Color.green;
            highlightedMovePoint = null;
        }
    }

    private void HiglightMovePoints()
    {
        GameObject obj = GetObjectAtCursor();
        if (obj == null)
        {
            StopHighlightingCurrentMovePoint();
            return;
        }

        if (obj.tag != "MovePoint")
        {
            StopHighlightingCurrentMovePoint();
            return;
        }

        if (obj == highlightedMovePoint)
        {
            return;
        }

        obj.GetComponent<Renderer>().material.color = Color.white;
        highlightedMovePoint = obj;
    }

    private void HandleClick()
    {
        if (!clickAction.WasPressedThisFrame())
        {
            return;
        }

        if (clickAction.ReadValue<float>() > 0)
        {
            // So that we don't make another move point on top of already existing one
            if (!highlightedMovePoint)
            {
                MakeNewMovePointAtCursor();
            }
            
        } else
        {
            RemoveHighlightedMovePoint();
        }
    }

    private void MakeNewMovePointAtCursor()
    {
        Vector3? cursorPositionCheck = GetCursorWorldPositon();
        // We didn't hit anything. What else can we do lmao
        if (cursorPositionCheck == null)
        {
            return;
        }

        Vector3 currentCursorPosition = (Vector3)cursorPositionCheck;
        Transform newSphere = Instantiate(commandSphere, movePointsObject);
        newSphere.position = currentCursorPosition;
        movePoints.Add(newSphere);
        newSphere.gameObject.SetActive(true);
    }
    
    private void RemoveHighlightedMovePoint()
    {
        if (highlightedMovePoint)
        {
            RemoveMovePoint(highlightedMovePoint.transform);
            highlightedMovePoint = null;
        }
    }
    
    private Vector3? GetCursorWorldPositon()
    {
        Ray cameraRay = thisCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit))
        {
            return hit.point;
        } else
        {
            return null;
        }
    }

    private GameObject GetObjectAtCursor()
    {
        Ray cameraRay = thisCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit))
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
    
    private void StopHighlightingCurrentMovePoint()
    {
        if (highlightedMovePoint)
        {
            highlightedMovePoint.GetComponent<Renderer>().material.color = Color.green;
            highlightedMovePoint = null;
        }
    }
}