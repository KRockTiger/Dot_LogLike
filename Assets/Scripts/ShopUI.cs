using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject fKey;
    [SerializeField] private bool isShop = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            fKey.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            fKey.SetActive(false);
            if (isShop)
            {
                isShop = false;
            }
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        shopUI.gameObject.SetActive(false);
        fKey.SetActive(false);
    }

    private void Update()
    {
        if (fKey.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isShop = !isShop;

                if (isShop)
                {
                    Cursor.visible = true;
                }

                else
                {
                    Cursor.visible = false;
                }
            }
        }
        shopUI.SetActive(isShop);
    }
}
