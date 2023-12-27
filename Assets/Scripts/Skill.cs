using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("스킬 옵션")]
    [SerializeField] private float moveSpeed = 3f;

    private bool isStop = false;
    private Vector3 direction;

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (isStop)
        { return; }
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    public void PSetDirection(Vector3 _direction)
    {
        direction = _direction;
    }

    public Vector3 PGetDirection()
    {
        return direction;
    }

    public void BStopMoving()
    {
        isStop = true;
    }

    public void BDestroying()
    {
        Destroy(gameObject);
    }
}
