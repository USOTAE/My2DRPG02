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
        //��ɫ�ƶ��߼�
        this.transform.Translate(moveDir.normalized * speed * Time.deltaTime);
        //�������ת�� ������0 ����ɿ��������ǿ��ת������
        if (moveDir.x > 0)
        {
            roleSprite.flipX = false;
        }
        else if (moveDir.x < 0)
        {
            roleSprite.flipX = true;
        }

        //�Ƿ��ƶ�
        if(moveDir == Vector2.zero)
        {
            ChangeAction(E_Action_Type.Idle);
        }
        else
        {
            ChangeAction(E_Action_Type.Move);
        }
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
                break;
            case E_Action_Type.JumpAtk:
                break;
            case E_Action_Type.Atk1:
                break;
            case E_Action_Type.Atk2:
                break;
            case E_Action_Type.Atk3:
                break;
            case E_Action_Type.FootAtk1:
                break;
            case E_Action_Type.FootAtk2:
                break;
            case E_Action_Type.Hit:
                break;
            case E_Action_Type.HitFly:
                break;
            case E_Action_Type.Throw:
                break;
            case E_Action_Type.Pickup:
                break;
            case E_Action_Type.Defend:
                break;
            default:
                break;
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
    Atk1,
    Atk2,
    Atk3,
    /// <summary>
    /// �Ȳ����� ��ӦKick����
    /// </summary>
    FootAtk1,
    FootAtk2,
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