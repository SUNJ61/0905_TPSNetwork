using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDirction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Transform ItemListTr;
    private Transform ItemTr;

    private Vector2 startPosition;

    public bool isDown;

    void Start()
    {
        ItemListTr = GameObject.Find("ItemList").transform;
        ItemTr = GetComponent<Transform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = Input.mousePosition; //���콺�� ��ġ�� ����
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 endPosition = Input.mousePosition; //���콺�� ��ġ�� ����
        Vector2 direction = endPosition - startPosition; //������ ���Ϳ��� �������� ���͸� ��

        // �Ʒ������� �巡���ߴ��� �Ǻ�
        if (direction.y < 0) //�� y���� 0���� ������ �Ʒ������� �巡�� ����.
        {
            Debug.Log("�Ʒ������� �巡���߽��ϴ�.");
            isDown = true;
        }
        else //�� y���� 0���� ũ�� �������� �巡�� ����.
        {
            Debug.Log("�ٸ� �������� �巡���߽��ϴ�.");
            isDown = false;
        }
    }
}
