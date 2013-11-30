using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    internal enum LCSLight
    {
        /// <summary>
        /// ����
        /// </summary>
        snuff = 0,
        /// <summary>
        /// ��
        /// </summary>
        down = 1,
        /// <summary>
        /// ��
        /// </summary>
        forbid = 2,
        /// <summary>
        /// ��
        /// </summary>
        rightDown = 3,
        /// <summary>
        /// ��
        /// </summary>
        leftDown = 4
    }

    internal enum DeviceType
    {
        CMS,
        RGS,
        WIS,
        CSLS,
        LCS,
        RMS,
        CCTV,
        FS
    }

    /// <summary>
    /// �ƥ����
    /// </summary>
    enum CategoryType
    {
        /// <summary>
        /// �@��ƥ�
        /// </summary>
        GEN,    //�@��ƥ�
        /// <summary>
        /// �ö�ƥ�
        /// </summary>
        OBS,    //�ö�ƥ�
        /// <summary>
        /// �ѭԨƥ�
        /// </summary>
        WEA,    //�ѭԨƥ�
        /// <summary>
        /// �G�D���q�ƥ�
        /// </summary>
        TUN,    //�G�D���q�ƥ�
        /// <summary>
        /// �ި�ƥ�
        /// </summary>
        RES,    //�ި�ƥ�
        /// <summary>
        /// ��L�ƥ�
        /// </summary>
        OTH,    //��L�ƥ�
    }

    /// <summary>
    /// ����
    /// </summary>
    internal enum Degree
    {
        /// <summary>
        /// �L
        /// </summary>
        N = 0,   //�L
        /// <summary>
        /// �C
        /// </summary>
        L = 1,    //�C
        /// <summary>
        /// ��
        /// </summary>
        M = 2, //��
        /// <summary>
        /// ��
        /// </summary>
        H = 3,  //��
        /// <summary>
        /// �W��
        /// </summary>
        S = 4  //�W��
    }

    /// <summary>
    /// �T�����A
    /// </summary>
    internal enum MegType
    {
        /// <summary>
        /// ĵ�i
        /// </summary>
        A,    //ĵ�i
        /// <summary>
        /// �D�j��
        /// </summary>
        U, //�D�j��
        /// <summary>
        /// �j��
        /// </summary>
        F   //�j��
    }

    public enum EventStatus
    {
        /// <summary>
        /// �T�{��
        /// </summary>
        Enter = 1,              //�T�{
        /// <summary>
        /// ���ݤ�
        /// </summary>
        Wait = 2,               //����
        /// <summary>
        /// ���
        /// </summary>
        Abort = 6,              //���
        /// <summary>
        /// ����
        /// </summary>
        Terminate = 5,          //����
        /// <summary>
        /// ����
        /// </summary>
        End = 4,                //����
        /// <summary>
        /// ���椤
        /// </summary>
        Execute = 3             //����
    }

    public enum Notifier
    {
        /// <summary>
        /// ���q��
        /// </summary>
        ExyTel = 1,     //���q��
        /// <summary>
        /// �M�u�q��
        /// </summary>
        SpyTel = 2,     //�M�u�q��
        /// <summary>
        /// �L�u�q��
        /// </summary>
        Wireless = 3,   //�L�u�q��
        /// <summary>
        /// �����q��
        /// </summary>
        LocalTel = 4,   //�����q��
        /// <summary>
        /// ���
        /// </summary>
        Mobile = 5,     //���
        /// <summary>
        /// ������
        /// </summary>
        Device = 6,     //������
        /// <summary>
        /// �۰ʰ���
        /// </summary>
        Auto = 7        //�۰ʰ���
    }

    /// <summary>
    /// ��s�����p�����
    /// </summary>
    public enum RenewData
    {
        All
    }


    public enum LoginMode
    {
        /// <summary>
        /// �۰�
        /// </summary>
        Auto = 1,       //�۰�
        /// <summary>
        /// �b�۰�
        /// </summary>
        Half = 2,       //�b�۰�
        /// <summary>
        /// ���
        /// </summary>
        Manual = 3      //���
    }
}
