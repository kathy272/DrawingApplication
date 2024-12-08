using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.UI;


[System.Serializable]
public class PinData
{
    public int id;
    public Vector3 position;
    public string title;
    public string content;
    public string filePath;
}

[System.Serializable]
public class PinPositionUpdate
{
    public int id;
    public Vector3 position; 
}


[System.Serializable]
public class PinDataCollection
{
    public List<PinData> pinDataList;

    public PinDataCollection(List<PinData> pins)
    {
        pinDataList = pins;
    }
}

public class PinDataSender : MonoBehaviour
{
    public WebSocket ws;

   // public string jsonPath = "C:/Users/kendl/AppData/LocalLow/DefaultCompany/BaProject/pins.json";
    private PinDataCollection pinList;
    public GameObject paintableCanvas;
    public Sprite Image;

    void Start()
    {
    
        ws = new WebSocket("ws://172.16.100.59:8080");

        ws.OnOpen += (sender, e) => Debug.Log("WebSocket connection opened.");
        ws.OnError += (sender, e) => Debug.LogError("WebSocket error: " + e.Message);
        ws.OnClose += (sender, e) => Debug.Log("WebSocket connection closed: " + e.Reason);

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Received from server: " + e.Data);
            var receivedPinData = JsonUtility.FromJson<PinDataCollection>(e.Data);
            if (receivedPinData != null)
            {
                Debug.Log("Received pin data: " + receivedPinData.pinDataList.Count);
                MainThreadDispatcher.Enqueue(() =>
                {
                    Debug.Log("Main thread action executed.");

                    CreateGameobject(receivedPinData);
                });
            }
            else
            {
                Debug.LogWarning("Received null or invalid JSON data.");
            }
        };

        if (ws != null && !ws.IsAlive)
        {
            ws.Connect();
        }
        else
        {
            Debug.LogWarning("WebSocket is already connected.");
        }

        //  SendPinData();
    }

    public void CreateGameobject(PinDataCollection receivedPinData)
    {
        if (receivedPinData != null && receivedPinData.pinDataList != null)
        {
            foreach (var pin in receivedPinData.pinDataList)
            {
                Debug.Log($"Received pin ID: {pin.id}");

                // Create a new GameObject
                GameObject newPin = new GameObject(pin.id.ToString());
                newPin.transform.SetParent(paintableCanvas.transform);

                float x = pin.position.x * 20f;
                float y = pin.position.z * 20f;
                float z = pin.position.y; // not neccessary in 2D but for resaving the position
                Debug.Log("x: " + x + " y: " + y + " z:" + z);

                newPin.transform.position = new Vector3(x, y, z);
                newPin.transform.localScale = new Vector3(1f, 1f, 1f);

                Image image = newPin.AddComponent<Image>();
                image.sprite = Image;
            }
        }
    }

    /*
         void SendPinData()
        {
            if (ws != null && ws.IsAlive)
            {
                string json = JsonUtility.ToJson(pinList);
                Debug.Log("Sending: " + json);
                ws.Send(json);
            }
            else
            {
                Debug.LogWarning("WebSocket is not connected. Cannot send data.");
            }
        }*/
    public void SendPositionUpdate(PinPositionUpdate update)
    {
        if (ws != null && ws.IsAlive)
        {
            string jsonUpdate = JsonUtility.ToJson(update);
            ws.Send(jsonUpdate);
            Debug.Log("Sent updated pin position: " + jsonUpdate);
        }
        else
        {
            Debug.LogWarning("WebSocket is not connected. Cannot send position update.");
        }
    }

    void OnApplicationQuit()
    {
        if (ws != null)
            ws.Close();
    }
}
