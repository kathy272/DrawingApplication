using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class PinManager : MonoBehaviour
{
    public GameObject paintableCanvas; 
    public Sprite Image; 
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
    public class PinDataCollection

    {
        public List<PinData> pinDataList;
    }

    private PinDataCollection pinList;

    void Start()
    {
        // Load the pin data from the JSON file
      /*  string jsonPath = "C:/Users/kendl/AppData/LocalLow/DefaultCompany/BaProject/pins.json";
        string json = File.ReadAllText(jsonPath);
        pinList = JsonUtility.FromJson<PinDataCollection>(json);

        // Place the pins on the canvas
        foreach (var pin in pinList.pinDataList)
        {

            Debug.Log("Pin ID: " + pin.id);
            //create a new pin object
            GameObject newPin = new GameObject(pin.id.ToString());
          newPin.transform.SetParent(paintableCanvas.transform);

            float x = pin.position.x*20f;
            float y = pin.position.z*20f;
            float z = pin.position.y ; // not neccessary in 2D but for resaving the position
            Debug.Log("x: " + x + " y: " + y+ " z:" +z);

            newPin.transform.position = new Vector3(x, y, z);
            newPin.transform.localScale = new Vector3(1f, 1f, 1f);

            Image image = newPin.AddComponent<Image>();
            image.sprite = Image; */
        }
    }





