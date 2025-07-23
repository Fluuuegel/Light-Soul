using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    public UIManager UIManager { get; private set; }
    public SceneController SceneController { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            UIManager = new UIManager();
            SceneController = new SceneController();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        UIManager.Canvas = UIHelper.Instance.FindCanvas();
        DontDestroyOnLoad(UIManager.Canvas);

        #region StartScene

        SceneController.AddScene("StartScene", new StartScene());
        UIManager.Push(new StartPanel());

        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
