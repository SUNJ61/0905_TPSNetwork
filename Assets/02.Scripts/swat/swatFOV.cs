using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swatFOV : MonoBehaviour
{
    public float viewRange = 15.0f;
    [Range(0, 360)]
    public float viewAngle = 120.0f;

    private Transform swatTr;
    //private Transform playerTr; //싱글용 플레이어 위치
    private GameObject[] playerTr; //멀티에서 플레이어들을 담기위한 변수
    public GameObject PlayerTr
    {
        get { return playerTr[search_Player]; }
    }

    private int playerLayer;
    private int barrelLayer;
    private int boxLayer;
    private int layerMask;
    private int search_Player;
    private int player_Index = 0;

    private readonly string playerTag = "Player";
    private readonly string playerLay = "PLAYER";
    private readonly string barrelLay = "BARREL";
    private readonly string boxLay = "BOXES";
    void Start()
    {
        swatTr = GetComponent<Transform>();
        //playerTr = GameObject.FindWithTag(playerTag).GetComponent<Transform>();

        playerLayer = LayerMask.NameToLayer(playerLay);
        barrelLayer = LayerMask.NameToLayer(barrelLay);
        boxLayer = LayerMask.NameToLayer(boxLay);
        layerMask = 1 << playerLayer | 1 << barrelLayer | 1 << boxLayer;

        StartCoroutine(SearchPlayer()); //네트워크에서 플레이어 추격함수
    }
    IEnumerator SearchPlayer()
    {
        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(1.0f);

            playerTr = GameObject.FindGameObjectsWithTag(playerTag);
            if (playerTr != null)
            {
                Transform target = playerTr[0].transform;
                float distance = Vector3.Distance(target.position, swatTr.position);

                float dist;
                foreach (var player in playerTr)
                {
                    dist = Vector3.Distance(player.transform.position, swatTr.position);
                    if (distance > dist)
                    {
                        target = player.transform;
                        distance = dist;
                        search_Player = player_Index;
                    }
                    player_Index++;
                }
                player_Index = 0;
            }
        }
    }

        public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
    public bool isTracePlayer()
    {
        bool isTrace = false;
        Collider[] cols = Physics.OverlapSphere(swatTr.position, viewRange, 1 << playerLayer);
        if (cols.Length == 1)
        {
            //Vector3 dir = (playerTr.position - swatTr.position).normalized;//싱글
            Vector3 dir = (playerTr[search_Player].transform.position - swatTr.position).normalized; //플레이어를 바라보는 방향을 저장함. //멀티
            if (Vector3.Angle(swatTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        //Vector3 dir = (playerTr.position - swatTr.position).normalized; //싱글
        Vector3 dir = (playerTr[search_Player].transform.position - swatTr.position).normalized; //플레이어를 바라보는 방향 //멀티

        if (Physics.Raycast(swatTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag(playerTag));
        }

        return isView;
    }
}
