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
    private int MaxPool = 10; //������Ʈ Ǯ������ ������ ����
    public List<GameObject> bulletpoolList; //using System.Collections.Generic; Ȱ��ȭ ��

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

        if (PhotonNetwork.IsMasterClient) //������ Ŭ���̾�Ʈ������ ���� ��ȯ
        {
            //CreatBulletPool(); //������Ʈ Ǯ�� ���� �Լ�
            CreatE_BulletPool();
            CreatEnemyPool();
            //CreatS_BulletPool();
            //CreatswatPool();
        }
    }
    private void Start() //������Ʈ Ǯ������ ���� ����Ʈ�� ������� start����
    {
            var spawnPoint = GameObject.Find("SpawnPoints"); //var�� ��������Ʈ ������Ʈ �Ҵ�
            if (spawnPoint != null) //var ��������Ʈ�� �����ٸ�
                spawnPoint.GetComponentsInChildren<Transform>(SpawnPointList); //������Ʈ�� �θ��ڽ��� ��� ����Ʈ�� ��´�.

            SpawnPointList.RemoveAt(0); //���� ���� ���� �θ� ������Ʈ ����

            if (PhotonNetwork.IsMasterClient && SpawnPointList.Count > 0) //RPC�� ����Ʈ Ŭ���̾�Ʈ ��ȯ.
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
            #region �̱� ������Ʈ Ǯ��
            //var enemyObj = Instantiate(EnemyPrefab, EnemyGroup.transform);
            //enemyObj.name = $"{(i + 1).ToString()}����";
            //enemyObj.SetActive(false);
            //EnemyList.Add(enemyObj);
            #endregion
            var enemyObj = PhotonNetwork.Instantiate(EnemyPrefab.name, Vector3.zero, Quaternion.identity); //��Ƽ
            enemyObj.name = $"{(i + 1).ToString()}����";
            enemyObj.SetActive(false);
            enemyObj.transform.SetParent(EnemyGroup.transform);
            EnemyList.Add(enemyObj);
        }
    }
    #region ����Ʈ �����͸� �����ϴ� �Լ� ���� ������ ����.
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
            if (GameManager.G_instance.isGameOver) yield break; //���ӿ����� �ȴٸ�, startcoroutine�� ������.
            #region �̱ۿ� ����
            //foreach(GameObject _enemy in EnemyList)
            //{
            //    if(_enemy.activeSelf == false)
            //    {
            //        idx = Random.Range(0,SpawnPointList.Count);
            //        _enemy.transform.position = SpawnPointList[idx].position;
            //        _enemy.transform.rotation = SpawnPointList[idx].rotation;
            //        _enemy.gameObject.SetActive(true);

            //        photonView.RPC("RemoteCreatEnemy", RpcTarget.Others, _enemy, E_idx);
            //        break; //�Ѹ��� �¾�� foreach����
            //    }
            //}
            #endregion 
            for(int i = 0; i < EnemyList.Count; i++) //���ӿ�����Ʈ�� �ƴ� �ش� �ε����� �ѱ�� ���� for������ ����
            {
                if (EnemyList[i].activeSelf == false)
                {
                    E_idx = Random.Range(0, SpawnPointList.Count);
                    EnemyList[i].transform.position = SpawnPointList[E_idx].position;
                    EnemyList[i].transform.rotation = SpawnPointList[E_idx].rotation;
                    EnemyList[i].gameObject.SetActive(true);

                    //photonView.RPC("RemoteCreatEnemy", RpcTarget.Others, i, E_idx); //����Ʈ ���� �Լ�, ���� ������
                    break; //�Ѹ��� �¾�� foreach����
                }
            }
        }
    }

    #region ����Ʈ �����͸� �޾� �����ϴ� �Լ�. ���� ������ ����.
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
            #region �̱� ������Ʈ Ǯ��
            //var swatObj = Instantiate(swatPrefab, swatGroup.transform);
            //swatObj.name = $"{(i + 1).ToString()}����";
            //swatObj.SetActive(false);
            //swatList.Add(swatObj);
            #endregion
            var swatObj = PhotonNetwork.Instantiate(swatPrefab.name, Vector3.zero, Quaternion.identity); //��Ƽ
            swatObj.name = $"{(i + 1).ToString()}����";
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
        GameObject PlayerBulletGroup = new GameObject("PlayerBulletGroup "); //���� ������Ʈ �Ѱ� ����
        for (int i = 0; i < MaxPool; i++)
        {
            var _bullet = Instantiate(Bullet, PlayerBulletGroup.transform); //�Ѿ� ������Ʈ�� 10���� PlayerBulletGroup �ȿ� ����
            _bullet.name = $"{(i+1).ToString()}��"; //������Ʈ �̸��� 1�� ���� 10�� ����
            _bullet.SetActive(false); //���� �Ȱ� ��Ȱ��ȭ
            bulletpoolList.Add(_bullet); // ������ �Ѿ� ����Ʈ�� �־���.
        }
    }
    public GameObject GetBulletPool()
    {
        for(int i = 0; i < bulletpoolList.Count; i++)
        {
            if(bulletpoolList[i].activeSelf == false) //�ش� ��°�� �Ѿ��� ��Ȱ��ȭ �Ǿ��ִٸ�
            {
                return bulletpoolList[i]; //��Ȱ��ȭ �Ǿ��ִٸ� �Ѿ� ��ȯ
            }
        }
        return null; //Ȱ��ȭ �Ǿ������� null�� ��ȯ
    }

    void CreatE_BulletPool()
    {
        GameObject EnemyBulletGroup = new GameObject("EnemyBulletGroup "); //���� ������Ʈ �Ѱ� ����
        for (int i = 0; i < E_MaxPool; i++)
        {
            #region �̱� ������Ʈ Ǯ��
            //var _bullet = Instantiate(E_Bullet, EnemyBulletGroup.transform); //�Ѿ� ������Ʈ�� 10���� PlayerBulletGroup �ȿ� ����
            //_bullet.name = $"{(i + 1).ToString()}��"; //������Ʈ �̸��� 1�� ���� 10�� ����
            //_bullet.SetActive(false); //���� �Ȱ� ��Ȱ��ȭ
            //E_bulletpoolList.Add(_bullet); // ������ �Ѿ� ����Ʈ�� �־���.
            #endregion
            var _bullet = PhotonNetwork.Instantiate(E_Bullet.name, Vector3.zero, Quaternion.identity); 
            _bullet.name = $"{(i + 1).ToString()}��";
            _bullet.SetActive(false);
            _bullet.transform.SetParent(EnemyBulletGroup.transform);
            E_bulletpoolList.Add(_bullet);
        }
    }
    public GameObject GetE_BulletPool()
    {
        for (int i = 0; i < E_bulletpoolList.Count; i++)
        {
            if (E_bulletpoolList[i].activeSelf == false) //�ش� ��°�� �Ѿ��� ��Ȱ��ȭ �Ǿ��ִٸ�
            {
                return E_bulletpoolList[i]; //��Ȱ��ȭ �Ǿ��ִٸ� �Ѿ� ��ȯ
            }
        }
        return null; //Ȱ��ȭ �Ǿ������� null�� ��ȯ
    }

    void CreatS_BulletPool()
    {
        GameObject swatBulletGroup = new GameObject("swatBulletGroup "); //���� ������Ʈ �Ѱ� ����
        for (int i = 0; i < S_MaxPool; i++)
        {
            #region �̱� ������Ʈ Ǯ��
            //var s_bullet = Instantiate(S_Bullet, swatBulletGroup.transform); //�Ѿ� ������Ʈ�� 10���� PlayerBulletGroup �ȿ� ����
            //s_bullet.name = $"{(i + 1).ToString()}��"; //������Ʈ �̸��� 1�� ���� 10�� ����
            //s_bullet.SetActive(false); //���� �Ȱ� ��Ȱ��ȭ
            //S_bulletpoolList.Add(s_bullet); // ������ �Ѿ� ����Ʈ�� �־���.
            #endregion
            var s_bullet = PhotonNetwork.Instantiate(S_Bullet.name, Vector3.zero, Quaternion.identity);
            s_bullet.name = $"{(i + 1).ToString()}��";
            s_bullet.SetActive(false);
            s_bullet.transform.SetParent(swatBulletGroup.transform);
            S_bulletpoolList.Add(s_bullet);
        }
    }
    public GameObject GetS_BulletPool()
    {
        for (int i = 0; i < S_bulletpoolList.Count; i++)
        {
            if (S_bulletpoolList[i].activeSelf == false) //�ش� ��°�� �Ѿ��� ��Ȱ��ȭ �Ǿ��ִٸ�
            {
                return S_bulletpoolList[i]; //��Ȱ��ȭ �Ǿ��ִٸ� �Ѿ� ��ȯ
            }
        }
        return null; //Ȱ��ȭ �Ǿ������� null�� ��ȯ
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
