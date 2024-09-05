using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    private GameObject bloodEffect;
    private Image Hpbar;
    private Text Hptext;

    //private readonly string bulletTag = "BULLET"; //������Ÿ�Ͽ��� ���
    //private readonly string S_bulletTag = "S_BULLET"; //������Ÿ�Ͽ��� ���
    private readonly string BLDeffTag = "Effects/BulletImpactFleshBigEffect";
    //private readonly string enemyUI = "Canvas_EnemyUI"; //ĵ�۽� ã���� ����Ϸ��� ������ ��� x

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
    #region ������Ÿ�� ����� �浹����
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
    //}//����ĳ��Ʈ������ ������� ���Ѵ�.
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
    #region ������Ÿ�� �� �ڵ�
    //private void ShowBLD_Effect(Collision col)
    //{ 
    //    Vector3 pos = col.contacts[0].point;
    //    //collision����ü �ȿ� contacts��� �迭�� �ִµ� ���⿣ ������ ��ġ�� ����Ǿ� �ִ�.
    //    Vector3 _Normal = col.contacts[0].normal; // ������Ʈ�� ������ ������ ��´�.
    //    Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
    //    //�� ���⿡�� ������Ʈ�� ���󰡴� �������� ����Ʈ�� ���
    //    GameObject blood = Instantiate(bloodEffect, pos, rot);
    //    Destroy(blood, 1.0f);
    //}
    #endregion
    private void ShowBLD_Effect(Vector3 col)
    {
        Vector3 pos = col;
        Vector3 _Normal = col.normalized; // ������Ʈ�� ������ ������ ��´�.
        Quaternion rot = Quaternion.FromToRotation(-transform.forward, _Normal);
        //�� ���⿡�� ������Ʈ�� ���󰡴� �������� ����Ʈ�� ���
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
    void Die()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE; //���ʹ� ���¸� �����ϴ� ��ũ��Ʈ�� �ҷ��� ���� ����
        GameManager.G_instance.KillScore();
    }
    void ExpDie()
    {
        GetComponent<EnemyAI>().state = EnemyAI.State.EXPDIE;
        GameManager.G_instance.KillScore();
    }
}
