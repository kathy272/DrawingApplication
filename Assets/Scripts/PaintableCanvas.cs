using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Klak.Ndi;

public class PaintableCanvas : MonoBehaviour
{
    enum EPaintingMode
    {
        Off,
        Draw
    }

    [SerializeField] float RaycastDistance = 10.0f;
    [SerializeField] LayerMask PaintableCanvasLayerMask = ~0;
    [SerializeField] MeshFilter CanvasMeshFilter;
    [SerializeField] MeshRenderer CanvasMeshRenderer;
    [SerializeField] RectTransform CanvasRectTransform;
    [SerializeField] int PixelsPerMetre = 200;
    [SerializeField] Color CanvasDefaultColour = Color.white;
    [SerializeField] float BrushScale = 0.25f;
    [SerializeField] float BrushWeight = 0.25f; //intensity of the brush


    [SerializeField] Slider BrushSizeSlider;
    [SerializeField] Slider BrushWeightSlider;

    [SerializeField] NdiSender ndiSender;
    [SerializeField] Texture2D BaseImage;

    EPaintingMode PaintingMode_PrimaryMouse = EPaintingMode.Draw; // Left mouse button

    int CanvasWidthInPixels;
    int CanvasHeightInPixels;

    Texture2D PaintableTexture;
    RenderTexture NDIRenderTexture;
    BaseBrush ActiveBrush;
    Color ActiveColour = Color.magenta;
    void Start()
    {

        float desiredWidth = 10f;  // Set the desired width of the canvas in world units
        float desiredHeight = 10f; // Set the desired height of the canvas in world units
        transform.localScale = new Vector3(desiredWidth, desiredHeight, 1f);
        CanvasWidthInPixels = Mathf.CeilToInt(desiredWidth * PixelsPerMetre);
        CanvasHeightInPixels = Mathf.CeilToInt(desiredHeight * PixelsPerMetre);

        PaintableTexture = new Texture2D(CanvasWidthInPixels, CanvasHeightInPixels, TextureFormat.ARGB32, false);

        NDIRenderTexture = new RenderTexture(CanvasWidthInPixels, CanvasHeightInPixels, 0);


        if (BaseImage != null)
        {
            Texture2D scaledBaseImage = ScaleTexture(BaseImage, CanvasWidthInPixels, CanvasHeightInPixels);


            for (int Y = 0; Y < CanvasHeightInPixels; Y++)
            {
                for (int X = 0; X < CanvasWidthInPixels; X++)
                {
                    PaintableTexture.SetPixel(X, Y, scaledBaseImage.GetPixel(X,Y));
                }
            }
        }
        else
        {
            for (int Y = 0; Y < CanvasHeightInPixels; Y++)
            {
                for (int X = 0; X < CanvasWidthInPixels; X++)
                {
                    PaintableTexture.SetPixel(X, Y, CanvasDefaultColour);
                }
            }
        }
  
        PaintableTexture.Apply();
        CanvasMeshRenderer.material.mainTexture = PaintableTexture;

        ndiSender.sourceTexture = NDIRenderTexture;

        BrushSizeSlider.value = BrushScale;
        BrushWeightSlider.value = BrushWeight;

        BrushSizeSlider.onValueChanged.AddListener(OnBrushSizeChanged);
        BrushWeightSlider.onValueChanged.AddListener(OnBrushWeightChanged);

    }

    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(source, rt);
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt); // added to avoid errors
        return result;
    }

    void OnBrushSizeChanged(float NewBrushSize)
    {
        BrushScale = NewBrushSize;
    }
 void OnBrushWeightChanged(float NewBrushWeight)
    {
        BrushWeight = NewBrushWeight;
    }
    // Update is called once per frame
    void Update()
    {
        if (ActiveBrush != null)
        {
            if (PaintingMode_PrimaryMouse == EPaintingMode.Draw && Input.GetMouseButton(0))
            {
                Update_PerformDrawing(PaintingMode_PrimaryMouse);
            }
        }
        Graphics.Blit(PaintableTexture, NDIRenderTexture); // Blit the texture to the NDI sender
    }

    RaycastHit[] HitResults = new RaycastHit[1];
    void Update_PerformDrawing(EPaintingMode PaintingMode)
    {
        Ray DrawingRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.RaycastNonAlloc(DrawingRay, HitResults, RaycastDistance, PaintableCanvasLayerMask) > 0)
        {
            Debug.Log(ActiveBrush.name);
            PerformDrawingWith(ActiveBrush, ActiveColour, HitResults[0].textureCoord);
        }
    }

    void PerformDrawingWith(BaseBrush ActiveBrush, Color ActiveColour, Vector2 LocationUV)
    {
        int DrawingOriginX = Mathf.RoundToInt(LocationUV.x * CanvasWidthInPixels);
        int DrawingOriginY = Mathf.RoundToInt(LocationUV.y * CanvasHeightInPixels);

        int ScaledBrushWidth = Mathf.RoundToInt(ActiveBrush.BrushTexture.width * BrushScale);
        int ScaledBrushHeight = Mathf.RoundToInt(ActiveBrush.BrushTexture.height * BrushScale);

        for (int BrushY = 0; BrushY < ScaledBrushHeight; BrushY++)
        {
            int PixelY = DrawingOriginY + BrushY - (ScaledBrushHeight / 2);
            if (PixelY < 0 || PixelY >= CanvasHeightInPixels)
                continue;

            float BrushUV_Y = (float)BrushY / (float)ScaledBrushHeight;

            for (int BrushX = 0; BrushX < ScaledBrushWidth; BrushX++)
            {
                int PixelX = DrawingOriginX + BrushX - (ScaledBrushWidth / 2);
                if (PixelX < 0 || PixelX >= CanvasWidthInPixels)
                    continue;

                float BrushUV_X = (float)BrushX / (float)ScaledBrushWidth;
                Color BrushPixel = ActiveBrush.BrushTexture.GetPixelBilinear(BrushUV_X, BrushUV_Y);
                Color CanvasPixel = PaintableTexture.GetPixel(PixelX, PixelY);

                CanvasPixel = ActiveBrush.Apply(CanvasPixel, BrushPixel, ActiveColour, BrushWeight * Time.deltaTime);
                PaintableTexture.SetPixel(PixelX, PixelY, CanvasPixel);
            }
        }
        PaintableTexture.Apply();
    }

    public void SelectBrush(BaseBrush InBrush)
    {
        ActiveBrush = InBrush;
    }

    public void SetColour(Color InColour)
    {
        ActiveColour = InColour;
    }
}