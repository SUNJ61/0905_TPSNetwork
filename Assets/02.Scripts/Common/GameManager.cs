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
    public GameDataObject gameData; //���� ScriptableObject ��� �ֱ�

    public delegate void ItemChangeDelegate(); //�κ��丮 �������� ����Ǿ��� �� �߻� ��ų �̺�Ʈ ����.
    public static event ItemChangeDelegate OnItemChange;
    [SerializeField] private GameObject slotList; //���Ծȿ� �������� �ִ��� �˻��ϱ����� �����´�.
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
        Time.timeScale = (isPaused) ? 0.0f : 1.0f; //isPaused�� true�� 0, false�� 1�� �ȴ�. 
        var playerObj = GameObject.FindGameObjectWithTag(playertag); //�÷��̾� ������Ʈ�� ã�� ����
        var scripts = playerObj.GetComponents<MonoBehaviour>(); //�÷��̾� ������Ʈ�� �ִ� ��� ��ũ��Ʈ�� �����Ѵ�.
        //MonoBehaviour��� Ŭ������ ��� �޾ұ� ������ ������Ʈ�� ��ũ��Ʈ�� �� �� �ִ�. ��, MonoBehaviour�� ��ũ��Ʈ�� �ٴ� ���.
        foreach( var script in scripts )
        {
            script.enabled = !isPaused; //��� �ϸ� ��ũ��Ʈ ��Ȱ��ȭ, ��� Ǯ�� ��ũ��Ʈ Ȱ��ȭ. 
        }
        var canvasGroup = GameObject.Find(panel_WPname).GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    private void LoadGameData()
    {
        #region PlayerPrefs�� �̿��� ������ ����
        //KillCount = PlayerPrefs.GetInt(KillKey, 0); //�ش����� DataManager�� ��ü
        //�÷��̾� �����۷����� ����. �ش� Ŭ������ "KILLCOUNT"��� Ű�� ������ �����ϰ� 0�� ����.
        //"KILLCOUNT"��� Ű�� �̹� ������ ����� �����͸� �ҷ��´�.
        #endregion

        #region ���������� ������ ����
        //GameData data = dataManager.Load(); //���۵Ǹ� ������ �÷��� �ߴ� ���� ������ �����´�.
        //gameData.hp = data.hp; //�ҷ��� �����͸� ���� ���ӿ�����Ʈ�� ���� �����Ϳ� ������.
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.KillCount = data.KillCount; //�ش� ����� ScriptableObject�� ��ü
        //gameData.equipItem = data.equipItem; //ScriptableObject�� ���������� ������ �� �־� ������ �ε��ϴ� ������ �ʿ����.
        #endregion

        if (gameData.equipItem.Count > 0) //�������� ������ ���� ������ ȣ��ȴ�.
            InventorySetUp();

        KillTXT.text = "<color=#FFFFFF>KILL</color> : " + gameData.KillCount.ToString("0000"); //���ڸ� 4��° �ڸ����� ǥ��
    }
    void InventorySetUp()//���� ���� �� �κ��丮�� �������� ���� �뵵.
    {
        var slots = slotList.GetComponentsInChildren<Transform>(); //���Ը���Ʈ �ڽĿ�����Ʈ ���θ� ��´�.
        for (int i = 0; i < gameData.equipItem.Count; i++) //������ �����۸�ŭ ���ư���.
        {
            for(int j = 1; j < slots.Length; j++) //slots 0�� �ε������� �θ������Ʈ�� �� �ֱ� ������ j�� 1���� ����.
            {
                if (slots[j].childCount > 0) continue; //���Կ�����Ʈ�� �����ۿ�����Ʈ�� �ڽ����� ������ ���� �ε����� ����.
                int itemIndex = (int)gameData.equipItem[i].itemType; //�κ��丮�� ���� �������� ������ ���� �ε����� ����.
                //equipItem[i]�� �ش��ϴ� ������ �������� �ε����� itemIndex�� �����Ѵ�.
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j].transform);//�ش� �������� �θ�� ������ �ȴ�.
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                //�������� ItemInfoŬ������ itemData������ �ε��� gameData.equipItem[i] ���� ����. (�Ѵ� ItemŬ������ ��������)
                break; //���� for�� Ż���� �ܺ� for���� �ε����� �ϳ� ������Ų��.
            }
        }
    }
    void SaveGameData() //��������� ���� ���� ����
    {
        //dataManager.Save(gameData); //ScriptableObject�� ��ü
#if UNITY_EDITOR //�����ͻ󿡼��� ����.
        UnityEditor.EditorUtility.SetDirty(gameData); //����Ƽ �����ͻ󿡼� �Ͼ ���� .asset�� ������ ���Ͽ� ������ ����
#endif
    }
    public void KillScore()
    {
        //++KillCount; //���� �Ŵ����� ��ü
        ++gameData.KillCount;
        KillTXT.text = "<color=#FFFFFF>KILL</color> : " + gameData.KillCount.ToString("0000");
        //PlayerPrefs.SetInt(KillKey, KillCount); //KillCount�� "KILLCOUNT"Ű�� ����, ������ �Ŵ����� ��ü
        //�ش� Ŭ������ ���ȼ��� ����. �ǹ������� ��ȣȭ�ؼ� �����ؾ��Ѵ�.
    }

    public void AddItem(Item item) //�κ��丮�� �������� �߰� ���� �� ������ ������ ������Ʈ �ϴ� �Լ�.
    { //Contains�� ����Ʈ�Լ��̸�, ����Ʈ �� ���� �����Ͱ� �ִ��� Ȯ���ϴ� �Լ�.
        if (gameData.equipItem.Contains(item)) return; //���� �����ۿ� ���� �������̸� �߰����� �ʰ� ������.
        gameData.equipItem.Add(item); //���������� ������ GameData.equipItem ����Ʈ�� �������� �߰��Ѵ�.
        switch(item.itemType) //�ǽð� ������ �Է�
        {
            case (Item.ItemType.HP): //����� �����Ϳ� ��
                if (item.itemCalc == Item.ItemCalc.VALUE) //������ ��� ����� ���� ���ϴ� ����̸�
                    gameData.hp += item.value;
                else //������ ��� ����� �ۼ�Ʈ���� ��ŭ ���ϴ� ����̸�
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
        OnItemChange(); //�������� ����� ���� �ǽð����� �ݿ��ϱ����� �̺�Ʈ�� �߻���Ų��.
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif
    }
    public void RemoveItem(Item item) //�κ��丮�� �������� ���� �� �� �������� �����͸� ��ȭ ��Ű�� ���� ���
    {
        gameData.equipItem.Remove(item); //����Ʈ���� �ش� �������� �����Ѵ�.

        switch (item.itemType) //�ǽð� ������ �Է�
        {
            case (Item.ItemType.HP): //����� �����Ϳ� ��
                if (item.itemCalc == Item.ItemCalc.VALUE) //������ ��� ����� ���� ���ϴ� ����̸�
                    gameData.hp -= item.value;
                else //������ ��� ����� �ۼ�Ʈ���� ��ŭ ���ϴ� ����̸�
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
    private void OnApplicationQuit() //������ ���� �� �ڵ� ȣ�� �ȴ�. OnDisable() ���� �ʰ� ȣ�� �ȴ�.
    {
        SaveGameData();
    }
    private void OnDisable() //����Ƽ�� ���� ȣ�� �ȴ�.
    {
        //PlayerPrefs.DeleteKey(KillKey); //"KILLCOUNT"Ű�� ���� �����۷����� �����Ѵ�. �����͸Ŵ����� ��ü
    }
}
