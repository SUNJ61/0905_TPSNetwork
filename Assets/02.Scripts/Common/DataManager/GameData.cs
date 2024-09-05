using System.Collections.Generic;
using UnityEngine;

namespace DataInfo
{
    [System.Serializable] //�Ʒ��� Ŭ������ ����� �������� ����Ƽ���� ������
    public class GameData//MonoBehaviour�� ������� ������ ����� ��Һ��ٴ� �������� ������ ���� Ŭ�����̴�.(EntityŬ����)
    {
        public int KillCount = 0;
        public float hp = 120;
        public float damage = 25f;
        public float speed = 6.0f;
        public List<Item> equipItem = new List<Item>(); //equip = ������, ����Ʈ�� �Ʒ��� ������ Ŭ������ ��´�.
    }
    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP, SPEED, GRANADE, DAMAGE }; //������ ������ ������ ������ ���
        public enum ItemCalc { VALUE, PERSENT} //������ ��� ���, �ۼ�Ʈ������ŭ �߰��ϴ� ��İ� ���� �߰��ϴ� ���

        public ItemType itemType; //������ ����
        public ItemCalc itemCalc; //������ ��� ���

        public string name; //������ �̸�
        public string desc; //������ �Ұ�
        public float value; //������ ��� ��.
    }
}
