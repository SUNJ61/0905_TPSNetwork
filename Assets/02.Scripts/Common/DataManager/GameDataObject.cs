using System.Collections.Generic;
using UnityEngine;
using DataInfo;

[CreateAssetMenu(fileName = "GameDataSO", menuName ="Creat GameData",order = 1)]
public class GameDataObject : ScriptableObject//ScriptableObject는 컴퍼넌트가 아니기 때문에 MonoBehaviour가 필요없다.
{ // ??? 스크립트 대신에 사용한다.
    public int KillCount = 0;
    public float hp = 120f;
    public float damage = 25f;
    public float speed = 6.0f;
    public List<Item> equipItem = new List<Item>();
}
