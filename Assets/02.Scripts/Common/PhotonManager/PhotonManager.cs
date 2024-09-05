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
        Debug.Log("마스터 서버 접속");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방이 없어 접속 실패! 방을 만듭니다.");
        OnMakeRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("룸 접속");
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
            if(roomInfo.RemovedFromList == true) //해당 룸이 삭제됬을 때
            {
                Rooms.TryGetValue(roomInfo.Name, out tempRoom); //딕셔너리에 저장된 room프리팹을 임의의 공간에 저장.
                Destroy(tempRoom); //저장된 room 삭제
                Rooms.Remove(roomInfo.Name); //딕셔너리에서 해당 room 삭제
            }
            else //해당 룸이 삭제되지 않았을 때
            {
                if (Rooms.ContainsKey(roomInfo.Name) == false) //룸이 딕셔너리에 존재하지 않을 때
                {
                    GameObject roomPrefab = Instantiate(Room_Prefab, scrollContent.transform); //룸 오브젝트를 룸리스트에 생성
                    roomPrefab.GetComponent<RoomData>().Roominfo = roomInfo; //해당 룸 이름을 roomInfo로 변경
                    Rooms.Add(roomInfo.Name, roomPrefab); //딕셔너리에 해당 룸이름으로 룸을 저장
                }
                else //룸이 딕셔너리에 존재 할 때
                {
                    Rooms.TryGetValue(roomInfo.Name, out tempRoom); //딕셔너리의 룸을 임의의 변수에 저장
                    tempRoom.GetComponent<RoomData>().Roominfo = roomInfo; //해당 룸의 이름을 roomInfo로 설정
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
