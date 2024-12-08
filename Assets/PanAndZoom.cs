using System.IO;
using UnityEngine;

public class PanAndZoom : MonoBehaviour
{
    public Camera PanCamera;

    public float zoomSpeed = 30f;

    public float panSpeed = 20f;

    public float sensitivity = 1f;

    public float minZoom = 20f;

    public float maxZoom = 5f;

    private Vector3 dragOrigin;

    private float targetZoom;

    //public Transform[] pins;
    private Vector2 mapSize;

    public GameObject paintableCanvas;

    private PinDataCollection pinList;

    private PinDataSender pinDataSender;

    private string jsonPath = "C:/Users/kendl/AppData/LocalLow/DefaultCompany/BaProject/pins.json";

    internal void Start()
    {

        targetZoom = PanCamera.orthographicSize;
        mapSize = paintableCanvas.GetComponent<RectTransform>().sizeDelta;

        pinDataSender = FindAnyObjectByType<PinDataSender>();
        if (File.Exists(jsonPath))
        {
            string json = File.ReadAllText(jsonPath);
            pinList = JsonUtility.FromJson<PinDataCollection>(json);
        }
        else
        {
            Debug.LogError("Pin JSON file not found!");
        }
    }

    internal void Update()
    {
        HandlePan();
        HandleZoom();
    }

    internal void HandleZoom()
    {
        targetZoom -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        targetZoom = Mathf.Clamp(targetZoom, maxZoom, minZoom);
        PanCamera.orthographicSize = Mathf.MoveTowards(PanCamera.orthographicSize, targetZoom, zoomSpeed * Time.deltaTime);
    }

    internal void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = PanCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - PanCamera.ScreenToWorldPoint(Input.mousePosition);
            PanCamera.transform.position += difference;
            difference.z = 0;

            UpdatePinsRelativeToMap(difference);
        }

        if (Input.GetMouseButtonUp(1))
        {
            //  OnUserInteractionEnd();
        }
    }

    internal void UpdatePinsRelativeToMap(Vector3 difference)
    {
        difference /= 10f;

        foreach (var pin in pinList.pinDataList)
        {

            Vector3 pinPosition = pin.position;

            pinPosition.x += difference.x;
            pinPosition.y += difference.y;
            pinPosition.z += difference.z;
        
            pin.position = pinPosition;

            PinPositionUpdate pinPositionUpdate = new PinPositionUpdate();
            pinPositionUpdate.id = pin.id;
            pinPositionUpdate.position.x = pinPosition.x;
            pinPositionUpdate.position.y = pinPosition.y;
            //keep the z position from the json file
            pinPositionUpdate.position.z = 1f;

            Debug.Log("Pin ID: " + pinPositionUpdate.id + " x: " + pinPositionUpdate.position.x + " y: " + pinPositionUpdate.position.y + " z: " + pinPositionUpdate.position.z);
            pinDataSender.SendPositionUpdate(pinPositionUpdate);

        }
    }
}
