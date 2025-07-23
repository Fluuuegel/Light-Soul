using UnityEngine;

public class EnemyStateController : BaseCharacterState
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private RectTransform _bgRT;
    private RectTransform _fillRT;
    private float _bgWidth;

    void Start()
    {
        _bgRT = UIHelper.Instance.FindChild(gameObject, "Background").GetComponent<RectTransform>();
        _bgWidth = _bgRT.sizeDelta.x;

        _fillRT = UIHelper.Instance.FindChild(gameObject, "Fill Area").GetComponent<RectTransform>();
        OnCharacterHPChanged += UpdateHPUI;
    }
    
    private void UpdateHPUI(float currHP, float maxHP)
    {
        float ratio = Mathf.Clamp01(currHP / maxHP);
        _fillRT.sizeDelta = new Vector2(_bgWidth * ratio, _fillRT.sizeDelta.y);
    }
}
