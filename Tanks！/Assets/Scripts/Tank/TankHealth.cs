using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;           //坦克满血数值；
    //Slider组件：一个包含浮点值的滑动条。
    public Slider m_Slider;                         //声明一个公共 Slider 组件m_Slider;                 
    public Image m_FillImage;                       //声明一个公共 Iamge 组件 m_FillImage;
    public Color m_FullHealthColor = Color.green;   //坦克满血时血条颜色；
    public Color m_ZeroHealthColor = Color.red;     //坦克被摧毁时血条颜色；
    public GameObject m_ExplosionPrefab;            //声明一个公共游戏对象 m_ExplosionPrefab；


    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;    //声明一个私有粒子系统组件 m_ExplosionParticles；
    private float m_CurrentHealth;                  //坦克当前血量数值；
    private bool m_Dead;                            //声明一个私有布尔类型变量m_Dead来判断坦克的死活；

    //脚本加载的时候执行的代码
    private void Awake()
    {
        //Instantiate()：将对象的所有子物体和子组件完全复制，成为一个新的对象。
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        //将爆炸特效预制件m_ExplosionPrefab的粒子组件返回给 m_ExplosionParticles；
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        //将爆炸特效预制件m_ExplosionPrefab的音频源返回给m_ExplosionAudio；
        m_ExplosionParticles.gameObject.SetActive(false);
        //停用爆炸特效预制件m_ExplosionPrefab对象；
    }

    //当物体被激活的时候执行的代码 
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;    //初始化坦克血量数值；
        m_Dead = false;                        //初始化坦克状态为未被摧毁；

        SetHealthUI();                         //初始化坦克血量显示；
    }
   
    //调整坦克的血量数值以及血量显示，输入值amount为伤害值；
    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth -= amount;    //当前坦克血量减去伤害值；
        SetHealthUI();                //更新坦克血量显示；
        if(m_CurrentHealth<=0f&&!m_Dead)   
        {
            OnDeath();                     //若坦克血量小于等于0且当前状态为未被摧毁执行OnDeath()；
        }
    }

    //获得补血道具时补血；
    public void TakeHealth(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        m_CurrentHealth += amount;    //当前坦克血量减去伤害值；
        if (m_CurrentHealth > 100f )
        {
            m_CurrentHealth = 100f;
        }
        SetHealthUI();                //更新坦克血量显示；
    }

    //调整坦克血条的数值以及血条的颜色
    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;    //将m_CurrentHealth赋给m_Slider组件滑动条的值；
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        //给血条设置颜色以及数值；
        //Color Lerp (Color a, Color b, float t)：在颜色 a 与 b 之间按 t 进行线性插值。
        //t是夹在0到1之间。当t=0时，返回颜色a。当t=1时，返回颜色b。当t=0.5时返回颜色a和b之间的平均色。
        /*
            a	颜色 a。
            b	颜色 b。
            t	用于组合 a 和 b 的浮点数。
         */
    }

    //坦克被摧毁时执行的代码
    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;                  //坦克状态设置为已被摧毁；
        m_ExplosionParticles.transform.position = transform.position;
        //将坦克的位置赋给爆炸特性粒子组件m_ExplosionParticles；
        m_ExplosionParticles.gameObject.SetActive(true);
        //启用爆炸特效预制件m_ExplosionPrefab对象；
        m_ExplosionParticles.Play();
        //启动粒子系统；
        m_ExplosionAudio.Play();
        //播放爆炸音效；
        gameObject.SetActive(false);
        //停用坦克对象；
    }
}