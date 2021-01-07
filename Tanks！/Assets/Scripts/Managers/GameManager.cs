using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        //赢得最终胜利所需的获胜轮数；
    public float m_StartDelay = 3f;         //每轮游戏开始前的准备时间；
    public float m_EndDelay = 3f;           //每轮游戏结束后的等待时间；
    public CameraControl m_CameraControl;   //声明一个公共CameraControl脚本组件m_CameraControl；
    public Text m_MessageText;              
    public GameObject m_TankPrefab;        
    public TankManager[] m_Tanks;


    private int m_RoundNumber;              //游戏已进行的轮数；
    //WaitForSeconds：使用缩放时间将协程执行暂停指定的秒数。
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;


    public float AddHealthCreatTime = 3f;
    public float PlusRaidusCreatTime = 12f;
    public float MultipleShellsCreatTime = 8f;
    public GameObject m_AddHealth;
    public Transform m_AddHealthTransform;

    public GameObject m_PlusRaidus;
    public Transform m_PlusRaidusTransform;

    public GameObject m_MultipleShells;
    public Transform m_MultipleShellsTransform;

    //场景加载好，开始的时候执行一次 （用来初始化数据），一定会在Update系列函数前面
    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);        //初始化设置每轮游戏开始前的准备时间；
        m_EndWait = new WaitForSeconds(m_EndDelay);            //初始化设置每轮游戏结束后的等待时间；

        SpawnAllTanks();
        SetCameraTargets();
        StartCoroutine(GameLoop());             //StartCoroutine()：启动协程。
    }

    // 每一帧执行一次
    private void Update()
    {
        AddHealthCreatTime -= Time.deltaTime;
        if (AddHealthCreatTime <= 0)
        {
            AddHealthCreatTime = Random.Range(6, 10);
            GameObject AddHealthInstance = Instantiate(m_AddHealth, m_AddHealthTransform.position, m_AddHealthTransform.rotation);
            AddHealthInstance.transform.position = new Vector3(Random.Range(20f, -22f), 1f, Random.Range(18f, -13f));
        }

        PlusRaidusCreatTime -= Time.deltaTime;
        if (PlusRaidusCreatTime <= 0)
        {
            PlusRaidusCreatTime = Random.Range(8, 12);
            GameObject PlusRaidusInstance = Instantiate(m_PlusRaidus, m_PlusRaidusTransform.position, m_PlusRaidusTransform.rotation);
            PlusRaidusInstance.transform.position = new Vector3(Random.Range(20f, -22f), 1f, Random.Range(18f, -13f));
        }

        MultipleShellsCreatTime -= Time.deltaTime;
        if (MultipleShellsCreatTime <= 0)
        {
            MultipleShellsCreatTime = Random.Range(8, 12);
            GameObject MultipleShellsInstance = Instantiate(m_MultipleShells, m_MultipleShellsTransform.position, m_MultipleShellsTransform.rotation);
            MultipleShellsInstance.transform.position = new Vector3(Random.Range(20f, -22f), 1f, Random.Range(18f, -13f));
        }
    }

    //生成所有坦克；
    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            //使用坦克预制件在设定好不同出生点处生成坦克； 
            m_Tanks[i].m_PlayerNumber = i + 1;    //设置玩家编号；
            m_Tanks[i].Setup();                   //初始化所有坦克；
        }
    }

    //将所有坦克设置为相机的拍摄目标；
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];      //创建一个Transform类对象数组来储存所有坦克的位置；

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;         //获取所有坦克的位置；
        }

        m_CameraControl.m_Targets = targets;                      //将所有坦克的位置返回给摄像机脚本；
    }


    private IEnumerator GameLoop()
    {
        
        yield return StartCoroutine(RoundStarting());      //启动RoundStarting协程，等待它执行完毕返回来继续执行想一行；
        yield return StartCoroutine(RoundPlaying());       //启动RoundPlaying协程，等待它执行完毕返回来继续执行想一行；
        yield return StartCoroutine(RoundEnding());        //启动RoundEnding协程，等待它执行完毕返回来继续执行想一行；

        if (m_GameWinner != null)
        {
            Application.LoadLevel(Application.loadedLevel);    //若已决出最终胜利玩家，则
        }
        else
        {
            StartCoroutine(GameLoop());                    //若还未决出最终胜利玩家，则开启下一轮游戏；
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();                  //重置所有坦克；
        DisableTankControl();             //禁用所有坦克控制；

        m_CameraControl.SetStartPositionAndSize();      //每一轮开始时初始化相机的位置以及画幅大小；

        m_RoundNumber++;                  //游戏已进行的轮数+1；
        m_MessageText.text = "ROUND " + m_RoundNumber;      //显示当前游戏轮数；
        
        yield return m_StartWait;         //等待三秒结束；
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();                    //启用所有坦克控制；
        m_MessageText.text = string.Empty;      //string.Empty：表示空字符串。

        while (!OneTankLeft())
        {
            yield return null;                 //若有一个以上坦克存活，则暂缓一帧，等待一帧 Update 后继续向下执行；
        }
        //若只剩下一个坦克存活或者没有坦克存活，则跳出while，RoundPlaying结束；
        //yield return null表示暂缓一帧，在下一帧接着往下处理
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();                  //禁用所有坦克控制；

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if(m_RoundWinner!=null)
        {
            m_RoundWinner.m_Wins++;          //若本轮游戏有获胜玩家，则该玩家获胜轮数+1；
        }

        m_GameWinner = GetGameWinner();

        string message = EndMessage(); 

        m_MessageText.text = message;       //显示UI文本；
        yield return m_EndWait;             //每轮结束时等待三秒；
    }

    //判断场上坦克存活数量是否等于或少于一辆；
    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;        //若只剩下一个坦克存活或者没有坦克存活，则返回true；
    }

    //获取本轮游戏获胜的玩家
    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];                    //若本轮游戏结束时该坦克仍存活，则该玩家获胜；
        }

        return null;             //若本轮游戏结束时没有玩家存活，则返回null；
    }

    //获取游戏最终获胜的玩家
    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];                         //若该玩家获胜轮数达到获取最终胜利所需的轮数，则该玩家获胜；
        }

        return null;                         ////若没有玩家获胜轮数达到获取最终胜利所需的轮数，则返回null；
    }

    //判断所需显示的文本
    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    //重置所有坦克；
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    //启用所有坦克控制；
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }

    //禁用所有坦克控制；
    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}