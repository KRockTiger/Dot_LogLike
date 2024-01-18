using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemOption
    {
        attackPostion, //���ݷ°��� ����
        healthPostion, //ü�°��� ����
        equip, //���
    }

    [Header("������ ����")]
    [SerializeField] private Image itemImage; //������ �̹���
    [SerializeField] private ItemOption itemOption; //������ ����
    [SerializeField] private string itemName; //������ �̸�
    [SerializeField] private string itemDescription; //������ ����
    [SerializeField] private int intValue; //int�� ����� ==> �ַ� ü�°��� ���
    [SerializeField] private float floatValue; //float�� ����� ==> �ַ� ���ݷ°��� ���

    /// <summary>
    /// �������� ȹ���� �� ����
    /// </summary>
    public void PGetItem()
    {
        switch (itemOption) //������ ������ �°� ���
        {
            case ItemOption.attackPostion:
                break;

            case ItemOption.healthPostion:
                break;

            case ItemOption.equip:
                break;
        }
    }
}
