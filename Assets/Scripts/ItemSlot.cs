using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Item item; //���� ������
    [SerializeField] private Image itemImage; //������ �̹���
    [SerializeField] private GameObject objHighlight; //���̶���Ʈ ������Ʈ
    [SerializeField] private GameObject objItemImage; //������ �̹��� ������Ʈ

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!objHighlight.activeSelf)
        {
            objHighlight.SetActive(true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (objHighlight.activeSelf)
        {
            objHighlight.SetActive(false);
        }
    }

    private void Update()
    {
        if (itemImage == null)
        {
            objItemImage.SetActive(false); //�������� ������� ���� ��� �̹��� ����
        }
    }
}
