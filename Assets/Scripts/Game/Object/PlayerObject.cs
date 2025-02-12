using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
    //�����Ծ���ٶ�
    public float jumpSpeed = 10;
    //�������ٶ�
    public float gSpeed = 9.8f;
    //��ǰ����Ծ�ٶ�
    private float nowJumpSpeed;

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

        //���� ��Ծ�߼�
        //�����ڵ����� �Ϳ�����Ծ
        if(roleAnimator.GetBool("isGround") == false)
        {
            
            //��Ծ�������
            bodyTransform.Translate(Vector2.up * nowJumpSpeed * Time.deltaTime);
            //��ֱ���� �����߼� �ٶȱ仯
            //v = v - gt
            nowJumpSpeed -= gSpeed * Time.deltaTime;

            //�����жϸ߶��Ƿ�С�ڵ���0 �����ж��Ƿ����
            //ע�� һ�������ж� == 0 ��Ϊ�� - ֡���ʱ��*�ٶ� �󲿷����������ոյ���0
            //ע��ʹ�õ���localPosition �˽�Position��localPosition
            if (bodyTransform.localPosition.y <= 0)
            {
                //���õ�����
                bodyTransform.localPosition = Vector2.zero;
                //�ı�����ʶ
                roleAnimator.SetBool("isGround", true);
            }
        }
    }

    private void Jump()
    {
        //�ڵ�����true ����������Ծ
        //֮���������Ծ��Լ������
        if (roleAnimator.GetBool("isGround"))
        {
            nowJumpSpeed = jumpSpeed;
            //�л�����
            roleAnimator.SetTrigger("jumpTrigger");
            //�л��ڵ����״̬
            roleAnimator.SetBool("isGround", false);
        }
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
                Jump();
                break;
        }
    }

    private void OnDestroy()
    {
        //�¼� �мӾ��м� �Ƴ�ʱһ��Ҫע���¼�
        RemoveController();
    }
}
