using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviourPunCallbacks
{
    public static ObjectPoolingManager poolingManager;
    private GameObject Bullet;
    private int MaxPool = 10; //오브젝트 풀링으로 생성할 갯수
    public List<GameObject> bulletpoolList; //using System.Collections.Generic; 활성화 됨

    private GameObject E_Bullet;
    private int E_MaxPool = 20;
    public List<GameObject> E_bulletpoolList;

    private GameObject EnemyPrefab;
    public List<GameObject> EnemyList;
    private int E_Spawn_MaxPool = 3;
    public List<Transform> SpawnPointList;

    private GameObject S_Bullet;
    private int S_MaxPool = 20;
    public List<GameObject> S_bulletpoolList;

    private GameObject swatPrefab;
    public List<GameObject> swatList;
    private int S_Spawn_MaxPool = 3;

    private string bullet = "Bullet";
    private string E_bullet = "E_Bullet";
    private string enemy = "Enemy";
    private string S_bullet = "S_Bullet";
    private string swat = "swat";

    private int E_idx;
    private int S_idx;
    void Awake()
    {
        if (poolingManager == null)
            poolingManager = this;
        else if (poolingManager != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Bullet = Resources.Load(bullet) as GameObject;
        E_Bullet = Resources.Load(E_bullet) as GameObject;
        EnemyPrefab = Resources.Load<GameObject>(enemy);
        S_Bullet = Resources.Load<GameObject>(S_bullet);
        swatPrefab = Resources.Load<GameObject>(swat);

        if (PhotonNetwork.IsMasterClient) //마스터 클라이언트에서만 몬스터 소환
        {
            //CreatBulletPool(); //오브젝트 풀링 생성 함수
            CreatE_BulletPool();
            CreatEnemyPool();
            //CreatS_BulletPool();
            //CreatswatPool();
        }
    }
    private void Start() //오브젝트 풀링이후 스폰 포인트를 잡기위해 start선언
    {
            var spawnPoint = GameObject.Find("SpawnPoints"); //var에 스폰포인트 오브젝트 할당
            if (spawnPoint != null) //var 스픈포인트가 잡혔다면
                spawnPoint.GetComponentsInChildren<Transform>(SpawnPointList); //오브젝트의 부모자식을 모두 리스트에 잡는다.

            SpawnPointList.RemoveAt(0); //가장 먼저 잡힌 부모 오브젝트 삭제

            if (PhotonNetwork.IsMasterClient && SpawnPointList.Count > 0) //RPC로 리모트 클라이언트 소환.
            {
                StartCoroutine(CreatEnemy());
                //StartCoroutine(Creatswat());
            }
    }

    private void CreatEnemyPool()
    {
        GameObject EnemyGroup = new GameObject("EnemyGroup");
        for (int i = 0; i < E_Spawn_MaxPool; i++)
        {
            #region 싱글 오브젝트 풀링
            //var enemyObj = Instantiate(EnemyPrefab, EnemyGroup.transform);
            //enemyObj.name = $"{(i + 1).ToString()}마리";
            //enemyObj.SetActive(false);
            //EnemyList.Add(enemyObj);
            #endregion
            var enemyObj = PhotonNetwork.Instantiate(EnemyPrefab.name, Vector3.zero, Quaternion.identity); //멀티
            enemyObj.name = $"{(i + 1).ToString()}마리";
            enemyObj.SetActive(false);
            enemyObj.transform.SetParent(EnemyGroup.transform);
            EnemyList.Add(enemyObj);
        }
    }
    #region 리스트 데이터를 전달하는 함수 쓰면 오류가 난다.
    //[PunRPC]
    //private void SendEnemyGroupData()
    //{
    //    List<PoolData> EnemyPools = new List<PoolData>();
    //    foreach(var enemy in EnemyList)
    //    {
    //        PoolData enemyData = new PoolData()
    //        {
    //            prefabname = EnemyPrefab.name,
    //            position = enemy.transform.position,
    //            rotation = enemy.transform.rotation,
    //            active = enemy.activeSelf,
    //            name = enemy.name
    //        };
    //        EnemyPools.Add(enemyData);
    //    }
    //    StreamBuffer streamBuffer = new StreamBuffer();
    //    short length = PoolDataSerialization.SerializePoolData(streamBuffer, EnemyPools);

    //    photonView.RPC("ReceiveEnemyGroupData", RpcTarget.Others, streamBuffer.ToArray());
    //}

    //[PunRPC]
    //private void ReceiveEnemyGroupData(byte[] Enemydata)
    //{
    //    StreamBuffer streamBuffer = new StreamBuffer(Enemydata);
    //    List<PoolData> enemyPools = PoolDataSerialization.DeserializePoolData(streamBuffer);


    //    GameObject EnemyGroup = new GameObject("EnemyGroup");
    //    foreach (var data in enemyPools)
    //    {
    //        GameObject enemyObj = PhotonNetwork.Instantiate(data.prefabname, data.position, data.rotation);
    //        enemyObj.name = data.name;
    //        enemyObj.SetActive(data.active);
    //        enemyObj.transform.SetParent(EnemyGroup.transform);
    //        EnemyList.Add(enemyObj);
    //    }
    //}
    #endregion

    IEnumerator CreatEnemy()
    {
        yield return new WaitForSeconds(5.0f);
        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(3.0f);
            if (GameManager.G_instance.isGameOver) yield break; //게임오버가 된다면, startcoroutine을 나간다.
            #region 싱글용 생성
            //foreach(GameObject _enemy in EnemyList)
            //{
            //    if(_enemy.activeSelf == false)
            //    {
            //        idx = Random.Range(0,SpawnPointList.Count);
            //        _enemy.transform.position = SpawnPointList[idx].position;
            //        _enemy.transform.rotation = SpawnPointList[idx].rotation;
            //        _enemy.gameObject.SetActive(true);

            //        photonView.RPC("RemoteCreatEnemy", RpcTarget.Others, _enemy, E_idx);
            //        break; //한마리 태어나고 foreach종료
            //    }
            //}
            #endregion 
            for(int i = 0; i < EnemyList.Count; i++) //게임오브젝트가 아닌 해당 인덱스를 넘기기 위해 for문으로 변경
            {
                if (EnemyList[i].activeSelf == false)
                {
                    E_idx = Random.Range(0, SpawnPointList.Count);
                    EnemyList[i].transform.position = SpawnPointList[E_idx].position;
                    EnemyList[i].transform.rotation = SpawnPointList[E_idx].rotation;
                    EnemyList[i].gameObject.SetActive(true);

                    //photonView.RPC("RemoteCreatEnemy", RpcTarget.Others, i, E_idx); //리스트 전달 함수, 쓰면 오류남
                    break; //한마리 태어나고 foreach종료
                }
            }
        }
    }

    #region 리스트 데이터를 받아 저장하는 함수. 쓰면 오류가 난다.
    //[PunRPC]
    //private void RemoteCreatEnemy(int i ,int idx)
    //{
    //    EnemyList[i].transform.position = SpawnPointList[idx].position;
    //    EnemyList[i].transform.rotation = SpawnPointList[idx].rotation;
    //    EnemyList[i].gameObject.SetActive(true);
    //}
    #endregion

    private void CreatswatPool()
    {
        GameObject swatGroup = new GameObject("swatGroup");
        for (int i = 0; i < S_Spawn_MaxPool; i++)
        {
            #region 싱글 오브젝트 풀링
            //var swatObj = Instantiate(swatPrefab, swatGroup.transform);
            //swatObj.name = $"{(i + 1).ToString()}마리";
            //swatObj.SetActive(false);
            //swatList.Add(swatObj);
            #endregion
            var swatObj = PhotonNetwork.Instantiate(swatPrefab.name, Vector3.zero, Quaternion.identity); //멀티
            swatObj.name = $"{(i + 1).ToString()}마리";
            swatObj.SetActive(false);
            swatObj.transform.SetParent(swatGroup.transform);
            swatList.Add(swatObj);
        }
    }

    IEnumerator Creatswat()
    {
        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(3.0f);
            if (GameManager.G_instance.isGameOver) yield break;
            foreach (GameObject _swat in swatList)
            {
                if (_swat.activeSelf == false)
                {
                    S_idx = Random.Range(0, SpawnPointList.Count);
                    _swat.transform.position = SpawnPointList[S_idx].position;
                    _swat.transform.rotation = SpawnPointList[S_idx].rotation;
                    _swat.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    void CreatBulletPool()
    {
        GameObject PlayerBulletGroup = new GameObject("PlayerBulletGroup "); //게임 오브젝트 한개 생성
        for (int i = 0; i < MaxPool; i++)
        {
            var _bullet = Instantiate(Bullet, PlayerBulletGroup.transform); //총알 오브젝트를 10개를 PlayerBulletGroup 안에 생성
            _bullet.name = $"{(i+1).ToString()}발"; //오브젝트 이름을 1발 부터 10발 까지
            _bullet.SetActive(false); //생성 된것 비활성화
            bulletpoolList.Add(_bullet); // 생성된 총알 리스트에 넣었다.
        }
    }
    public GameObject GetBulletPool()
    {
        for(int i = 0; i < bulletpoolList.Count; i++)
        {
            if(bulletpoolList[i].activeSelf == false) //해당 번째의 총알이 비활성화 되어있다면
            {
                return bulletpoolList[i]; //비활성화 되어있다면 총알 반환
            }
        }
        return null; //활성화 되어있으면 null을 반환
    }

    void CreatE_BulletPool()
    {
        GameObject EnemyBulletGroup = new GameObject("EnemyBulletGroup "); //게임 오브젝트 한개 생성
        for (int i = 0; i < E_MaxPool; i++)
        {
            #region 싱글 오브젝트 풀링
            //var _bullet = Instantiate(E_Bullet, EnemyBulletGroup.transform); //총알 오브젝트를 10개를 PlayerBulletGroup 안에 생성
            //_bullet.name = $"{(i + 1).ToString()}발"; //오브젝트 이름을 1발 부터 10발 까지
            //_bullet.SetActive(false); //생성 된것 비활성화
            //E_bulletpoolList.Add(_bullet); // 생성된 총알 리스트에 넣었다.
            #endregion
            var _bullet = PhotonNetwork.Instantiate(E_Bullet.name, Vector3.zero, Quaternion.identity); 
            _bullet.name = $"{(i + 1).ToString()}발";
            _bullet.SetActive(false);
            _bullet.transform.SetParent(EnemyBulletGroup.transform);
            E_bulletpoolList.Add(_bullet);
        }
    }
    public GameObject GetE_BulletPool()
    {
        for (int i = 0; i < E_bulletpoolList.Count; i++)
        {
            if (E_bulletpoolList[i].activeSelf == false) //해당 번째의 총알이 비활성화 되어있다면
            {
                return E_bulletpoolList[i]; //비활성화 되어있다면 총알 반환
            }
        }
        return null; //활성화 되어있으면 null을 반환
    }

    void CreatS_BulletPool()
    {
        GameObject swatBulletGroup = new GameObject("swatBulletGroup "); //게임 오브젝트 한개 생성
        for (int i = 0; i < S_MaxPool; i++)
        {
            #region 싱글 오브젝트 풀링
            //var s_bullet = Instantiate(S_Bullet, swatBulletGroup.transform); //총알 오브젝트를 10개를 PlayerBulletGroup 안에 생성
            //s_bullet.name = $"{(i + 1).ToString()}발"; //오브젝트 이름을 1발 부터 10발 까지
            //s_bullet.SetActive(false); //생성 된것 비활성화
            //S_bulletpoolList.Add(s_bullet); // 생성된 총알 리스트에 넣었다.
            #endregion
            var s_bullet = PhotonNetwork.Instantiate(S_Bullet.name, Vector3.zero, Quaternion.identity);
            s_bullet.name = $"{(i + 1).ToString()}발";
            s_bullet.SetActive(false);
            s_bullet.transform.SetParent(swatBulletGroup.transform);
            S_bulletpoolList.Add(s_bullet);
        }
    }
    public GameObject GetS_BulletPool()
    {
        for (int i = 0; i < S_bulletpoolList.Count; i++)
        {
            if (S_bulletpoolList[i].activeSelf == false) //해당 번째의 총알이 비활성화 되어있다면
            {
                return S_bulletpoolList[i]; //비활성화 되어있다면 총알 반환
            }
        }
        return null; //활성화 되어있으면 null을 반환
    }

    private void OnSerialization(List<PoolData> poolData)
    {
        StreamBuffer streamBuffer = new StreamBuffer();
        short length = PoolDataSerialization.SerializePoolData(streamBuffer, poolData);
    }

    private void OnDeSerialization(byte[] data)
    {
        StreamBuffer streamBuffer = new StreamBuffer(data);
        List<PoolData> enemyPools = PoolDataSerialization.DeserializePoolData(streamBuffer);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //if(PhotonNetwork.IsMasterClient)
        //{
            //photonView.RPC("SendEnemyGroupData", RpcTarget.MasterClient);
        //}
    }
}
