using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class UI_BrushElement : MonoBehaviour
{

    [SerializeField] RawImage brushImage;
    [SerializeField] Text brushName;
    [SerializeField] Image background;

    public UnityEvent<BaseBrush> OnBrushSelected = new();

    private readonly Color defaultColor = new Color(0f, 0f, 0f, 0.39f);
    private readonly Color selectedColor = Color.grey; 

    BaseBrush LinkedBrush;
    private static UI_BrushElement currentSelected;

    public void BindToBrush(BaseBrush InBrush)
    {

        LinkedBrush = InBrush;
        brushImage.texture = InBrush.BrushTexture;
        brushName.text = InBrush.DisplayName;
    }

    public void OnBrushElementClick(BaseEventData InEventData)
    {
        if (InEventData is PointerEventData)
        {
            OnBrushSelected.Invoke(LinkedBrush);
            SetAsSelected();

        }
    }

    public void SetAsSelected()
    {
        if (currentSelected != null && currentSelected != this)
        {
            currentSelected.SetSelected(false);
        }

        currentSelected = this;
        SetSelected(true);
    }
    public void SetSelected(bool isSelected)
    {
        if (background != null)
        {
            background.color = isSelected ? selectedColor : defaultColor;
        }
    }

    public void SelectOnStart()
    {
        
        SetAsSelected();
        OnBrushSelected.Invoke(LinkedBrush); 
    }

}
