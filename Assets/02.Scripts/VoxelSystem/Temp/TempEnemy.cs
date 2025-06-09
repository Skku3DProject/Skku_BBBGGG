using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class TempEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float breakDelay = 0.5f;
    public int breakDamage = 1;
    public Vector3 BasePosition;        // GameManager에서 설정하거나 인스펙터에 지정

    private CharacterController _cc;
    private float _gravity = -9.81f;
    private Vector3 _vel;

    void Awake() => _cc = GetComponent<CharacterController>();

    void Update()
    {
        if (!_cc.isGrounded) _vel.y += _gravity * Time.deltaTime;
        else if (_vel.y < 0) _vel.y = 0;

        Vector3 dir = (BasePosition - transform.position).normalized;
        Vector3 horiz = new Vector3(dir.x, 0, dir.z) * moveSpeed;


        Vector3 lowOrigin = transform.position + Vector3.up * 0.5f;
        Vector3 highOrigin = transform.position + Vector3.up * 1.5f;

        float checkDist = moveSpeed * Time.deltaTime + _cc.radius + 0.1f;

        bool hitLow = Physics.Raycast(lowOrigin, horiz.normalized, out RaycastHit lowHit, checkDist);
        bool hitHigh = Physics.Raycast(highOrigin, horiz.normalized, out RaycastHit highHit, checkDist);

        if (hitLow && hitHigh)
        {
            Vector3Int blockPos = Vector3Int.FloorToInt(lowHit.point - lowHit.normal * 0.5f);
            StartCoroutine(BreakAndContinue(blockPos));
        }
        else
        {
            _cc.Move((horiz + _vel) * Time.deltaTime);
        }
    }

    private IEnumerator BreakAndContinue(Vector3Int blockPos)
    {
        yield return new WaitForSeconds(breakDelay);
        BlockManager.DamageBlock(blockPos, breakDamage);
    }
}
