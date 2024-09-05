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
    private WaitForSeconds reloadWs; //��ŸƮ �ڷ�ƾ ��ȯ �ð� ����.
    private AudioClip ReloadClip; //������ ����.
    private MeshRenderer MuzzleFlash; //���� ���� ���� �÷����� ���� �ѱ����� �޽� ������

    private float nextFire = 0.0f; //�߻� ������ �ð� ���� ����.
    private int curBullet = 0; //���� �Ѿ� �� üũ�� ����.
    private bool isReload = false; //������ �ϴ��� �˷��ִ� ����.

    private readonly int hashFire = Animator.StringToHash("FireTrigger");
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");//������ Ʈ����
    private readonly int maxBullet = 10; //�ִ� ��ź��, 10�� ��� ������������ ���� ����
    private readonly string enemyGunSound= "Sound/enemyGunSound";
    private readonly string reloadSound = "Sound/p_reload";
    private readonly string playerTag = "Player";
    private readonly float fireRate = 0.2f; // �Ѿ� �߻� ����
    private readonly float damping = 10.0f; // �÷��̾ ���� ȸ���� �ӵ�
    private readonly float reloadTime = 2.0f; //������ �ð�

    public bool isFire = false; //�߻� ���¸� �����ϴ� ����.
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag(playerTag).transform;
        firePos = transform.GetChild(4).GetChild(0).GetChild(0).transform; //���ʹ� �𵨸��� 3,0,0 �ε����� �ִ� ������Ʈ
        //find�� ã���� ���ʹ̰� �������� �Ǿ��� �� ���� ������Ʈ �̸��� ������ ���� ������ �߻�.
        fireClip = Resources.Load(enemyGunSound) as AudioClip;

        curBullet = maxBullet;
        reloadWs = new WaitForSeconds(reloadTime); //������ �ð� �Ŀ� �Լ� ��ȯ.
        ReloadClip = Resources.Load<AudioClip>(reloadSound);

        MuzzleFlash = transform.GetChild(4).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        MuzzleFlash.enabled = false;
    }
    void Update()
    {
        if(isFire && !isReload && !GameManager.G_instance.isGameOver) //���� �����߿� �߻����̶��
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f); // ����ð����� 0.2 ~ 0.4������ �ð��� ��.
            }
            Vector3 playernormal = playerTr.position - enemyTr.position; //�÷��̾� - ���ʹ� => ���ʹ̰� �÷��̾� ���� ����
            Quaternion rot = Quaternion.LookRotation(playernormal.normalized);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, damping*Time.deltaTime);
            //���ʹ̰� ���� ������ rot �������� damping�ӵ��� ȸ���Ѵ�.
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
        isReload = (--curBullet % maxBullet) == 0; //���� -1�ǰ� ����, �� 9���� �����ϴٰ� 0�� �Ǹ� ���ε尡 true�� �ȴ�.
        if(isReload)
        {
            StartCoroutine(Reloading()); //���ε� ������ true�� �Ǹ� Reloading�Լ� �ҷ���.
        }
        StartCoroutine(ShowMuzzleFlash());
    }
    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload); //������ �ִϸ��̼� ����
        SoundManager.S_instance.PlaySound(enemyTr.transform.position, ReloadClip); //������ ���� ����

        yield return reloadWs; //������ �ð���ŭ ����ϴ� ���� ����� �纸

        curBullet = maxBullet; //�������� �������Ƿ� �Ѿ˼��� �ٽ� 10���� ����.
        isReload = false; //�������� �������Ƿ� ������ ���� false.
    }
    IEnumerator ShowMuzzleFlash()
    {
        MuzzleFlash.enabled = true;
        MuzzleFlash.transform.localScale = Vector3.one * Random.Range(0.2f, 1.0f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        MuzzleFlash.transform.localRotation = rot; //���� �����̼� �θ� ������Ʈ�� ������� ȸ��

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        MuzzleFlash.enabled = false;
    }
}
