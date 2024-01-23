using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Player player;
    [SerializeField] private Item item; //���� ������
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private GameObject objHighlight; //���̶���Ʈ ������Ʈ
    [SerializeField] private GameObject objItemImage; //������ �̹��� ������Ʈ

    public void OnPointerEnter(PointerEventData eventData)
    {
        objHighlight.SetActive(true);

        if (item != null)
        {
            itemDescription.gameObject.SetActive(true);
            itemDescription.text = $"{item.itemName}\n{item.itemDescription}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        objHighlight.SetActive(false);

        if (item != null)
        {
            itemDescription.gameObject.SetActive(false);
            itemDescription.text = string.Empty;
        }
    }

    private void Start()
    {
        itemDescription.gameObject.SetActive(false);
        itemDescription.text = string.Empty;
    }

    private void Update()
    {
        if (item == null)
        {
            objItemImage.SetActive(false); //�������� ������� ���� ��� �̹��� ����
        }

        else if (item != null && player != null)
        {
            UsingItem();
        }
    }

    /// <summary>
    /// �������� �����ϰ� ���� ��� ����
    /// </summary>
    private void UsingItem()
    {
        objItemImage.SetActive(true);

        switch (item.valueType)
        {
            case Item.ValueType.damage:
                player.PGetDamage(item.PGetFloatValue());
                break;

            case Item.ValueType.move:
                player.PGetMove(item.PGetFloatValue());
                break;

            case Item.ValueType.maxHP:
                player.PGetMaxHP(item.PGetIntValue());
                break;
        }
    }

    /// <summary>
    /// ������ ���� Ȯ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public Item PCheckItem()
    {
        return item;
    }

    /// <summary>
    /// ������ ���� ��������
    /// </summary>
    /// <param name="_item"></param>
    public void PAddItem(Item _item)
    {
        Image itemImg = objItemImage.GetComponentInChildren<Image>(); //�ڽ� ������Ʈ�� �̹��� ���۳�Ʈ ��������
        item = _item; //������ ���
        itemImg.sprite = item.itemSprite; //������ �̹��� ���
    }
}
