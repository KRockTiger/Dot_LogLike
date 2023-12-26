using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [Header("스킬 옵션")]
    [SerializeField] private float moveSpeed = 3f;

    private bool isStop = false;

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        if (isStop)
        { return; }
        transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;
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
