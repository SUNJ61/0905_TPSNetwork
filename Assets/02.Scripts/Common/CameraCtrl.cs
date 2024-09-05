using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ī�޶� ��鸮�� ����,�ð� / ī�޶� ��鸮�� �� Ȯ�� �� bool����(�巳�뿡�� ����)
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
