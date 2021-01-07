using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;          //声明一个公共的Transform类对象m_FireTransform，储存炮弹发射的位置以及角度; 
    public Transform m_FireTransform2;
    public Transform m_FireTransform3;
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;        //声明一个公共的音频源 m_ShootingAudio;
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f;       //设置最小发射力度为15；
    public float m_MaxLaunchForce = 30f;       //设置最大发射力度为30；
    public float m_MaxChargeTime = 0.75f;      //设置从最小发射力度提升到最大发射力度所需时间为0.75s；


    private string m_FireButton;              //玩家发射键位名称；
    private float m_CurrentLaunchForce;       //发射力度；
    private float m_ChargeSpeed;              //发射力度提升速度；
    private bool m_Fired;                     //声明一个私有布尔类型变量m_Fired来判断玩家是否发射；

    private bool m_Plus;
    private bool m_Multiple;

    //当物体被激活的时候执行的代码 
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;         //初始化发射力度为最小发射力度；
        m_AimSlider.value = m_MinLaunchForce;            //初始化发射力度显示箭头；
        m_Plus = false;
        m_Multiple = false;
    }

    //场景加载好，开始的时候执行一次 （用来初始化数据），一定会在Update系列函数前面
    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;          //为不同玩家设置不同的发射键位名称；

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

    }

    // 每一帧执行一次
    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;

        if(m_CurrentLaunchForce>=m_MaxLaunchForce&&!m_Fired)    //若发射力度已达到最大而玩家还未松开发射按键
        {
            //at max charge,not yet fired
            m_CurrentLaunchForce = m_MaxLaunchForce;            //则以最大发射力度发射炮弹；
            Fire();
        }
        else if(Input.GetButtonDown(m_FireButton))              //玩家按下发射键时
        {
            //have we pressed fire for the first time?
            m_Fired = false;                                    //设置发射状态为未发射
            m_CurrentLaunchForce = m_MinLaunchForce;            //设置发射力度为最小发射力度；

            m_ShootingAudio.clip = m_ChargingClip;              //将音频源m_ShootingAudio的默认音频片段设置为蓄力音效；
        }
        else if(Input.GetButton(m_FireButton)&&!m_Fired)        //玩家按下发射键而未松开时
        {
            //Holding the fire button,not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed *Time.deltaTime;              //发射力度逐渐增加；

            m_AimSlider.value = m_CurrentLaunchForce;                   //发射力度显示箭头逐渐拉长；
        }
        else if(Input.GetButtonUp(m_FireButton) &&!m_Fired)       //玩家松开发射键时
        {
            //we released the button,having not fired yet
            Fire();                                              //开火；
        }
    }

    //生成并发射炮弹
    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;                       //设置发射状态为已发射
        //Object Instantiate(Object original, Vector3 position, Quaternion rotation);
        //克隆 original 对象的所有子对象和组件并返回
        /*
            original	要复制的现有对象。
            position	新对象的位置。
            rotation	新对象的方向。
        */
        //as  Rigidbody：强制转换为刚体组件；
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        //在坦克炮口处生成一个炮弹刚体；

        if(m_Plus)
        {
            ShellExplosion targetShell = shellInstance.GetComponent<ShellExplosion>();
            targetShell.m_ExplosionRadius = 30f;
        }
        if (m_Multiple)
        {
            Rigidbody shellInstance2 = Instantiate(m_Shell, m_FireTransform2.position, m_FireTransform2.rotation) as Rigidbody;
            Rigidbody shellInstance3 = Instantiate(m_Shell, m_FireTransform3.position, m_FireTransform3.rotation) as Rigidbody;
            shellInstance2.velocity = m_CurrentLaunchForce * m_FireTransform2.forward;
            shellInstance3.velocity = m_CurrentLaunchForce * m_FireTransform3.forward;
        }

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        //给炮弹一个速度矢量；
        //Vector3 Rigidbody.velocity ;刚体的速度矢量。它表示刚体位置的变化率。
        m_ShootingAudio.clip = m_FireClip;          //将音频源m_ShootingAudio的默认音频片段设置为开火音效；
        m_ShootingAudio.Play();                     //播放开火音效；

        m_CurrentLaunchForce = m_MinLaunchForce;    //重置发射力度为最小发射力度；
    }

    void OnTriggerEnter(Collider other)           //OnTriggerEnter的输入参数为碰撞对象的 Collider 组件；
    {
        if (other.gameObject.CompareTag("PlusRadius")) 
        {
            other.gameObject.SetActive(false);    //停用碰撞对象；
            m_Plus = true;
        }
        if (other.gameObject.CompareTag("MultipleShells"))
        {
            other.gameObject.SetActive(false);    //停用碰撞对象；
            m_Multiple = true;
        }
    }

}