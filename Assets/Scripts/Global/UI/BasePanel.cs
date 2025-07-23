using UnityEngine;

public abstract class BasePanel
{
    public UIPanelInfo UiPanelInfo;
    public GameObject PanelObject;
    protected CanvasGroup CanvasGroup { get; private set; }

    public BasePanel(UIPanelInfo uiPanelInfo)
    {
        UiPanelInfo = uiPanelInfo;
    }

    public virtual void OnStart() 
    {
        if (PanelObject == null)
        {
            Debug.LogError($"{nameof(PanelObject)} not exists!");
            return;
        }

        CanvasGroup = UIHelper.Instance.GetComponentOrWarn<CanvasGroup>(PanelObject);

        if (CanvasGroup == null)
        {
            CanvasGroup = PanelObject.AddComponent<CanvasGroup>();
        }

        CanvasGroup.interactable = true;
    }

    public virtual void OnEnable() 
    {
        CanvasGroup.interactable = true;
    }

    public virtual void OnDisable() 
    {
        CanvasGroup.interactable = false;
    }

    public virtual void OnDestory() 
    {
        CanvasGroup.interactable = false;
    }

}
