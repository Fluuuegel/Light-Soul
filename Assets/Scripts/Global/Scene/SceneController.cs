using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using UniRx;

public class SceneController
{
    private static SceneController _instance;
    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Uninitialized SceneController!");
            return _instance;
        }
    }

    private Dictionary<string, SceneBase> _scenes = new();
    private List<string> _history = new();
    public SceneController()
    {
        _instance = this;
    }

    public void AddScene(string sceneName, SceneBase scene)
    {
        _scenes.Add(sceneName, scene);
        _history.Add(sceneName);
    }

    public void SceneLoad(string sceneName, SceneBase scene)
    {
        if (_history.Contains(sceneName))
        {
            Debug.Log($"{sceneName} has been loaded!");
        }
        else
        {
            _history.Add(sceneName);
        }

        if (!_scenes.ContainsKey(sceneName))
            _scenes.Add(sceneName, scene);


        var current = SceneManager.GetActiveScene().name;
        if (_scenes.TryGetValue(current, out var oldScene))
            oldScene.ExitScene();

        GameManager.Instance.UIManager.Pop(isLoad: true);

        SceneManager.LoadScene(sceneName);

        scene.EnterScene();
    }

    public IEnumerator SceneLoadAsync(string sceneName, SceneBase scene)
    {
        if (_history.Contains(sceneName))
        {
            Debug.Log($"{sceneName} has been loaded!");
        }
        else
        {
            _history.Add(sceneName);
        }

        if (!_scenes.ContainsKey(sceneName))
            _scenes.Add(sceneName, scene);

        var current = SceneManager.GetActiveScene().name;
        if (_scenes.TryGetValue(current, out var oldScene))
            oldScene.ExitScene();

        var loadingPanel = new LoadingPanel();
        UIManager.Instance.Push(loadingPanel);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingPanel._loadingSlider.value = operation.progress;
            if (operation.progress >= 0.9f)
            {
                GameManager.Instance.UIManager.Pop(isLoad: true);
                scene.EnterScene();
                loadingPanel._loadingSlider.value = 1;
                break;
            }
            yield return null;
        }

        yield return null;
        operation.allowSceneActivation = true;
    }
}
