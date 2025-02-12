using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
    protected override void Awake()
    {
        base.Awake();
        //�����������
        InputMgr.GetInstance().StartOrEndCheck(true);
        //��ȡ����Ȩ��
        GetController();
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// �������Ȩ
    /// </summary>
    public void GetController()
    {
        //�¼������мӾ��м� 
        //һ����Ҫ���������洫lambda���ʽ һ�������·�ȥ����һ������
        EventCenter.GetInstance().AddEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().AddEventListener<float>("Vertical", CheckY);

        //������������
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }

    /// <summary>
    /// �������Ȩ
    /// </summary>
    public void RemoveController()
    {
        //�¼������мӾ��м� 
        //һ����Ҫ���������洫lambda���ʽ һ�������·�ȥ����һ������
        EventCenter.GetInstance().RemoveEventListener<float>("Horizontal", CheckX);
        EventCenter.GetInstance().RemoveEventListener<float>("Vertical", CheckY);
        //������������
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyDown", CheckKeyDown);
    }

    private void CheckX(float x)
    {
        //x �ͻ��� -1 0 1����ֵ����
        //��AΪ-1 ����Ϊ0 ��DΪ1
        //��ȡ�����ƶ�����
        moveDir.x = x;
    }

    private void CheckY(float y)
    {
        moveDir.y = y;
    }

    /// <summary>
    /// ������ ���ƶ����������
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyDown(KeyCode key)
    {
        switch(key)
        {
            case KeyCode.J:
                Debug.Log("J");
                break;
            case KeyCode.K:
                Debug.Log("K");
                break;
            case KeyCode.L:
                Debug.Log("L");
                break;
            case KeyCode.Space:
                Debug.Log("Space");
                break;
        }
    }

    private void OnDestroy()
    {
        //�¼� �мӾ��м� �Ƴ�ʱһ��Ҫע���¼�
        RemoveController();
    }
}
