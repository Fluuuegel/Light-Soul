using UnityEngine;

public class PlayerStateController : BaseCharacterState
{
    void Start()
    {
        var panel = UIManager.Instance.GetPanel<StatePanel>();
        panel.BindCharacter(this);
    }
}
