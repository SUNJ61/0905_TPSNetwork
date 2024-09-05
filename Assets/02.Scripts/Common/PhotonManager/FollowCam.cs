using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class FollowCam : MonoBehaviourPun
{
    private CinemachineVirtualCamera V_Cam;
    //private PhotonView pv = null;
    void Start()
    {
        //pv = GetComponent<PhotonView>();
        //pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        //pv.ObservedComponents[0] = this;
        if (photonView.IsMine)
        {
            V_Cam = FindObjectOfType<CinemachineVirtualCamera>();
            V_Cam.Follow = transform;
            V_Cam.LookAt = transform;
        }
    }
}
