using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public Slider _loadingSlider;

    public LoadingPanel(): base(new UIPanelInfo("LoadingPanel", "Prefabs/UI/LoadingPanel"))
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        _loadingSlider = UIHelper.Instance.GetChildComponent<Slider>(PanelObject, "LoadingSlider");

    }

    public override void OnEnable()
    {
        base.OnEnable();
        PanelObject.SetActive(true);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PanelObject.SetActive(false);
    }
}
