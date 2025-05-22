using System;
using UnityEngine;
public class SkillTory : MonoBehaviour
{
    public TempSkillSlot[] SkillSlot;
    [SerializeField] private GameObject _base;
    [SerializeField] private GameObject _gridSystem;
    private void Start()
    {
        SkillSlot = _base.GetComponentsInChildren<TempSkillSlot>();
    }
    
    
}
