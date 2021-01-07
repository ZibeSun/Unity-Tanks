using System;
using UnityEngine;

[Serializable]
//Serializable：指示可序列化的类或结构。
public class TankManager
{
    //Color：在 Unity 中，该结构用于传递颜色。
    public Color m_PlayerColor;            //声明一个公共Color结构m_PlayerColor;
    public Transform m_SpawnPoint;         //声明一个公共的Transform类对象m_SpawnPoint，储存坦克的出生位置;
    [HideInInspector] public int m_PlayerNumber;             //玩家编号；        
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     


    private TankMovement m_Movement;       //声明一个私有TankMovement脚本组件m_Movement; 
    private TankShooting m_Shooting;       //声明一个私有TankShooting脚本组件m_Shooting;
    private GameObject m_CanvasGameObject;

    //坦克的初始化设置
    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();   //将坦克的TankMovement脚本组件返回给 m_Movement；
        m_Shooting = m_Instance.GetComponent<TankShooting>();   //将坦克的TankShooting脚本组件返回给 m_Shooting；
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;  //获取坦克的Canvas子对象；

        m_Movement.m_PlayerNumber = m_PlayerNumber;       //给脚本组件设置玩家编号；
        m_Shooting.m_PlayerNumber = m_PlayerNumber;       //给脚本组件设置玩家编号；

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";
        //设置玩家名称并着色；
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        //创建MeshRenderer对象数组，获取坦克所有MeshRenderer子对象并返回给它；
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;     //设置坦克的颜色；
        }
    }

    //禁用坦克控制
    public void DisableControl()
    {
        m_Movement.enabled = false;              //停用m_Movement脚本；
        m_Shooting.enabled = false;              //停用m_Shooting脚本；

        m_CanvasGameObject.SetActive(false);    //停用坦克的UI显示；
    }

    //启用坦克控制
    public void EnableControl()
    {
        m_Movement.enabled = true;              //启用m_Movement脚本；
        m_Shooting.enabled = true;              //启用m_Shooting脚本；

        m_CanvasGameObject.SetActive(true);     //启用坦克的UI显示；
    }

    //重置坦克
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;     //使坦克回到出生点；
        m_Instance.transform.rotation = m_SpawnPoint.rotation;     //重置坦克角度；

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
