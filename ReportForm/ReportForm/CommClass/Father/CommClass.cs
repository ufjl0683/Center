using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;


namespace ReportForm.CommClass.Father
{
    public enum RptHander
    {
        //���D������
        �@�ѥ�q���, //ctrlRPT_DATA_00
        ��������q��ưO������, //ctrlRPT_DATA_01
        ���������D�ϥβv�Ψ����Z����,//ctrlRPT_DATA_02
        �@������q��ưO������,//ctrlRPT_DATA_03
        �@�������D�ϥβv�Ψ����Z����,//ctrlRPT_DATA_04
        �{�I�t�v�լd��q��ưO������,//ctrlRPT_DATA_06
        VD�ԲӰO������,//ctrlRPT_DATA_24
        VD�`�p�O������,//ctrlRPT_DATA_25
        �p�ɥ�q��ưO������,//ctrlRPT_HDA_01
        �p�ɥ�q�y�q�έp����̤���ɬq�J��,//ctrlRPT_HDA_03
        �p�ɥ�q�����t�ײέp����,//ctrlRPT_HDA_05
        �`�D�����C���q�q�έp����,//ctrlRPT_HDA_11

        //�L�D������
        �D�u�����C���q�q�έp����,//ctrlRPT_HDA_10
        ���ϥ`�D�����q�q�έp����,//ctrlRPT_HDA_12
        ���ϥD�u�����q�q�έp����,//ctrlRPT_HDA_13
        ���ϥD�u�p�ɸ��q�����t�ײέp����,//ctrlRPT_HDA_14
        ��T�i�ܼлx�Y�ɸ�Ƴ���,//ctrlRPT_MON_01
        �]�ƪ��A�Y�ɺʵ�����,//ctrlRPT_MON_07
        ���q�ö몬�p�@��������,//ctrlRPT_HDA_35



        //�L�D������,��������
        �ާ@�O������,//ctrlRPT_OPR1_01
        �{���׺ݳ]�ƹB�@�O������,//ctrlRPT_OPR2_06
        �w�ɤ��O������,//ctrlRPT_OPR2_07
        �{���׺ݳ]�ƪ��A�O������,//ctrlRPT_STA_01
    }

    internal enum EngRptHander
    {
        //���D������
        RPT_DATA_00,
        RPT_DATA_01,
        RPT_DATA_02,
        RPT_DATA_03,
        RPT_DATA_04,
        RPT_DATA_06,
        RPT_DATA_24,
        RPT_DATA_25,
        RPT_HDA_01,
        RPT_HDA_03,
        RPT_HDA_05,
        RPT_HDA_11,

        //�L�D������
        RPT_HDA_10,
        RPT_HDA_12,
        RPT_HDA_13,
        RPT_HDA_14,
        RPT_MON_01,
        RPT_MON_07,

        //�L�D������,��������
        RPT_OPR1_01,
        RPT_OPR2_06,
        RPT_OPR2_07,
        RPT_STA_01,
        RPT_HDA_35
    }

    public enum HasMain
    {
        //���D������
        �@�ѥ�q���, //ctrlRPT_DATA_00
        ��������q��ưO������, //ctrlRPT_DATA_01
        ���������D�ϥβv�Ψ����Z����,//ctrlRPT_DATA_02
        �@������q��ưO������,//ctrlRPT_DATA_03
        �@�������D�ϥβv�Ψ����Z����,//ctrlRPT_DATA_04
        �{�I�t�v�լd��q��ưO������,//ctrlRPT_DATA_06
        VD�ԲӰO������,//ctrlRPT_DATA_24
        VD�`�p�O������,//ctrlRPT_DATA_25
        �p�ɥ�q��ưO������,//ctrlRPT_HDA_01
        �p�ɥ�q�y�q�έp����̤���ɬq�J��,//ctrlRPT_HDA_03
        �p�ɥ�q�����t�ײέp����,//ctrlRPT_HDA_05
        �`�D�����C���q�q�έp����,//ctrlRPT_HDA_11
    }

    public enum HasKind
    {
        //�L�D������,��������
        �ާ@�O������,//ctrlRPT_OPR1_01
        �{���׺ݳ]�ƹB�@�O������,//ctrlRPT_OPR2_06
        �w�ɤ��O������,//ctrlRPT_OPR2_07
        �{���׺ݳ]�ƪ��A�O������//ctrlRPT_STA_01
    }

    public enum NonKind
    {
        //�L�D������
        �D�u�����C���q�q�έp����,//ctrlRPT_HDA_10
        ���ϥ`�D�����q�q�έp����,//ctrlRPT_HDA_12
        ���ϥD�u�����q�q�έp����,//ctrlRPT_HDA_13
        ���ϥD�u�p�ɸ��q�����t�ײέp����,//ctrlRPT_HDA_14
        ��T�i�ܼлx�Y�ɸ�Ƴ���,//ctrlRPT_MON_01
        �]�ƪ��A�Y�ɺʵ�����,//ctrlRPT_MON_07
        //�C��w�ɸ��p�s�D�Z,
        ���q�ö몬�p�@��������,//ctrlRPT_HDA_35
    }
    internal enum myTable
    {
        tblDevice,
        tblList
    }
    internal enum myColumn
    {
        Devicename,
        RoadInfo1,
        RoadInfo2
    }

    internal static class Comm
    {
        static public XRControlStyle GetXRControlStyle()
        {
            XRControlStyle xrControlStyle = new DevExpress.XtraReports.UI.XRControlStyle();

            // �e����l��
            xrControlStyle.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(239)), ((System.Byte)(243)), ((System.Byte)(250)));
            xrControlStyle.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(199)), ((System.Byte)(209)), ((System.Byte)(228)));
            xrControlStyle.Borders = ((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
                | DevExpress.XtraPrinting.BorderSide.Bottom);
            xrControlStyle.Font = new System.Drawing.Font("�з���", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
            xrControlStyle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            return xrControlStyle;
        }

        #region ==== �إ����p�� DataSet ====
        static public System.Data.DataSet BuildDataSet(bool isMainTable, System.Data.DataTable dt, System.Data.DataTable dtDev)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            if (isMainTable)
            {
                System.Data.DataTable mydt = new System.Data.DataTable();
                System.Data.DataTable mydtDev = new System.Data.DataTable();

                mydt = dt.Copy();
                mydt.TableName = myTable.tblList.ToString();

                mydtDev = dtDev.Copy();
                mydtDev.TableName = myTable.tblDevice.ToString();

                ds.Tables.Add(mydtDev); // �Ĥ@�� DataTable�A�O����
                ds.Tables.Add(mydt);    // �ĤG�� DataTable�A�O�Ĥl

                // ���p�� DataSet
                System.Data.DataRelation Rel = new System.Data.DataRelation("REL", ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.Devicename.ToString()], ds.Tables[myTable.tblList.ToString()].Columns[myColumn.Devicename.ToString()]);
                ds.Relations.Add(Rel);
            }
            else
            {
                ds.Tables.Add(dt.Copy());
            }
            return ds;
        }
        #endregion ==== �إ����p�� DataSet ====

        #region ==== �O�_���D������ ====
        /// <summary>
        /// �O�_���D������
        /// </summary>
        /// <param name="sReportName">����W��</param>
        /// <returns>true:�O���D������</returns>
        static public bool IsMainTable(string sReportName)
        {
            System.Collections.Generic.List<string> reportNameList = new System.Collections.Generic.List<string>();

            reportNameList.AddRange(Enum.GetNames(typeof(HasMain)));
            foreach (string s in reportNameList)
            {
                if (sReportName == s) return true;
            }

            return false;
        }
        #endregion ==== �O�_���D������ ====


        //xlblMemo  ChangeMemo
        static public string ChangeMemo(string RptName)
        {
            Hashtable Ht = new Hashtable();
            string[] ChgRpt = Enum.GetNames(typeof(RptHander));
            string[] EngRpt = Enum.GetNames(typeof(EngRptHander));
            for (int i = 0; i < ChgRpt.Length; i++)
            {
                Ht.Add(ChgRpt[i], EngRpt[i]);
            }

            return (string)Ht[RptName];
        }
    }
}
