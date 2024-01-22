using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        attackPostion, //���ݷ°��� ����
        healthPostion, //ü�°��� ����
        artifact, //���
    }

    [Header("������ ����")]
    [SerializeField] private SpriteRenderer itemSprite; //������ ��������Ʈ
    [SerializeField] private ItemType itemType; //������ ����
    [SerializeField] private string itemName; //������ �̸�
    [SerializeField] private string itemDescription; //������ ����
    [SerializeField] private int intValue; //int�� ����� ==> �ַ� ü�°��� ���
    [SerializeField] private float floatValue; //float�� ����� ==> �ַ� ���ݷ°��� ���

    /// <summary>
    /// ������ ���� ����
    /// </summary>
    public ItemType PGetItemType()
    {
       return itemType;
    }

    /// <summary>
    /// int�� �������� public �Լ�
    /// </summary>
    /// <returns></returns>
    public int PGetIntValue()
    {
        return intValue;
    }

    /// <summary>
    /// float�� �������� public �Լ�
    /// </summary>
    /// <returns></returns>
    public float PGetFloatValue()
    {
        return floatValue;
    }
}
