using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainScript : MonoBehaviour 
{
    public float XMargin;
    public float ZMargin;

    public Material Mat;

    public bool RowSelect;
    [Range(1, 40)]
    public int SelectedRow;
    [Range(0, 1)]
    public float SurvivalMode;

    public TextMeshPro SelectedLabel;
    public bool SelectionLocked;

    public Text ControlsText;

    private ItemBehavior _selectedItem;
    public ItemBehavior SelectedItem
    {
        get{ return _selectedItem; }
        set
        {
            if(_selectedItem != value)
            {
                if(_selectedItem != null)
                {
                    _selectedItem.SetIsSelected(false);
                }
                if(value != null)
                {
                    Shader.SetGlobalInt("_RowSelected", value.Data.FirstTargetHealth);
                    Shader.SetGlobalInt("_ColumnSelected",value.Data.SecondTargetHealth);
                    value.SetIsSelected(true);
                }
                else
                {
                    Shader.SetGlobalInt("_RowSelected", -1);
                    Shader.SetGlobalInt("_ColumnSelected", -1);
                }
                _selectedItem = value;
            }
        }
    }

    public Color ColorA;
    public Color ColorB;
    public Color ColorC;

    private DuelResolve[] _tables;
    private ItemBehavior[,] _itemBehaviors;

    private Vector3 _clickPos;

    void Start ()
    {
        DuelOutcome outcomeA = DuelResolve.GetOutcome(40, 5);
        DuelOutcome outcomeB = DuelResolve.GetOutcome(5, 40);
        _tables = DuelResolve.GetResolvesTable();
        _itemBehaviors = MakeBoxes();
        MakeLabels();
        Material labelMaterial = new Material(SelectedLabel.fontMaterial);
        labelMaterial.SetInt("unity_GUIZTestMode", 0);
        SelectedLabel.fontMaterial = labelMaterial;
        labelMaterial.renderQueue = 5001;
    }

    private string GetControlText()
    {
        string lockText = SelectionLocked ? "Unlock:\tClick" : "Lock:\t\tClick";
        return "Rotate:\tLeft Mouse\nPan:\t\tMiddle Mouse\nZoom:\t\tRight Mouse\n" + lockText;
    }

    private void HandleToggleLock()
    {
        if (!SelectionLocked)
        {
            SelectedItem = GetSelectedItem();
        }
        if (Input.GetMouseButtonDown(0))
        {
            _clickPos = Input.mousePosition;
        }
        if(Input.GetMouseButtonUp(0))
        {
            float distance = (Input.mousePosition - _clickPos).magnitude;
            if(distance < 0.1f)
            {
                SelectionLocked = SelectedItem != null ? !SelectionLocked : false;
                ControlsText.text = GetControlText();
            }
        }
    }

    private void Update()
    {
        Shader.SetGlobalFloat("_SurvivalMode", SurvivalMode);
        Shader.SetGlobalFloat("_Locked", SelectionLocked ? 1 : 0);
        HandleToggleLock();
        HandleSelectedLabel();
    }
    

    private void HandleSelectedLabel()
    {
        bool somethingSelected = SelectedItem != null;
        SelectedLabel.gameObject.SetActive(somethingSelected);
        if(somethingSelected)
        {
            Transform labelTransform = SelectedLabel.transform.parent;
            SelectedLabel.text = GetLabelText(SelectedItem.Data);
            Vector3 lookatTarget = new Vector3(Camera.main.transform.position.x,
                labelTransform.position.y, 
                Camera.main.transform.position.z);
            Vector3 positionTarget = new Vector3(SelectedItem.transform.position.x,
                SelectedItem.transform.localScale.y,
                SelectedItem.transform.position.z);
            labelTransform.position = Vector3.Lerp(labelTransform.position, positionTarget, .1f);
            labelTransform.LookAt(lookatTarget);
        }
    }

    private static string GetLabelText(DuelOutcome data)
    {
        string colorCode = "FFB932";
        string lineAComponent = data.FirstSurvives ? (" -> " + data.RemainingHealth) : " (killed)";
        string lineBComponent = data.FirstSurvives ? " (killed)" : (" -> " + data.RemainingHealth);
        string lineA = string.Format("First Target:\t\t<color=#{0}>{1}</color>{2}", colorCode, data.SecondTargetHealth, lineAComponent);
        string lineB = string.Format("Second Target:\t<color=#{0}>{1}</color>{2}", colorCode, data.FirstTargetHealth, lineBComponent);
        string lineC = string.Format("Damage:\t\t<color=#{0}>{1}</color>", colorCode, data.Damage);
        return string.Format("{0}\n{1}\n{2}", lineA, lineB, lineC);
    }

    ItemBehavior GetSelectedItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            ItemBehavior behavior = hitInfo.transform.gameObject.GetComponent<ItemBehavior>();
            if (behavior != null)
            {
                return behavior;
            }
        }
        return null;
    }

    private ItemBehavior[,] MakeBoxes()
    {
        ItemBehavior[,] ret = new ItemBehavior[DuelResolve.Range, DuelResolve.Range];
        for (int i = 0; i < DuelResolve.Range; i++)
        {
            DuelResolve row = _tables[i];
            for (int j = 0; j < DuelResolve.Range; j++)
            {
                DuelOutcome item = row.Outcomes[j];
                ItemBehavior behavior = CreateItem(item);
                ret[i, j] = behavior;
            }
        }
        return ret;
    }

    private void MakeLabels()
    {
        GameObject labelsParent = new GameObject("labels");
        labelsParent.transform.parent = transform;
        CreateLabels(0, "1", labelsParent.transform);
        for (int i = 10; i < DuelResolve.Range + 1; i+=10)
        {
            CreateLabels(i, i.ToString(), labelsParent.transform);
        }
    }

    private void CreateLabels(int index, string labelText, Transform labelsParent)
    { 
        GameObject columnLabel = CreateLabel("column " + index, labelText);
        columnLabel.transform.position = new Vector3(index + .5f, .05f, -2.25f);
        columnLabel.transform.localScale = new Vector3(.25f, .25f, .25f);
        columnLabel.transform.rotation = Quaternion.Euler(90, 0, 0);
        columnLabel.transform.parent = labelsParent;

        GameObject rowLabel = CreateLabel("row " + index, labelText);
        rowLabel.transform.position = new Vector3(-1.25f, .05f, DuelResolve.Range - index - .5f);
        rowLabel.transform.localScale = new Vector3(.25f, .25f, .25f);
        rowLabel.transform.rotation = Quaternion.Euler(90, 90, 0);
        rowLabel.transform.parent = labelsParent;
    }

    private GameObject CreateLabel(string labelName, string labelText)
    {
        GameObject ret = new GameObject(labelName);
        TextMeshPro textMesh = ret.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Midline;
        textMesh.enableWordWrapping = false;
        textMesh.text = labelText;
        textMesh.color = Color.white;
        textMesh.fontSize = 32;
        MeshRenderer renderer = ret.GetComponent<MeshRenderer>();
        renderer.sharedMaterial.renderQueue = 2001;
        return ret;
    }

    private ItemBehavior CreateItem(DuelOutcome item)
    {
        Material newMat = new Material(Mat);
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = item.SecondTargetHealth + " duels " + item.FirstTargetHealth + " doing " + item.Damage + " damage";
        obj.transform.parent = transform;
        obj.GetComponent<MeshRenderer>().sharedMaterial = newMat;
        ItemBehavior behavior = obj.AddComponent<ItemBehavior>();
        behavior.Data = item;
        behavior.Mat = newMat;
        behavior.Main = this;
        return behavior;
    }
}
