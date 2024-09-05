using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class swatDamage : MonoBehaviour
{
    private GameObject bloodEffect;
    private Image Hpbar;
    private Text Hptext;

    //private readonly string bulletTag = "BULLET"; //레이 캐스트로 변경되면서 사용 x.
    //private readonly string E_bulletTag = "E_BULLET"; //레이 캐스트로 변경되면서 사용 x.
    private readonly string BLDeffTag = "Effects/BulletImpactFleshBigEffect";

    private float Hp = 100.0f;
    private float InitHp = 100f;
    void Awake()
    {
        bloodEffect = Resources.Load<GameObject>(BLDeffTag);
        Hpbar = transform.GetChild(3).GetChild(1).GetComponent<Image>();
        Hptext = transform.GetChild(3).GetChild(2).GetComponent<Text>();

        Hp = Mathf.Clamp(Hp, 0f, 100f);
        Hpbar.color = Color.green;
    }
    private void OnEnable()
    {
        Hp = 100.0f;
        HpBarCtrl();
    }
    #region 프로젝타일 감지방법
    //private void OnCollisionEnter(Collision col)
    //{
    //    if(col.collider.CompareTag(bulletTag)|| col.collider.CompareTag(E_bulletTag))
    //    {
    //        col.gameObject.SetActive(false);
    //        ShowBLD_Effect(col);
    //        Hp -= col.gameObject.GetComponent<Bullet>().Damage;
    //        if(Hp <= 0)
    //            Die();
    //    }
    //}
    #endregion
    void OnDamage(object[] _params)
    {
        ShowBLD_Effect(_params);
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
    private void ShowBLD_Effect(Collision col)
    {
        Vector3 pos = col.contacts[0].point;
        Vector3 _Normal = col.contacts[0].normal;
        Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
    private void ShowBLD_Effect(object[] _params)
    {
        Vector3 pos = (Vector3)_params[0];
        Vector3 _Normal = pos.normalized;
        Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
    void Die()
    {
        GetComponent<swatAI>().state = swatAI.S_State.DIE;
        GameManager.G_instance.KillScore();
    }
    void ExpDie()
    {
        GetComponent<swatAI>().state = swatAI.S_State.EXPDIE;
        GameManager.G_instance.KillScore();
    }
}
