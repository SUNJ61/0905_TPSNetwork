using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyFire : MonoBehaviour
{
    private AudioClip fireClip;
    private Animator animator;
    private Transform playerTr;
    private Transform enemyTr;
    private Transform firePos;
    private WaitForSeconds reloadWs; //스타트 코루틴 반환 시간 변수.
    private AudioClip ReloadClip; //재장전 사운드.
    private MeshRenderer MuzzleFlash; //직접 만든 머즐 플래쉬를 껏다 켜기위한 메쉬 렌더러

    private float nextFire = 0.0f; //발사 딜레이 시간 계산용 변수.
    private int curBullet = 0; //현재 총알 수 체크용 변수.
    private bool isReload = false; //재장전 하는지 알려주는 변수.

    private readonly int hashFire = Animator.StringToHash("FireTrigger");
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");//재장전 트리거
    private readonly int maxBullet = 10; //최대 장탄수, 10발 쏘면 재장전감지를 위한 변수
    private readonly string enemyGunSound= "Sound/enemyGunSound";
    private readonly string reloadSound = "Sound/p_reload";
    private readonly string playerTag = "Player";
    private readonly float fireRate = 0.2f; // 총알 발사 간격
    private readonly float damping = 10.0f; // 플레이어를 향해 회전할 속도
    private readonly float reloadTime = 2.0f; //재장전 시간

    public bool isFire = false; //발사 상태를 감지하는 변수.
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag(playerTag).transform;
        firePos = transform.GetChild(4).GetChild(0).GetChild(0).transform; //에너미 모델링에 3,0,0 인덱스에 있는 오브젝트
        //find로 찾으면 에너미가 여러명이 되었을 때 같은 오브젝트 이름이 여러개 생겨 오류가 발생.
        fireClip = Resources.Load(enemyGunSound) as AudioClip;

        curBullet = maxBullet;
        reloadWs = new WaitForSeconds(reloadTime); //재장전 시간 후에 함수 반환.
        ReloadClip = Resources.Load<AudioClip>(reloadSound);

        MuzzleFlash = transform.GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        MuzzleFlash.enabled = false;
    }
    void Update()
    {
        if(isFire && !isReload && !GameManager.G_instance.isGameOver) //게임 진행중에 발사중이라면
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f); // 현재시간에서 0.2 ~ 0.4초이후 시간이 됨.
            }
            Vector3 playernormal = playerTr.position - enemyTr.position; //플레이어 - 에너미 => 에너미가 플레이어 보는 방향
            Quaternion rot = Quaternion.LookRotation(playernormal.normalized);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, damping*Time.deltaTime);
            //에너미가 보는 방향이 rot 방향으로 damping속도로 회전한다.
        }
    }

    private void Fire()
    {
        var e_bullet = ObjectPoolingManager.poolingManager.GetE_BulletPool();
        if(e_bullet != null)
        {
            e_bullet.transform.position = firePos.transform.position;
            e_bullet.transform.rotation = firePos.transform.rotation;
            e_bullet.SetActive(true);
        }

        animator.SetTrigger(hashFire);
        SoundManager.S_instance.PlaySound(firePos.position,fireClip);
        isReload = (--curBullet % maxBullet) == 0; //먼저 -1되고 실행, 즉 9부터 감소하다가 0이 되면 리로드가 true가 된다.
        if(isReload)
        {
            StartCoroutine(Reloading()); //리로드 변수가 true가 되면 Reloading함수 불러옴.
        }
        StartCoroutine(ShowMuzzleFlash());
    }
    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload); //재장전 애니메이션 실행
        SoundManager.S_instance.PlaySound(enemyTr.transform.position, ReloadClip); //재장전 사운드 실행

        yield return reloadWs; //재장전 시간만큼 대기하는 동안 제어권 양보

        curBullet = maxBullet; //재장전이 끝났으므로 총알수를 다시 10으로 변경.
        isReload = false; //재장전이 끝났으므로 재장전 상태 false.
    }
    IEnumerator ShowMuzzleFlash()
    {
        MuzzleFlash.enabled = true;
        MuzzleFlash.transform.localScale = Vector3.one * Random.Range(0.2f, 1.0f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        MuzzleFlash.transform.localRotation = rot; //로컬 로케이션 부모 오브젝트와 상관없이 회전

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        MuzzleFlash.enabled = false;
    }
}
