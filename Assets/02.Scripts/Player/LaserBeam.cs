using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
public class LaserBeam : MonoBehaviourPun
{
    private Transform Tr;
    private LineRenderer line;
    private Transform firepos;
    private Player player;
    private FireCtrl fireCtrl;

    private int enemyLayer;
    private int swatLayer;

    private string enemyTag = "ENEMY";
    private string swatTag = "SWAT";

    void Start()
    {
        Tr = transform;
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.enabled = false;
        firepos = transform.GetComponentsInParent<Transform>()[1]; //자기자신을 포함하여 부모 컴퍼넌트를 잡는다.
        //즉, 인덱스가 0이면 자기자신, 인덱스가 1이면 제일 가까운 부모 오브젝트가 된다.
        player = transform.GetComponentInParent<Player>();//가장 위에 있는 부모오브젝트를 가져오는 방법
        fireCtrl = transform.GetComponentInParent<FireCtrl>();

        enemyLayer = LayerMask.NameToLayer(enemyTag);
        swatLayer = LayerMask.NameToLayer(swatTag);
    }
    void Update()
    {
        if (Time.timeScale <= 0f) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (photonView.IsMine)
        {
            Ray ray = new Ray(firepos.position, Tr.forward);//트레일 렌더러를 입힐 레이저 생성
                                                            //광선 위치를 살짝 올린 이유는, 생성 좌표는 제일 바닥을 기준으로 했기 때문에 살짝 올림.
            RaycastHit hit;
            Debug.DrawLine(ray.origin, ray.direction * 100f, Color.blue); //씬에서 광선 보이기

            if (fireCtrl.isFire && !fireCtrl.isReloading && !player.isRun)
            {
                hit = CreatBeam(ray);
            }

            #region 마우스 클릭시 레이저 생성
            if (Input.GetMouseButtonDown(0) && !player.isRun && !fireCtrl.isReloading)
            {
                hit = CreatBeam(ray);
            }
            #endregion
        }
    }

    private RaycastHit CreatBeam(Ray ray)
    {
        RaycastHit hit;
        line.SetPosition(0, Tr.InverseTransformPoint(ray.origin));//라인 렌더러 첫번째 점의 위치 설정.
                                                                  //InverseTransformDirection는 월드 좌표(절대좌표)를 로컬좌표로 바꾸는 함수 (레이저가 시작되는 곳으로 변경)
        if (Physics.Raycast(ray, out hit, 100f)) //어떤 물체에 맞았을 때 위치를 LineRenderer의 끝점으로 설정.
        {
            line.SetPosition(1, Tr.InverseTransformPoint(hit.point));
        }
        else //광선이 무언가에 맞지 않으면, 최대 끝점 거리를 광선의 발사 위치로 부터 100으로 한다.
        {
            line.SetPosition(1, Tr.InverseTransformPoint(ray.GetPoint(100f)));
        }
        StartCoroutine(ShowLaserBeam());
        return hit;
    }

    IEnumerator ShowLaserBeam()
    {
        line.enabled=true;
        yield return new WaitForSeconds(Random.Range(0.1f,0.2f));
        line.enabled=false;
    }
}
