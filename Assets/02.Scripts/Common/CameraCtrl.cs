using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//카메라 흔들리는 범위,시간 / 카메라가 흔들리는 걸 확인 할 bool변수(드럼통에서 관리)
public class CameraCtrl : MonoBehaviour
{
    public static CameraCtrl C_instance;

    private Vector3 CameraPos;
    private Quaternion CameraRot;
    private float CameraMoveTime;
    private bool IsMove;

    void Awake()
    {
        if (C_instance == null)
            C_instance = this;
        else if (C_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if(IsMove)
        {
            float x = Random.Range(-0.2f, 0.2f);
            float y = Random.Range(-0.2f, 0.2f);
            float z = Random.Range(-0.2f, 0.2f);

            Camera.main.transform.position += new Vector3(x, y, z);
            //Quaternion rot = CameraRot;
            if(Time.time - CameraMoveTime > 0.3f)
            {
                IsMove = false;
                Camera.main.transform.position = CameraPos;
            }
        }
    }

    public void CameraMoveOn()
    {
        IsMove = true;
        CameraPos = Camera.main.transform.position;
        //CameraRot = Camera.main.transform.rotation;
        CameraMoveTime = Time.time;
    }
}
