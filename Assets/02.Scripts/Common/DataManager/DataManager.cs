using System.IO; // 바이너리 파일 입출력을 위한 네임스페이스, 데이터를 교환 하기위한 통로인 stream을 만들기 위해 사용.
using System.Runtime.Serialization.Formatters.Binary; //바이너리 파일 포멧을 실시간으로 직렬화를 사용해 전달하기 위해 사용.
using UnityEngine;//직렬화는 코드에서 선언된 자료형들(int, string, float)을 byte단위 형식으로 바꾸는 것을 말한다.
using DataInfo;

public class DataManager : MonoBehaviour
{
    [SerializeField] string dataPath; //저장 경로
    public void Initialized() //저장경로를 초기화 하는 함수
    {
        dataPath = Application.persistentDataPath + "/gameData.dat"; //파일 저장경로와 파일명,확장자 지정
        //Application.persistentDataPath 공용폴더에 gameData.dat의 이름을 가진 dat확장자 파일로 지정
    }
    public void Save(GameData gamedata)
    {
        BinaryFormatter bf = new BinaryFormatter();//바이너리 파일 포멧을 위한 BinaryFormatter 생성
        FileStream file = File.Create(dataPath);//데이터 저장을 위한 파일 생성. file에 만들어진 파일의 정보를 저장한다.

        GameData data = new GameData(); //파일에 저장할 데이터를 클래스에 할당한다.
        data.KillCount = gamedata.KillCount; //외부에서 받은 정보를 같은 클래스 변수에 다시 저장한다.
        data.hp = gamedata.hp;
        data.speed = gamedata.speed;
        data.equipItem = gamedata.equipItem;
        bf.Serialize(file, data); //file에 data정보를 직렬화 과정을 거쳐 저장한다.
        file.Close(); //파일 스트림을 닫는다. 닫지 않고 다음 로직을 진행하면 메모리가 상당히 많이 차지 한채로 게임을 진행.
    }

    public GameData Load()
    {
        if(File.Exists(dataPath))//해당 경로에 파일이 존재 한다면
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open); //지정된 경로에 파일을 연다. file에 해당 정보 저장
            GameData data = (GameData)bf.Deserialize(file); //역직렬화를 통해 저장된 데이터를 Byte형태에서 C#자료형으로 바꾼다.

            file.Close();
            return data; //저장된 데이터를 반환한다.
        }
        else //경로에 파일이 존재하지 않는다면 기본 값을 반환
        {
            GameData data = new GameData();
            return data;
        }
    }
}
