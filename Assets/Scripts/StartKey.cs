using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartKey : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField] private GameObject fKey;
    [SerializeField] private bool bossKey;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        fKey.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        fKey.SetActive(false);
    }

    private void Start()
    {
        fKey.SetActive(false);
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (fKey.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F) && !bossKey)
            {
                gameManager.PSetStartGame(true);
                gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.F) && bossKey)
            {
                gameManager.PSetBoss();
                gameObject.SetActive(false);
            }
        }
    }
}
