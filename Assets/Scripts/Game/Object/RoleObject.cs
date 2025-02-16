using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������� ֮��Ĺ��� ��Ҷ��̳���
/// </summary>
public class RoleObject : MonoBehaviour
{
    //��ɫ���ƶ�����
    protected Vector2 moveDir = Vector2.zero;

    //�ƶ��ٶ�
    public int speed = 5;

    //����sprite
    protected SpriteRenderer roleSprite;
    //��ɫanimator
    protected Animator roleAnimator;
    //�����Ӷ���
    protected Transform bodyTransform;

    protected virtual void Awake()
    {
        //����Ӷ���ʱ���ҵ�Ŀ���Ӷ���
        bodyTransform = this.transform.Find("Role");
        roleSprite = bodyTransform.GetComponentInChildren<SpriteRenderer>();
        roleAnimator = this.GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        //���ƶ�ǰ���ж� �����ƶ�������ȥ�ƶ�
        if (CanMoving)
        {
            //��ɫ�ƶ��߼�
            this.transform.Translate(moveDir.normalized * speed * Time.deltaTime);
            //�������ת�� ������0 ����ɿ��������ǿ��ת������
            if (moveDir.x > 0)
                roleSprite.flipX = false;
            else if (moveDir.x < 0)
                roleSprite.flipX = true;
        }

        //�Ƿ��ƶ�
        if (moveDir == Vector2.zero)
            ChangeAction(E_Action_Type.Idle);
        else
            ChangeAction(E_Action_Type.Move);

    }

    /// <summary>
    /// �л�����
    /// </summary>
    /// <param name="type"></param>
    protected void ChangeAction(E_Action_Type type)
    {
        switch (type)
        {
            case E_Action_Type.Idle:
                roleAnimator.SetBool("isMoving", false);
                break;
            case E_Action_Type.Move:
                roleAnimator.SetBool("isMoving", true);
                break;
            case E_Action_Type.Jump:
                roleAnimator.SetTrigger("jumpTrigger");
                break;
            case E_Action_Type.JumpAtk:
                roleAnimator.SetTrigger("jumpAtkTrigger");
                break;
            case E_Action_Type.Atk:
                roleAnimator.SetTrigger("atkTrigger");
                break;
            case E_Action_Type.FootAtk:
                roleAnimator.SetTrigger("footAtkTrigger");
                break;
            case E_Action_Type.Hit:
                roleAnimator.SetBool("isHit", true);
                break;
            case E_Action_Type.HitFly:
                roleAnimator.SetBool("isHitFly", true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// �л�����Ƿ��ڵ���״̬
    /// </summary>
    /// <param name="isGround"></param>
    protected void ChangeRoleIsGround(bool isGround)
    {
        roleAnimator.SetBool("isGround", isGround);
    }

    protected bool CanMoving
    {
        get
        {
            //ȥ�õ�״̬���������״̬ �ж��Ƿ��ǿ����ƶ���״̬
            //AnimatorStateInfo layerInfo1 = roleAnimator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo layerInfo2 = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (IsAtkState ||
                layerInfo2.IsName("Defend") ||
                layerInfo2.IsName("Hit") ||
                layerInfo2.IsName("HitFly") ||
                layerInfo2.IsName("Pickup") ||
                layerInfo2.IsName("Throw"))
                return false;

            return true;
        }
    }

    /// <summary>
    /// �Ƿ��ǹ���״̬
    /// </summary>
    protected bool IsAtkState
    {
        get
        {
            AnimatorStateInfo layerInfo = roleAnimator.GetCurrentAnimatorStateInfo(1);
            if (layerInfo.IsName("Atk1") ||
                layerInfo.IsName("Atk2") ||
                layerInfo.IsName("Atk3") ||
                layerInfo.IsName("FootAtk1") ||
                layerInfo.IsName("FootAtk2"))
                return true;
            return false;
        }
    }
}

public enum E_Action_Type
{

    Idle,
    Move,
    Jump,
    /// <summary>
    /// ��Ծ����
    /// </summary>
    JumpAtk,
    Atk,
    /// <summary>
    /// �Ȳ����� ��ӦKick����
    /// </summary>
    FootAtk,
    Hit,
    /// <summary>
    /// ����
    /// </summary>
    HitFly,
    /// <summary>
    /// Ͷ������
    /// </summary>
    Throw,
    Pickup,
    Defend
}