using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObject : RoleObject
{
    /// <summary>
    /// ����
    /// </summary>
    public override void Atk()
    {
        //�л�����״̬���� ����Ҫ������������ ״̬�����߿���
        ChangeAction(E_Action_Type.Atk);
    }

    /// <summary>
    /// �ƶ�
    /// </summary>
    /// <param name="dir"></param>
    public void Move(Vector2 dir)
    {
        //�ı��ƶ����� �ڸ����Update�߼��� �ͻ�ȥ�����ƶ���ص��߼�
        moveDir = dir;
    }

    /// <summary>
    /// ����
    /// </summary>
    public override void Dead()
    {
        ChangeAction(E_Action_Type.Dead);
    }
}
