using System.Collections;z
using UnityEngine;
using Cinemachine;

public class PinchDetection : MonoBehaviour
{
    [SerializeField] private float zoomSpeed;
    [SerializeField] CinemachineVirtualCamera vcam;

    private TouchControls controls;
    private Coroutine zoomCoroutine;
    private Transform cameraTransform;

    private void Awake()
    {
        controls = new TouchControls();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        controls.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        controls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
    }

    private void ZoomStart()
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }

    private void ZoomEnd()
    {
        StopCoroutine(zoomCoroutine);
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f, distance = 0f;
        while(true)
        {
            distance = Vector2.Distance(controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

            if(distance < previousDistance)
            {
                float newOrthographicSize = vcam.m_Lens.OrthographicSize + zoomSpeed;
                newOrthographicSize = Mathf.Clamp(newOrthographicSize, 15, CameraSystem.GetBoundWidth() / 4.5f);
                vcam.m_Lens.OrthographicSize = newOrthographicSize;
            }
            else if(distance > previousDistance)
            {
                float newOrthographicSize = vcam.m_Lens.OrthographicSize - zoomSpeed;
                newOrthographicSize = Mathf.Clamp(newOrthographicSize, 15, CameraSystem.GetBoundWidth() / 4.5f);
                vcam.m_Lens.OrthographicSize = newOrthographicSize;
            }

            previousDistance = distance;
            yield return null;
        }
    }
}

