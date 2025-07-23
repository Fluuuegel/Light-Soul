using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the scene UI object push to stack or pop out from stack
/// Control the dict info for manager
/// </summary>
[Serializable]
public class UIManager
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Uninitialized UIManager!");
            return _instance;
        }
    }

    public Dictionary<string, GameObject> UIObjects { get; } = new();

    public Stack<BasePanel> UIPanelStack { get; set; } = new();

    public GameObject Canvas;

    public UIManager() 
    {
        _instance = this;
    }

    public T GetPanel<T>() where T : BasePanel
    {
        foreach (var panel in UIPanelStack)
        {
            if (panel is T targetPanel)
                return targetPanel;
        }

        Debug.LogWarning($"Panel of type {typeof(T).Name} not found in stack.");
        return null;
    }

    public GameObject GetOrCreate(UIPanelInfo uiPanelInfo)
    {
        if (UIObjects.TryGetValue(uiPanelInfo.Name, out var go))
        {
            return go;
        }

        if (Canvas == null)
        {
            Canvas = UIHelper.Instance.FindCanvas();
        }

        var prefab = Resources.Load<GameObject>(uiPanelInfo.Path);

        if (prefab == null) return null;

        go = GameObject.Instantiate(prefab, Canvas.transform);
        UIObjects.Add(uiPanelInfo.Name, go);
        return go;
    }

    public void Push(BasePanel panel) 
    {
        if (UIPanelStack.Count > 0)
            UIPanelStack.Peek().OnDisable();

        var go = GetOrCreate(panel.UiPanelInfo);
        panel.PanelObject = go;
        if (UIPanelStack.Count == 0 || UIPanelStack.Peek().UiPanelInfo.Name != panel.UiPanelInfo.Name)
            UIPanelStack.Push(panel);

        panel.OnStart();

    }

    public void Pop(bool isLoad)
    {
        if (UIPanelStack.Count == 0) return;

        // Pop everything when loading
        if (isLoad)
        {
            while (UIPanelStack.Count > 0)
            {
                Pop(false);
            }
            return;
        }

        // Normal pop
        var top = UIPanelStack.Pop();
        top.OnDisable();
        top.OnDestory();
        if (UIObjects.TryGetValue(top.UiPanelInfo.Name, out var go))
        {
            GameObject.Destroy(go);
            UIObjects.Remove(top.UiPanelInfo.Name);
        }

        if (UIPanelStack.Count > 0)
            UIPanelStack.Peek().OnEnable();
    }
}
