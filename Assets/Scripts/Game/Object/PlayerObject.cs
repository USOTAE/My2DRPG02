using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : RoleObject
{
    //�������м���
    private int atkCount = 0;
    //�Ȳ��������м���
    private int footAtkCount = 0;

    protected override void Awake()
    {
        base.Awake();

        //�����������
        InputMgr.GetInstance().StartOrEndCheck(true);
        //��ȡ����Ȩ��
        GetController();
    }

    /// <summary>
    /// ����
    /// </summary>
    public override void Dead()
    {
    }

    /// <summary>
    /// ʰȡ
    /// </summary>
    public void PickUp()
    {
        roleAnimator.SetTrigger("pickUpTrigger");
    }

    /// <summary>
    /// Ͷ��
    /// </summary>
    public void Throw()
    {
        roleAnimator.SetTrigger("throwTrigger");
    }


    private void Jump()
    {
        //�ڵ�����true ����������Ծ
        //֮���������Ծ��Լ������
        AnimatorStateInfo layerInfo = roleAnimator.GetCurrentAnimatorStateInfo(1);
        if (roleAnimator.GetBool("isGround") &&
            roleAnimator.GetBool("isDefend") == false &&
            roleAnimator.GetBool("isHit") == false &&
            roleAnimator.GetBool("isHitFly") == false &&
            IsAtkState == false &&
            layerInfo.IsName("Pickup") == false &&
            layerInfo.IsName("Throw") == false)
        {
            nowYSpeed = jumpSpeed;
            //�л�����
            ChangeAction(E_Action_Type.Jump);
            //�л��ڵ����״̬
            ChangeRoleIsGround(false);
        }
    }

    /// <summary>
    /// ��Ծ����
    /// </summary>
    private void JumpAtk()
    {
        //�����ǰ������Ծ����״̬ �Ͳ�Ҫ�ٴ�����Ծ����
        if (roleAnimator.GetCurrentAnimatorStateInfo(1).IsName("JumpAtk") == false)
        {
            ChangeAction(E_Action_Type.JumpAtk);
        }
    }

    /// <summary>
    /// �ֲ�����
    /// </summary>
    public override void Atk()
    {
        //�жϵ�ǰ�ܲ���ִ�й�����Ϊ
        //�����ж��м��ַ�ʽ
        //1 ����״̬�ж� 
        //2 ���������ж�
        //�����㹥��������ֱ�ӷ���
        if (roleAnimator.GetBool("isDefend"))
            return;

        //Ϊ�����������ļ�ʱ������� ʹ����������������
        //Ŀ���Ǳ������¼�����Ͷ���ʱ����̵��µ��ظ���������

        //��ʽ1 ��������ȥ����
        ChangeAction(E_Action_Type.Atk);

        //��ʽ2 ʹ��int�ۼ�������
        //�ü�������
        //1 �ӳٺ���
        //2 �Լ�д��ʱ�߼�(�Լ�ʵ�� ��ʱ�߼� ��ʱ������)
        //3 ͨ��Эͬ����ȥ��ʱ

        //ÿ�ΰ��� ȡ����һ���ӳٺ���
        CancelInvoke(nameof(DelayClearAtkCount));
        //�ֲ��������м���
        atkCount++;
        roleAnimator.SetInteger("atkCount", atkCount);
        //���м����ӳ�����
        Invoke(nameof(DelayClearAtkCount), .3f);
    }

    /// <summary>
    /// �Ȳ����� �ο��ֲ�����
    /// </summary>
    private void FootAtk()
    {
        if (roleAnimator.GetBool("isDefend"))
            return;

        //�л��Ȳ�����
        ChangeAction(E_Action_Type.FootAtk);

        CancelInvoke(nameof(DelayClearFootAtkCount));
        footAtkCount++;
        roleAnimator.SetInteger("footAtkCount", footAtkCount);
        Invoke(nameof(DelayClearFootAtkCount), .3f);
    }

    /// <summary>
    /// �Ƿ��
    /// </summary>
    /// <param name="isDefend"></param>
    private void Defend(bool isDefend)
    {
        roleAnimator.SetBool("isDefend", isDefend);
    }

    private void DelayClearFootAtkCount()
    {
        footAtkCount = 0;
        roleAnimator.SetInteger("footAtkCount", footAtkCount);
    }

    private void DelayClearAtkCount()
    {
        atkCount = 0;
        roleAnimator.SetInteger("atkCount", atkCount);
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
        //��������̧��
        EventCenter.GetInstance().AddEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
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
        EventCenter.GetInstance().RemoveEventListener<KeyCode>("SomeKeyUp", CheckKeyUp);
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
                //�ֲ�����
                //������ڵ��� ������Ծ�����߼�
                if (roleAnimator.GetBool("isGround") == false)
                    JumpAtk();
                //�ڵ����� �ܹ�����
                //�Ժ�����������������ټ�
                else
                    Atk();
                break;
            case KeyCode.K:
                //�Ȳ�����
                if (roleAnimator.GetBool("isGround") == false)
                    JumpAtk();
                else
                    FootAtk();
                break;
            case KeyCode.L:
                //��
                Defend(true);
                break;
            case KeyCode.Space:
                Debug.Log("Space");
                Jump();
                break;
            //���԰���
            case KeyCode.Alpha1:
                Wound(1f);
                break;
            case KeyCode.Alpha2:
                HitFly(-10f,10f);
                break;
            case KeyCode.Alpha3:
                PickUp();
                break;
            case KeyCode.Alpha4:
                Throw();
                break;
        }
    }

    private void CheckKeyUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.L:
                Defend(false);
                break;
        }
    }


    private void OnDestroy()
    {
        //�¼� �мӾ��м� �Ƴ�ʱһ��Ҫע���¼�
        RemoveController();
    }
}
