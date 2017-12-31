using UnityEngine;
public class ItemBehavior : MonoBehaviour
{
    public DuelOutcome Data;
    public Material Mat;
    public MainScript Main;
    private MeshRenderer meshRenderer;
    
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        SetMaterialProperties();
    }

    private void Update()
    {
        Vector3 pos = new Vector3(Data.FirstTargetHealth * Main.XMargin,
            (float)(Data.Damage) / 2,
            (DuelResolve.Range - Data.SecondTargetHealth) * Main.ZMargin);
        Vector3 scale = new Vector3(1, Data.Damage, 1);
        transform.localPosition = pos;
        transform.localScale = scale;
    }

    public void SetIsSelected(bool value)
    {
        Mat.SetFloat("_Selected", value ? 1 : 0);
    }

    // Note: Drops the framerate from 80 to 40 when done at runtime. 
    // Put this in Update() while iterating on shaders, then move it to Start() when done.
    private void SetMaterialProperties()
    {
        bool boring = Data.FirstTargetHealth > Data.SecondTargetHealth && (Data.SecondTargetHealth * 2) > Data.FirstTargetHealth;
        bool rowSelected = Data.FirstTargetHealth == Main.SelectedRow || !Main.RowSelect;
        meshRenderer.enabled = rowSelected;
        Mat.SetFloat("_Boring", boring ? 1 : 0);
        Mat.SetFloat("_FirstSurvives", Data.FirstSurvives ? 1 : 0);
        Mat.SetColor("_ColorA", Main.ColorA);
        Mat.SetColor("_ColorB", Main.ColorB);
        Mat.SetColor("_ColorC", Main.ColorC);
        Mat.SetFloat("_XGrid", (float)Data.FirstTargetHealth / 40);
        Mat.SetFloat("_ZGrid", (float)Data.SecondTargetHealth / 40);
        Mat.SetFloat("_Height", (float)Data.Damage / 60);
        Mat.SetFloat("_BaseHeight", (float)Data.FirstTargetHealth / 60);
        Mat.SetFloat("_Ratio", (float)Data.Damage / (Data.FirstTargetHealth + Data.SecondTargetHealth));
        Mat.SetInt("_Row", Data.FirstTargetHealth);
        Mat.SetInt("_Column", Data.SecondTargetHealth);
    }
}
