using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //�̱������� ���
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
            if (objInventory.activeSelf) //�����ִٸ�
            {
                objInventory.SetActive(false); //�ݱ�
                Cursor.visible = false;
            }

            else //�����ִٸ�
            {
                objInventory.SetActive(true); //����
                Cursor.visible = true;
            }

            //��� ���� => objInventory.SetActive(!objInventory.activeSelf);
        }
    }
}
