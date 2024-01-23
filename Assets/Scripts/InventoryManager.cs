using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //싱글톤으로 사용
    public static InventoryManager Instance;

    [SerializeField] private GameObject objInventory;
    [SerializeField] private GameObject objItem;
    [SerializeField] KeyCode openInvenkey;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        OpenInventory();
    }

    private void OpenInventory()
    {
        if (Input.GetKeyDown(openInvenkey))
        {
            if (objInventory.activeSelf) //열려있다면
            {
                objInventory.SetActive(false); //닫기
                Cursor.visible = false;
            }

            else //닫혀있다면
            {
                objInventory.SetActive(true); //열기
                Cursor.visible = true;
            }

            //요약 버전 => objInventory.SetActive(!objInventory.activeSelf);
        }
    }
}
