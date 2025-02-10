using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    void Awake()
    {
        GetController();
    }

    void Update()
    {

    }

    /// <summary>
    /// 给予控制权
    /// </summary>
    public void GetController()
    {
        //事件中心有加就有减 
        //一定不要往里往里面传lambda表达式 一定是在下方去声明一个函数
        EventCenter.GetInstance().AddEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().AddEventListener<float>("Vertical", CheckY);

        //监听按键按下
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }

    /// <summary>
    /// 剥夺控制权
    /// </summary>
    public void RemoveController()
    {
        //事件中心有加就有减 
        //一定不要往里往里面传lambda表达式 一定是在下方去声明一个函数
        EventCenter.GetInstance().RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().RemoveEventListener<float>("Vertical", CheckY);
        //监听按键按下
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }

    private void CheckX(float x)
    {
        //x 就会是 -1 0 1三个值的数
        //按A为-1 不按为0 按D为1
    }

    private void CheckY(float y)
    {
        
    }

    /// <summary>
    /// 检测玩家 除移动以外的输入
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyDown(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.J:
                break;
            case KeyCode.K:
                break;
            case KeyCode.L:
                break;
            case KeyCode.Space:
                break;
        }
    }

    private void OnDestroy()
    {
        //事件 有加就有减 移除时一定要注销事件
        RemoveController();
    }
}
