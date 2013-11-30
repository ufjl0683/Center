using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace ReportForm.CommClass.Father
{
    public interface I_Report
    {
        int Width { get;set;}
        int Height { get;set;}
        int ReportColCnt { get;set;}
        int TitleCnt { get;set;}
        int NonShow { get;set;}
    }
}
