using UnityEngine;
using UnityEngine.UI;

public class StartPanel : BasePanel
{
    private Button _startButton;

    public StartPanel(): base(new UIPanelInfo("StartPanel", "Prefabs/UI/StartPanel"))
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        _startButton = UIHelper.Instance.GetChildComponent<Button>(
            PanelObject, "StartButton"
        );
        if (_startButton != null)
        {
            _startButton.onClick.AddListener(OnStartClicked);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(OnStartClicked);
        base.OnDisable();
    }

    private void OnStartClicked()
    {
        // GameManager.Instance.SceneController.SceneLoad(
        //     "GameScene",
        //     //"Environment_Free",
        //     new GameScene()
        // );
        
        // Asyncload
        GameManager.Instance.StartCoroutine(GameManager.Instance.SceneController.SceneLoadAsync(
            "GameScene", 
            //"Environment_Free",
            new GameScene()
        ));
    }
}
