using UnityEngine;

public class AutoReturnObject : MonoBehaviour
{
    public string key;
    public float Timer = 2f;
    private float _timer = 0;

    private void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= Timer)
        {
            EnemyObjectPoolManger.Instance.ReturnObject(key, gameObject);
            _timer = 0;
        }
    }
}
