using System.Collections.Generic;
using UnityEngine;

namespace DataInfo
{
    [System.Serializable] //아래의 클래스의 선언된 변수들을 유니티에서 보여줌
    public class GameData//MonoBehaviour을 상속하지 않으면 기능적 요소보다는 데이터적 성격이 강한 클래스이다.(Entity클래스)
    {
        public int KillCount = 0;
        public float hp = 120;
        public float damage = 25f;
        public float speed = 6.0f;
        public List<Item> equipItem = new List<Item>(); //equip = 장착된, 리스트가 아래의 아이템 클래스를 담는다.
    }
    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP, SPEED, GRANADE, DAMAGE }; //아이템 종류를 선언한 열거형 상수
        public enum ItemCalc { VALUE, PERSENT} //아이템 계산 방식, 퍼센트비율만큼 추가하는 방식과 값을 추가하는 방식

        public ItemType itemType; //아이템 종류
        public ItemCalc itemCalc; //아이템 계산 방식

        public string name; //아이템 이름
        public string desc; //아이템 소개
        public float value; //아이템 계산 값.
    }
}
