using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemyFOV : MonoBehaviourPun
{
    public float viewRange = 15.0f; //��ĳ������ ���� �����Ÿ�
    [Range(0, 360)] //�Ʒ��� ������ ������ 0~360���� �����Ѵ�.
    public float viewAngle = 120f; //��ĳ������ �þ߰�

    private Transform enemyTr;
    //private Transform playerTr; //�̱ۿ� �÷��̾� ��ġ
    [SerializeField]private GameObject[] playerTr; //��Ƽ���� �÷��̾���� ������� ����
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
        //playerTr = GameObject.FindWithTag(playerTag).GetComponent<Transform>(); //�̱ۿ�

        playerLayer = LayerMask.NameToLayer(playerLay);
        boxLayer = LayerMask.NameToLayer(boxLay);
        barrelLayer = LayerMask.NameToLayer(barrelLay);
        layerMask = 1 << playerLayer | 1 << boxLayer | 1 << barrelLayer;
    }

    private void OnEnable()
    {
        if(PhotonNetwork.IsMasterClient)
            StartCoroutine(SearchPlayer()); //��Ʈ��ũ���� �÷��̾� �߰��Լ�
    }

    IEnumerator SearchPlayer() //�÷��̾ �ȴ��� ������ ���� ���ľ���
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
        angle += transform.eulerAngles.y; //���� ��ǥ�� �������� �����ϱ� ���� ��ĳ������ Yȸ������ ���Ѵ�.
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
        //�������� 1�� ���� ���̶� �����ϸ� �ش� ����3�� x���� Sin = x/1, z���� cos = z/1�̴�.
        //Deg2Rad�� ���� ������ ��ȯ���ִ� ���. (PI*2)/360 �� �ǹ�. (Rad2Deg�� ������ �Ϲ� ������ ��ȯ�ϴ� ���)
    }

    public bool isTracePlayer() //�������� �Ǵ��ϴ� �Լ�.
    {
        bool isTrace = false;
        //���ʹ��� �����ǿ��� �÷��̾ 15�ݰ�ȿ� �÷��̾ �ִ��� �˻��ϰ� ������ cols�� ��´�.
        Collider[] cols = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);
        if (cols.Length == 1) //�÷��̾ �����ȿ� �ִٸ�
        {
            //Vector3 dir = (playerTr.position - enemyTr.position).normalized; //�÷��̾ �ٶ󺸴� ������ ������. //�̱�
            Vector3 dir = (playerTr[search_Player].transform.position - enemyTr.position).normalized; //�÷��̾ �ٶ󺸴� ������ ������. //��Ƽ
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {//���ʹ̰� �ٶ󺸰��ִ� ���⿡�� �÷��̾ �ٶ󺸴� ������ �þ߰��� ������ �þ߰��� ���ݺ��� ������
                isTrace = true;//player�� �ٶ󺸰� �ִٰ� �Ǵ��Ͽ� �÷��̾ �Ѵ´�.
            }
        }
        return isTrace;
    }
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        //Vector3 dir = (playerTr.position - enemyTr.position).normalized; //�÷��̾ �ٶ󺸴� ���� //�̱�
        Vector3 dir = (playerTr[search_Player].transform.position - enemyTr.position).normalized; //�÷��̾ �ٶ󺸴� ���� //��Ƽ

        if(Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {//���ʹ��� ��ġ���� �÷��̾� �������� ������ ���� viewRange�ȿ��� �÷��̾�, ��ֹ�, �跲���� �¾�����
            isView = (hit.collider.CompareTag(playerTag)); //���� ��ü�� �ݶ��̴��� �±װ� �÷��̾�� true
        }
        return isView;
    }
}
