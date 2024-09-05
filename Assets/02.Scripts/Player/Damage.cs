using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private GameObject BLDeff;
    private Image BloodScreen;
    private Image Hpbar;

    private readonly string e_bullettag = "E_BULLET";
    private readonly string s_bullettag = "S_BULLET";
    //private readonly string enemytag = "ENEMY"; //for문 돌려서 플레이어가 죽었을 때 오브젝트 찾기 용도
    //private readonly string swattag = "SWAT"; //for문 돌려서 플레이어가 죽었을 때 오브젝트 찾기 용도
    private readonly string BLDeffStr = "Effects/BulletImpactFleshBigEffect";
    private readonly string UIObj = "Canvas_UI";

    private float curHp = 0f;
    [SerializeField]private float InitHp;
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;//static을 달아서 class안에서만 사용하는 것이 아닌 외부에서도 사용할 수 있도록 한다.
    private void OnEnable()
    {
        GameManager.OnItemChange += UpdateSetUp; //아이템이 추가 or 빠질 때 발동 된다.
    }
    void UpdateSetUp()
    {
        InitHp = GameManager.G_instance.gameData.hp;
        curHp += GameManager.G_instance.gameData.hp - curHp;
    }
    void Start()
    {
        BLDeff = Resources.Load<GameObject>(BLDeffStr);
        InitHp = GameManager.G_instance.gameData.hp;
        curHp = InitHp;

        BloodScreen =GameObject.Find(UIObj).transform.GetChild(0).GetComponent<Image>();
        Hpbar = GameObject.Find(UIObj).transform.GetChild(2).GetChild(2).GetComponent<Image>();
        Hpbar.color = Color.green;
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.CompareTag(e_bullettag) || col.gameObject.CompareTag(s_bullettag))
        {
            col.gameObject.SetActive(false);
            curHp = Mathf.Clamp(curHp, 0f, InitHp);
            curHp -= 5;
            ShowBLD_Eff(col);
            HpBarCtrl();
            if (curHp <= 0f)
            {
                PlayerDie();
            }
            StartCoroutine(ShowBLD_Screen());
        }
    }

    private void HpBarCtrl()
    {
        Hpbar.fillAmount = (float)curHp / (float)InitHp;
        if (curHp <= 30f)
            Hpbar.color = Color.red;
        else if (curHp <= 50f)
            Hpbar.color = Color.yellow;
        else
            Hpbar.color = Color.green;
    }

    IEnumerator ShowBLD_Screen()
    {
        BloodScreen.color = new Color(1, 0, 0, Random.Range(0.25f,0.35f));
        yield return new WaitForSeconds(1f);
        BloodScreen.color = Color.clear; //색상, 알파값을 전부 0으로 만든다.
    }
    public void PlayerDie()
    {
        #region 플레이어가 죽었을 때 모든 적을 배열에 담아 for문으로 비활성화
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemytag);// 적이 많아지면 for문으로 하기에는 효율이 좋지않음.
        //GameObject[] swats = GameObject.FindGameObjectsWithTag(swattag);
        //for(int i = 0; i < enemies.Length; i++)
        //{
        //    enemies[i].gameObject.SendMessage("OnPlayerDie",SendMessageOptions.DontRequireReceiver);
        //}
        //for(int i = 0; i < swats.Length;i++)
        //{
        //    swats[i].gameObject.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        #endregion
        OnPlayerDie(); // delegate void PlayerDie() 대리자를 수식받은 변수.
    }
    private void ShowBLD_Eff(Collision col)
    {
        Vector3 pos = col.contacts[0].point;
        Vector3 _Normal = col.contacts[0].normal;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _Normal);
        GameObject BLD = Instantiate(BLDeff, pos, rot);
        Destroy(BLD, 1.0f);
    }
}
