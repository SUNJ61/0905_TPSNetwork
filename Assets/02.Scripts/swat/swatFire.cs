using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swatFire : MonoBehaviour
{
    private AudioClip fireClip;
    private Animator animator;
    private Transform P_Tr;
    private Transform S_Tr;
    private Transform firePos;
    private WaitForSeconds reloadWs;
    private AudioClip ReloadClip;
    private ParticleSystem MuzzleFlash;

    private float nextFire = 0.0f;
    private int curBullet = 0;
    private bool isReload = false;

    private readonly int hashFire = Animator.StringToHash("FireTrigger");
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");
    private readonly int maxBullet = 10;
    private readonly string swatGunSound = "Sound/enemyGunSound";
    private readonly string reloadSound = "Sound/p_reload";
    private readonly string playerTag = "Player";
    private readonly float fireRate = 0.2f;
    private readonly float damping = 10.0f;
    private readonly float reloadTime = 2.0f;

    public bool isFire = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        S_Tr = GetComponent<Transform>();
        P_Tr = GameObject.FindWithTag(playerTag).GetComponent<Transform>();
        firePos = transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Transform>();
        fireClip = Resources.Load<AudioClip>(swatGunSound);

        curBullet = maxBullet;
        reloadWs = new WaitForSeconds(reloadTime);
        ReloadClip = Resources.Load<AudioClip>(reloadSound);

        MuzzleFlash = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
    }
    void Update()
    {
        MuzzleFlash.Stop();
        if (isFire && !isReload && !GameManager.G_instance.isGameOver)
        {
            if(Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }
            Vector3 playernormal = P_Tr.position - S_Tr.position;
            Quaternion rot = Quaternion.LookRotation(playernormal.normalized);
            S_Tr.rotation = Quaternion.Slerp(S_Tr.rotation, rot, Time.deltaTime * damping);
        }
    }
    private void Fire()
    {
        var s_bullet = ObjectPoolingManager.poolingManager.GetS_BulletPool();
        if(s_bullet != null)
        {
            s_bullet.transform.position = firePos.transform.position;
            s_bullet.transform.rotation = firePos.transform.rotation;
            s_bullet.SetActive(true);

            animator.SetTrigger(hashFire);
            SoundManager.S_instance.PlaySound(firePos.position, fireClip);
            isReload = (--curBullet % maxBullet) == 0;
            if (isReload)
            {
                StartCoroutine(Reloading());
            }
            StartCoroutine(ShowMuzzleFlash());
        }
    }
    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload); //재장전 애니메이션 실행
        SoundManager.S_instance.PlaySound(S_Tr.transform.position, ReloadClip);

        yield return reloadWs;

        curBullet = maxBullet;
        isReload = false;
    }
    IEnumerator ShowMuzzleFlash()
    {
        MuzzleFlash.Play();

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        MuzzleFlash.Stop();
    }
}
