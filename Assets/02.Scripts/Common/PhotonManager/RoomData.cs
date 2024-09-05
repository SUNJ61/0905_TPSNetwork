using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
public class RoomData : MonoBehaviourPun
{
    private RoomInfo _Roominfo;
    private Text roomInfoText;
    private PhotonManager photonManager;

    public RoomInfo Roominfo
    {
        get { return _Roominfo; }
        set 
        {
            _Roominfo = value;
            roomInfoText.text = $"{_Roominfo.Name}\t ({_Roominfo.PlayerCount}/{_Roominfo.MaxPlayers})";
            GetComponent<Button>().onClick.AddListener(() => OnEnterRoom(_Roominfo.Name));
        }
    }
    void Awake()
    {
        roomInfoText = GetComponentInChildren<Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }
    private void OnEnterRoom(string roomName)
    {
        photonManager.SetUserID();
        PhotonNetwork.JoinRoom(roomName);
    }
}
