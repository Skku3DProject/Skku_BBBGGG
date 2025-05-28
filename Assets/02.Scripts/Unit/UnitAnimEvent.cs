using UnityEngine;

public class UnitAnimEvent : MonoBehaviour
{
    private Unit _unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    private void OnAnimTrigger() => _unit.AnimTrigger();
    private void OnAttackAnimStart() => _unit.StartAttackAnim();
    private void OnAttackAnimEnd() => _unit.EndAttackAnim();
}
