using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper 
{
    private static UIHelper _instance;
    public static UIHelper Instance => _instance ??= new UIHelper();

    public GameObject FindCanvas()
    {
        var canvas = GameObject.FindFirstObjectByType<Canvas>()?.gameObject;
        if (canvas == null)
            Debug.LogError("Cannot find Canvas!");
        return canvas;
    }

    public GameObject FindChild(GameObject parent, string name)
    {
        foreach (var t in parent.GetComponentsInChildren<Transform>())
        {
            if (t.name == name)
            {
                // Debug.Log($"{name} finded!");
                return t.gameObject;
            }
        }

        Debug.LogWarning($"Cannot find {name} in {parent.name}");
        return null;
    }

    // Recursively search for the child
    public GameObject FindChildRecur(GameObject parent, string name)
    {
        Transform result = FindChildRecursive(parent.transform, name);
        return result?.gameObject;
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        foreach (Transform child in parent)
        {
            var result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    public T GetComponentOrWarn<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogWarning($"Cannot find {typeof(T).Name} in {go.name}");
        }
        return comp;
    }
   
    public T GetChildComponent<T>(GameObject parent, string childName) where T : Component
    {
        var child = FindChild(parent, childName);
        if (child == null)
        {
            return null;
        }

        var comp = child.GetComponent<T>();
        if (comp == null)
        {
            Debug.LogWarning($"Cannot find {typeof(T).Name} in {child.name}");
        }
        return comp;
    }
}
