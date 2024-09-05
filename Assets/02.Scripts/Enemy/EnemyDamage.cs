using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private GameObject bloodEffect;
    private Image Hpbar;
    private Text Hptext;

    //private readonly string bulletTag = "BULLET"; //프로젝타일에서 사용
    //private readonly string S_bulletTag = "S_BULLET"; //프로젝타일에서 사용
    private readonly string BLDeffTag = "Effects/BulletImpactFleshBigEffect";
    //private readonly string enemyUI = "Canvas_EnemyUI"; //캔퍼스 찾을때 사용하려고 했지만 사용 x

    private float Hp = 100.0f;
    private float InitHp = 100f;
    void Awake()
    {
        bloodEffect = Resources.Load<GameObject>(BLDeffTag);
        Hpbar = transform.GetChild(5).GetChild(1).GetComponent<Image>();
        Hptext = transform.GetChild(5).GetChild(2).GetComponent<Text>();

        Hp = Mathf.Clamp(Hp, 0f, InitHp);
        Hpbar.color = Color.green;
    }
    private void OnEnable()
    {
        Hp = 100.0f;
        HpBarCtrl();
    }
    #region 프로젝타일 방식의 충돌감지
    //private void OnCollisionEnter(Collision col)
    //{
    //    if (col.collider.CompareTag(bulletTag) || col.collider.CompareTag(S_bulletTag))
    //    {
    //        col.gameObject.SetActive(false);
    //        ShowBLD_Effect(col);
    //        Hp -= col.gameObject.GetComponent<Bullet>().Damage;
    //        if (Hp <= 0)
    //            Die();
    //    }
    //}//레이캐스트에서는 사용하지 못한다.
    #endregion
    void OnDamage(object[] _params)
    {
        ShowBLD_Effect((Vector3)_params[0]);
        Hp -= (float)_params[1];
        HpBarCtrl();
        if (Hp <= 0)
            Die();
    }
    private void HpBarCtrl()
    {
        Hpbar.fillAmount = (float)Hp / (float)InitHp;
        if (Hp <= 30f)
            Hpbar.color = Color.red;
        else if (Hp <= 50f)
            Hpbar.color = Color.yellow;
        else
            Hpbar.color = Color.green;
        Hptext.text = $"HP : <color=#FF0000>{Hp.ToString()}</color>";
    }
    #region 프로젝타일 용 코드
    //private void ShowBLD_Effect(Collision col)
    //{ 
    //    Vector3 pos = col.contacts[0].point;
    //    //collision구조체 안에 contacts라는 배열이 있는데 여기엔 감지된 위치가 저장되어 있다.
    //    Vector3 _Normal = col.contacts[0].normal; // 오브젝트가 감지된 방향을 얻는다.
    //    Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
    //    //뒷 방향에서 오브젝트가 날라가던 방향으로 이펙트를 출력
    //    GameObject blood = Instantiate(bloodEffect, pos, rot);
    //    Destroy(blood, 1.0f);
    //}
    #endregion
    private void ShowBLD_Effect(Vector3 col)
    {
        Vector3 pos = col;
        Vector3 _Normal = col.normalized; // 오브젝트가 감지된 방향을 얻는다.
        Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
        //뒷 방향에서 오브젝트가 날라가던 방향으로 이펙트를 출력
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
    void Die()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE; //에너미 상태를 관리하는 스크립트를 불러와 상태 변경
        GameManager.G_instance.KillScore();
    }
    void ExpDie()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.EXPDIE;
        GameManager.G_instance.KillScore();
    }
}
