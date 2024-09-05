using DataInfo;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
public class GameManager : MonoBehaviourPun
{
    public static GameManager G_instance;
    public bool isGameOver = false;
    public bool isInvenopen = false;

    private Text KillTXT;

    private string CanvasObj = "Canvas_UI";
    private string playertag = "Player";
    private string panel_WPname = "Panel_Weapon";
    //private string KillKey = "KILLCOUNT";
    //private int KillCount = 0;

    private bool isPaused = false;

    private DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData; //만든 ScriptableObject 끌어서 넣기

    public delegate void ItemChangeDelegate(); //인벤토리 아이템이 변경되었을 때 발생 시킬 이벤트 정의.
    public static event ItemChangeDelegate OnItemChange;
    [SerializeField] private GameObject slotList; //슬롯안에 아이템이 있는지 검사하기위해 가져온다.
    public GameObject[] itemObjects;
    void Awake()
    {
        if (G_instance == null)
            G_instance = this;
        else if(G_instance != this)
            Destroy(G_instance);
        DontDestroyOnLoad(G_instance);

        StartCoroutine(CreatePlayer());

        KillTXT = GameObject.Find(CanvasObj).transform.GetChild(7).GetComponent<Text>();

        dataManager = GetComponent<DataManager>();
        dataManager.Initialized();

        LoadGameData();
    }
    
    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(1.0f);
        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
    }

    public void OnPauseClick()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0.0f : 1.0f; //isPaused가 true면 0, false면 1이 된다. 
        var playerObj = GameObject.FindGameObjectWithTag(playertag); //플레이어 오브젝트를 찾아 저장
        var scripts = playerObj.GetComponents<MonoBehaviour>(); //플레이어 오브젝트에 있는 모든 스크립트를 저장한다.
        //MonoBehaviour라는 클래스를 상속 받았기 때문에 오브젝트에 스크립트를 달 수 있다. 즉, MonoBehaviour는 스크립트를 다는 기능.
        foreach( var script in scripts )
        {
            script.enabled = !isPaused; //퍼즈를 하면 스크립트 비활성화, 퍼즈를 풀면 스크립트 활성화. 
        }
        var canvasGroup = GameObject.Find(panel_WPname).GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    private void LoadGameData()
    {
        #region PlayerPrefs를 이용한 데이터 저장
        //KillCount = PlayerPrefs.GetInt(KillKey, 0); //해당기능은 DataManager로 대체
        //플레이어 프리퍼런스의 약자. 해당 클래스는 "KILLCOUNT"라는 키가 없으면 생성하고 0을 대입.
        //"KILLCOUNT"라는 키가 이미 잇으면 저장된 데이터를 불러온다.
        #endregion

        #region 물리적으로 데이터 저장
        //GameData data = dataManager.Load(); //시작되면 이전의 플레이 했던 게임 정보를 가져온다.
        //gameData.hp = data.hp; //불러온 데이터를 현재 게임오브젝트의 게임 데이터에 덮어씌운다.
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.KillCount = data.KillCount; //해당 기능은 ScriptableObject로 대체
        //gameData.equipItem = data.equipItem; //ScriptableObject는 전역적으로 접근할 수 있어 별도로 로드하는 과정이 필요없음.
        #endregion

        if (gameData.equipItem.Count > 0) //아이템을 장착한 것이 있으면 호출된다.
            InventorySetUp();

        KillTXT.text = "<color=#FFFFFF>KILL</color> : " + gameData.KillCount.ToString("0000"); //숫자를 4번째 자리까지 표현
    }
    void InventorySetUp()//시작 됬을 때 인벤토리에 아이템을 넣을 용도.
    {
        var slots = slotList.GetComponentsInChildren<Transform>(); //슬롯리스트 자식오브젝트 전부를 담는다.
        for (int i = 0; i < gameData.equipItem.Count; i++) //보유한 아이템만큼 돌아간다.
        {
            for(int j = 1; j < slots.Length; j++) //slots 0번 인덱스에는 부모오브젝트가 들어가 있기 때문에 j는 1부터 시작.
            {
                if (slots[j].childCount > 0) continue; //슬롯오브젝트에 아이템오브젝트가 자식으로 있으면 다음 인덱스로 증가.
                int itemIndex = (int)gameData.equipItem[i].itemType; //인벤토리에 넣을 아이템의 종류에 따라 인덱스를 추출.
                //equipItem[i]에 해당하는 아이템 열거형의 인덱스를 itemIndex에 저장한다.
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j].transform);//해당 아이템의 부모는 슬롯이 된다.
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                //아이템의 ItemInfo클래스의 itemData변수에 로드한 gameData.equipItem[i] 값을 저장. (둘다 Item클래스의 변수형태)
                break; //내부 for을 탈출해 외부 for문의 인덱스를 하나 증가시킨다.
            }
        }
    }
    void SaveGameData() //현재까지의 게임 정보 저장
    {
        //dataManager.Save(gameData); //ScriptableObject로 대체
#if UNITY_EDITOR //에디터상에서만 실행.
        UnityEditor.EditorUtility.SetDirty(gameData); //유니티 에디터상에서 일어난 일을 .asset에 생성한 파일에 데이터 저장
#endif
    }
    public void KillScore()
    {
        //++KillCount; //게임 매니저로 대체
        ++gameData.KillCount;
        KillTXT.text = "<color=#FFFFFF>KILL</color> : " + gameData.KillCount.ToString("0000");
        //PlayerPrefs.SetInt(KillKey, KillCount); //KillCount를 "KILLCOUNT"키에 저장, 데이터 매니저로 대체
        //해당 클래스는 보안성이 없다. 실무에서는 암호화해서 저장해야한다.
    }

    public void AddItem(Item item) //인벤토리에 아이템을 추가 했을 때 데이터 정보를 업데이트 하는 함수.
    { //Contains는 리스트함수이며, 리스트 내 같은 데이터가 있는지 확인하는 함수.
        if (gameData.equipItem.Contains(item)) return; //보유 아이템에 같은 아이템이면 추가하지 않고 리턴함.
        gameData.equipItem.Add(item); //보유중이지 않으면 GameData.equipItem 리스트에 아이템을 추가한다.
        switch(item.itemType) //실시간 데이터 입력
        {
            case (Item.ItemType.HP): //저장된 데이터와 비교
                if (item.itemCalc == Item.ItemCalc.VALUE) //아이템 계산 방식이 값을 더하는 방식이면
                    gameData.hp += item.value;
                else //아이템 계산 방식이 퍼센트비율 만큼 더하는 방식이면
                    gameData.hp += gameData.hp * item.value;
                break;

            case (Item.ItemType.DAMAGE):
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage * item.value;
                break;

            case (Item.ItemType.SPEED):
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed * item.value;
                break;

            case (Item.ItemType.GRANADE):
                
                break;
        }
        OnItemChange(); //아이템이 변경된 것을 실시간으로 반영하기위해 이벤트를 발생시킨다.
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif
    }
    public void RemoveItem(Item item) //인벤토리에 아이템을 뺏을 때 그 아이템의 데이터를 변화 시키기 위해 사용
    {
        gameData.equipItem.Remove(item); //리스트에서 해당 아이템을 제거한다.

        switch (item.itemType) //실시간 데이터 입력
        {
            case (Item.ItemType.HP): //저장된 데이터와 비교
                if (item.itemCalc == Item.ItemCalc.VALUE) //아이템 계산 방식이 값을 더하는 방식이면
                    gameData.hp -= item.value;
                else //아이템 계산 방식이 퍼센트비율 만큼 더하는 방식이면
                    gameData.hp = gameData.hp / (1f + item.value);
                break;

            case (Item.ItemType.DAMAGE):
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage = gameData.damage / (1f + item.value);
                break;

            case (Item.ItemType.SPEED):
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed = gameData.speed / (1f + item.value);
                break;

            case (Item.ItemType.GRANADE):

                break;
        }
        OnItemChange();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif
    }
    private void OnApplicationQuit() //게임이 꺼질 때 자동 호출 된다. OnDisable() 보다 늦게 호출 된다.
    {
        SaveGameData();
    }
    private void OnDisable() //유니티를 끄면 호출 된다.
    {
        //PlayerPrefs.DeleteKey(KillKey); //"KILLCOUNT"키를 가진 프리퍼런스를 삭제한다. 데이터매니저로 대체
    }
}
