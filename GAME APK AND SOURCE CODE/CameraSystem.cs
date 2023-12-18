using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] GameObject boundsObject;
    [SerializeField] CinemachineVirtualCamera vcam;

    // private float mouseButtonHoldTime = .5f;
    private float dragPanSpeed = 8f;
    private bool dragPanMoveActive = false;
    private Vector2 lastFingerPosition;
    private TouchControls controls;
    private Coroutine dragCoroutine;
    private Transform cameraTransform;

    public static float boundWidth;
    public static float boundHeight;

    private void Awake()
    {
        controls = new TouchControls();
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        int width, height;
        float cellSize;
        GridMap<GridSpace> grid = Pathfinding.Instance.GetGrid();
        PolygonCollider2D polyCollider = boundsObject.GetComponent<PolygonCollider2D>();
        width = grid.GetWidth();
        height = grid.GetHeight();
        cellSize = grid.GetCellSize();

        Vector2[] vertices = new Vector2[4];
        vertices[0] = new Vector2(0, 0);
        vertices[1] = new Vector2(0, height * cellSize);
        vertices[2] = new Vector2(width * cellSize, height * cellSize);
        vertices[3] = new Vector2(width * cellSize, 0);

        boundWidth = (float)cellSize * (float)width;
        boundHeight = (float)cellSize * (float)height;


        polyCollider.points = vertices;

        transform.position = Player.Instance.GetPosition();

        controls.Touch.PrimaryTouchContact.started += _ => DragStart();
        controls.Touch.PrimaryTouchContact.canceled += _ => DragEnd();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void DragStart()
    {
        dragCoroutine = StartCoroutine(DragDetection());
    }

    private void DragEnd()
    {
        StopCoroutine(dragCoroutine);
    }

    public static float GetBoundWidth()
    {
        return boundWidth;
    }

    IEnumerator DragDetection()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);
        lastFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        while(true)
        {
            Vector2 fingerMovementDelta = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>() - lastFingerPosition;
            inputDir.x = (fingerMovementDelta.x / (300 * (1 / vcam.m_Lens.OrthographicSize)) * -1); //* dragPanSpeed;
            inputDir.y = (fingerMovementDelta.y / (300 * (1 / vcam.m_Lens.OrthographicSize)) * -1); //* dragPanSpeed;
            lastFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();  

            Vector3 moveDir = transform.up * inputDir.y + transform.right * inputDir.x;
            if(!(transform.position.x > cameraTransform.position.x) && 
                !(transform.position.x < cameraTransform.position.x) &&
                 !(transform.position.y > cameraTransform.position.y) && 
                  !(transform.position.y < cameraTransform.position.y))
            {
                transform.position += moveDir;
            }
            else
            {
                transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, -10);
            }
            yield return null;
        }
    }
}