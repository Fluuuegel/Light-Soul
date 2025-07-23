using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StatePanel : BasePanel
{
    private RectTransform _bgRT;
    private RectTransform _fillRT;
    private float _bgWidth;
    private PlayerStateController _playerState;

    public StatePanel() : base(new UIPanelInfo("StatePanel", "Prefabs/UI/StatePanel"))
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        _bgRT = UIHelper.Instance.FindChild(PanelObject, "Background").GetComponent<RectTransform>();
        _bgWidth = _bgRT.sizeDelta.x;

        _fillRT = UIHelper.Instance.FindChild(PanelObject, "Fill Area").GetComponent<RectTransform>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (_playerState != null)
        {
            _playerState.OnCharacterHPChanged -= UpdateHPUI;
        }
    }

    private void UpdateHPUI(float currHP, float maxHP)
    {
        float ratio = Mathf.Clamp01(currHP / maxHP);
        _fillRT.sizeDelta = new Vector2(_bgWidth * ratio, _fillRT.sizeDelta.y);
    }
    
    public void BindCharacter(PlayerStateController state)
    {
        _playerState = state;
        _playerState.OnCharacterHPChanged += UpdateHPUI;
    }

}