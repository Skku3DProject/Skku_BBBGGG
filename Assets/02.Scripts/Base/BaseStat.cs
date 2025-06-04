using System;
using DG.Tweening;
using UnityEngine;

public class BaseStat : MonoBehaviour, IDamageAble
{
    public float BaseHP = 2000;
    [SerializeField] private float _rotationSpeed = 100f;
    private Vector3 _currentPos;
    private float _timer = 0;

    private void Start()
    {
        PingPong();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _currentPos.y = _timer * _rotationSpeed;
        transform.rotation = Quaternion.Euler(0f, _currentPos.y, 0f);
    }

    private void PingPong()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMoveY(12.5f, 1f));
        seq.Append(transform.DOMoveY(12f, 1f));
        seq.SetLoops(-1); // 무한 반복
    }
    public void TakeDamage(Damage damage)
    {
        BaseHP -= damage.Value;

        if (BaseHP <= 0)
        {
            GameManager.instance.ChangeState(GameState.GameOver);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (StageManager.instance.CurrentPhase != EPhaseType.Combat)
        { 
            UIManager.instance.UI_Interaction(InteractionType.Base);   
        }
        else
        {
            UIManager.instance.DiscriptionObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            StageManager.instance.CombatStart();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return; 
        }
        
        UIManager.instance.DiscriptionObject.SetActive(false);
    }
}
