public class GameScene : SceneBase
{
    public override void EnterScene()
    {
        InputManager.Instance.ResetTriggers();
        UIManager.Instance.Push(new StatePanel());
    }
    
    public override void ExitScene()
    {
        
    }
}