using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;

public class Drop : MonoBehaviour, IDropHandler
{
    void Start()
    {

    }
    public void OnDrop(PointerEventData eventData) //������Ʈ�� ����ߴٸ�
    {
        if(transform.childCount == 0) //���� �ؿ� �ڽ� ������Ʈ�� ������
            Drag.draggingItem.transform.SetParent(transform, false); //�巡�׵� ������ ������Ʈ�� �θ� �������� �ٲ۴�.
                                                                     //�̶� ������ ������Ʈ�� ���� ��ǥ�� ����.

        Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
        //�巡�� �ǰ� �ִ� ������Ʈ�� ItemInfo�� itemData�� ���� itemData�� GameData�� ItemŬ������ ����ִ�.
        //ItemŬ���� ������ ���� item�� ������ ���� ������ ����.
        GameManager.G_instance.AddItem(item);
    }
}
