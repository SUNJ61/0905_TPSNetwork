using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject Room_Prefab;
    [SerializeField] private GameObject RoomList_Panel;

    [SerializeField] private InputField UserID;
    [SerializeField] private InputField RoomID;
    [SerializeField] private GameObject scrollContent;

    [SerializeField] private Button RandomJoinBtn;
    [SerializeField] private Button CreateBtn;

    private Dictionary<string, GameObject> Rooms = new Dictionary<string, GameObject>();

    private string userID;

    private readonly string Version = "V1.0.1";
    private readonly string Room_Prefab_Name = "Room";
    private readonly string UI_Name = "Start_UI";
    private readonly string RoomList_Panel_Name = "RoomList_Panel";
    private readonly string Btn_Panel_Name = "Button_Panel";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = Version;
        PhotonNetwork.ConnectUsingSettings();

        Room_Prefab = Resources.Load<GameObject>(Room_Prefab_Name);
        RoomList_Panel = GameObject.Find(RoomList_Panel_Name);

        UserID = GameObject.Find(UI_Name).transform.GetChild(3).GetChild(1).GetComponent<InputField>();
        RoomID = GameObject.Find(UI_Name).transform.GetChild(3).GetChild(3).GetComponent<InputField>();
        scrollContent = RoomList_Panel.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;

        CreateBtn = GameObject.Find(Btn_Panel_Name).transform.GetChild(0).GetComponent<Button>();
        RandomJoinBtn = GameObject.Find(Btn_Panel_Name).transform.GetChild(1).GetComponent<Button>();
    }
    private void Start()
    {
        userID = PlayerPrefs.GetString("User_ID", $"UESR_{Random.Range(1, 21):00}");

        UserID.text = userID;
        PhotonNetwork.NickName = userID;

        CreateBtn.onClick.AddListener(() => OnMakeRoom());
        RandomJoinBtn.onClick.AddListener(() => OnRandomJoin());
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� ����");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ����");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ����");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"���� ���� ���� ����! ���� ����ϴ�.");
        OnMakeRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ����");
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Multy_BattleField");
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList == true) //�ش� ���� �������� ��
            {
                Rooms.TryGetValue(roomInfo.Name, out tempRoom); //��ųʸ��� ����� room�������� ������ ������ ����.
                Destroy(tempRoom); //����� room ����
                Rooms.Remove(roomInfo.Name); //��ųʸ����� �ش� room ����
            }
            else //�ش� ���� �������� �ʾ��� ��
            {
                if (Rooms.ContainsKey(roomInfo.Name) == false) //���� ��ųʸ��� �������� ���� ��
                {
                    GameObject roomPrefab = Instantiate(Room_Prefab, scrollContent.transform); //�� ������Ʈ�� �븮��Ʈ�� ����
                    roomPrefab.GetComponent<RoomData>().Roominfo = roomInfo; //�ش� �� �̸��� roomInfo�� ����
                    Rooms.Add(roomInfo.Name, roomPrefab); //��ųʸ��� �ش� ���̸����� ���� ����
                }
                else //���� ��ųʸ��� ���� �� ��
                {
                    Rooms.TryGetValue(roomInfo.Name, out tempRoom); //��ųʸ��� ���� ������ ������ ����
                    tempRoom.GetComponent<RoomData>().Roominfo = roomInfo; //�ش� ���� �̸��� roomInfo�� ����
                }
            }
        }
    }

    private void OnRandomJoin()
    {
        SetUserID();
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnMakeRoom()
    {
        RoomOptions roomOption = new RoomOptions();
        roomOption.MaxPlayers = 4;
        roomOption.IsOpen = true;
        roomOption.IsVisible = true;

        PhotonNetwork.CreateRoom(SetRoomName(), roomOption);
    }

    public void SetUserID()
    {
        if (string.IsNullOrEmpty(UserID.text))
        {
            userID = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userID = UserID.text;
        }
        PlayerPrefs.GetString("USER_ID", userID);
        PhotonNetwork.NickName = userID;
    }

    private string SetRoomName()
    {
        if (string.IsNullOrEmpty(RoomID.text))
        {
            RoomID.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return RoomID.text;
    }
}
