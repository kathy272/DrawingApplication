using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UI_BrushPicker : MonoBehaviour
{
    [SerializeField] List<BaseBrush> Brushes= new();
    [SerializeField] GameObject BrushUIPrefab;
    [SerializeField] Transform BrushUIRoot;
    [SerializeField] UnityEvent<BaseBrush> OnBrushChanged = new();

     UI_BrushElement SelectedBrushUI;
    void Start()
    {

        //set base brush
        OnBrushChanged.Invoke(Brushes[0]);
        foreach (var brush in Brushes)
        {
            var NewBrushUIGO = Instantiate(BrushUIPrefab, BrushUIRoot);
            var NewBrushUILogic = NewBrushUIGO.GetComponent<UI_BrushElement>();
            NewBrushUILogic.BindToBrush(brush);
            NewBrushUILogic.OnBrushSelected.AddListener(OnBrushSelectedInternal);
        }

    }
    void OnBrushSelectedInternal(BaseBrush brush)
    {
    OnBrushChanged.Invoke(brush);
        Debug.Log("Brush Changed to " + brush.name);
    }

}
