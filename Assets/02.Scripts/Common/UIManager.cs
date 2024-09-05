using UnityEngine;
//using UnityEngine.SceneManagement; //싱글게임에서만 필요
using UnityEditor;

public class UIManager : MonoBehaviour
{
    #region 싱글게임 접속
    //public void OnClickPlayBtn() //버튼이 눌러졌을 때 씬을 변경
    //{ //네트워크 게임으로 바꾸면서 사용 x
    //    #region 로드씬사용 (해당기능 로드씬에서 실행)
    //    //SceneManager.LoadScene("Level_1"); //Level_1씬을 불러온다.
    //    //SceneManager.LoadScene("BattleFieldScene",LoadSceneMode.Additive);//LoadSceneMode.Additive는
    //    //기존씬(Level_1씬)을 삭제하지 않고 BattleFieldScene씬을 추가해서 새로운 씬을 로드한다. (즉, 씬 병합)
    //    //LoadSceneMode.Single은 기존 씬을 삭제하고 ()안에 선언된 새로운 씬을 로드한다.
    //    #endregion
    //    SceneManager.LoadScene("SceneLoader");
    //}
    #endregion
    public void OnClickQuitBtn()
    {
#if UNITY_EDITOR
        //실행중인 어플리케이션 종료
        EditorApplication.isPlaying = false; //유니티에서 종료
#else
    Application.Quit(); // 출시하려고 빌드 한 화면에서 종료, 유저에서 종료
#endif
    }
}
