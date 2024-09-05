using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemyFOV : MonoBehaviourPun
{
    public float viewRange = 15.0f; //적캐릭터의 추적 사정거리
    [Range(0, 360)] //아래의 변수의 범위를 0~360으로 제한한다.
    public float viewAngle = 120f; //적캐릭터의 시야각

    private Transform enemyTr;
    //private Transform playerTr; //싱글용 플레이어 위치
    [SerializeField]private GameObject[] playerTr; //멀티에서 플레이어들을 담기위한 변수
    public GameObject PlayerTr
    {
        get
        {
            if(playerTr.Length > 0)
                return playerTr[search_Player];
            else
                return null;
        }
    }

    private int playerLayer;
    private int boxLayer;
    private int barrelLayer;
    private int layerMask;
    private int search_Player;
    private int player_Index = 0;

    private readonly string playerTag = "Player";
    private readonly string playerLay = "PLAYER";
    private readonly string boxLay = "BOXES";
    private readonly string barrelLay = "BARREL";
    void Awake()
    {
        enemyTr = GetComponent<Transform>();
        //playerTr = GameObject.FindWithTag(playerTag).GetComponent<Transform>(); //싱글용

        playerLayer = LayerMask.NameToLayer(playerLay);
        boxLayer = LayerMask.NameToLayer(boxLay);
        barrelLayer = LayerMask.NameToLayer(barrelLay);
        layerMask = 1 << playerLayer | 1 << boxLayer | 1 << barrelLayer;
    }

    private void OnEnable()
    {
        if(PhotonNetwork.IsMasterClient)
            StartCoroutine(SearchPlayer()); //네트워크에서 플레이어 추격함수
    }

    IEnumerator SearchPlayer() //플레이어가 안담기는 문제가 있음 고쳐야함
    {
        yield return new WaitForSeconds(2.0f);

        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(0.2f);

            playerTr = GameObject.FindGameObjectsWithTag(playerTag);
            Transform target = playerTr[0].transform;
            float distance = Vector3.Distance(target.position, enemyTr.position);

            float dist;
            foreach(var player in playerTr)
            {
                dist = Vector3.Distance(player.transform.position, enemyTr.position);
                if(distance > dist)
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
    public Vector3 CirclePoint(float angle)
    {
        angle += transform.eulerAngles.y; //로컬 좌표계 기준으로 설정하기 위해 적캐릭터의 Y회전값을 더한다.
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
        //반지름이 1인 단위 원이라 가정하면 해당 벡터3의 x값은 Sin = x/1, z값은 cos = z/1이다.
        //Deg2Rad는 라디안 값으로 변환해주는 상수. (PI*2)/360 를 의미. (Rad2Deg는 라디안을 일반 각도로 변환하는 상수)
    }

    public bool isTracePlayer() //추적할지 판단하는 함수.
    {
        bool isTrace = false;
        //에너미의 포지션에서 플레이어가 15반경안에 플레이어가 있는지 검사하고 있으면 cols에 담는다.
        Collider[] cols = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);
        if (cols.Length == 1) //플레이어가 범위안에 있다면
        {
            //Vector3 dir = (playerTr.position - enemyTr.position).normalized; //플레이어를 바라보는 방향을 저장함. //싱글
            Vector3 dir = (playerTr[search_Player].transform.position - enemyTr.position).normalized; //플레이어를 바라보는 방향을 저장함. //멀티
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {//에너미가 바라보고있는 방향에서 플레이어를 바라보는 방향의 시야각이 설정한 시야각의 절반보다 작으면
                isTrace = true;//player를 바라보고 있다고 판단하여 플레이어를 쫓는다.
            }
        }
        return isTrace;
    }
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        //Vector3 dir = (playerTr.position - enemyTr.position).normalized; //플레이어를 바라보는 방향 //싱글
        Vector3 dir = (playerTr[search_Player].transform.position - enemyTr.position).normalized; //플레이어를 바라보는 방향 //멀티

        if(Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {//에너미의 위치에서 플레이어 방향으로 광선을 쏴서 viewRange안에서 플레이어, 장애물, 배럴등이 맞았으면
            isView = (hit.collider.CompareTag(playerTag)); //맞은 물체의 콜라이더의 태그가 플레이어면 true
        }
        return isView;
    }
}
