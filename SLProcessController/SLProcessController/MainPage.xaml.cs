using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using  SLProcessController.ProcessService;

namespace SLProcessController
{
    public partial class MainPage : UserControl
    {
        System.Windows.Threading.DispatcherTimer  tmr30sec;
       // HostInfo[] hostinfos;
        System.Collections.Generic.Dictionary<string, ProcessInfoCtl> dict = new Dictionary<string, ProcessInfoCtl>();
      System.Collections.ObjectModel.ObservableCollection<HostInfo> hostinfos;

      bool isLoaded = false;
       
        public MainPage()
        {
            InitializeComponent();
            LoadAllInfo();
         
            tmr30sec = new System.Windows.Threading.DispatcherTimer() { Interval = new TimeSpan(0, 0, 10) };
            tmr30sec.Tick += new EventHandler(tmr30sec_Tick);
          
      
          
        }

        void tmr30sec_Tick(object sender, EventArgs e)
        {

            FetchPorcessInfo();
            FetchProcessDbQueue();
            //throw new NotImplementedException();
        }

        void FetchProcessDbQueue()
        {
            if (dict.Count == 0)
                return;
            foreach (ProcessInfoCtl infoctl in dict.Values.ToArray<ProcessInfoCtl>())
            {
                ProcessInfo info = (infoctl.DataContext as ProcessInfo);

                SLProcessController.ProcessService.ServiceSoapClient client = new ServiceSoapClient();

                client.getDbQueueCntCompleted+= (s,a)=>
                {
                    if(a.Error==null)
                    info.DataQueueCnt=a.Result;
                };

                try
                {
                    client.getDbQueueCntAsync(info.MFCC_TYPE, info.HostIP, info.ConsolePort, info.MFCC_TYPE.ToUpper() == "HOST" ? true : false);
                }
                catch
                {
                    throw new Exception(info.ProcessName + ", get Dbqueuecnt failed!");
                }
            }

        }
        void FetchPorcessInfo()
        {
            if (loadHostCnt == 0 || hostinfos.Count == 0 || loadHostCnt != hostinfos.Count)
                return;
            if (this.accordion.SelectedItem == null)
                return;
            foreach (HostInfo hinfo in hostinfos)
            {
                //if ((this.accordion.SelectedItem as AccordionItem).Tag != hinfo)
                //    continue;

                ProcessService.ServiceSoapClient client = new ProcessService.ServiceSoapClient();

                client.GetProcessInfoCompleted += (s, a) =>
                {
                    try
                    {
                        if (a.Result == null || a.Error!=null )
                            return;
                        foreach (ProcessService.ProcessInfo info in a.Result)
                        {

                            (dict[info.ProcessName].DataContext as ProcessInfo).Mermory = info.Mermory;
                            (dict[info.ProcessName].DataContext as ProcessInfo).State = info.State;
                            (dict[info.ProcessName].DataContext as ProcessInfo).IsAlive = info.IsAlive;

                            dict[info.ProcessName].RefreshState();
                        }
                    }
                    catch (Exception ex)
                    {
                      //  MessageBox.Show(ex.Message);
                    }

                };

                client.GetProcessInfoAsync(hinfo.IP);

            }
        }
     
        void LoadAllInfo()
        {
            ProcessService.ServiceSoapClient client = new ProcessService.ServiceSoapClient();
            client.getHostInfosCompleted += new EventHandler<ProcessService.getHostInfosCompletedEventArgs>(client_getHostInfosCompleted);

            client.getHostInfosAsync();
           
        }

        int loadHostCnt = 0;
        void client_getHostInfosCompleted(object sender, ProcessService.getHostInfosCompletedEventArgs e)
        {
         
            hostinfos = e.Result;
         
            foreach (SLProcessController.ProcessService.HostInfo hi in e.Result)
            {
                ProcessService.ServiceSoapClient client = new ProcessService.ServiceSoapClient();
                AccordionHeader header=new AccordionHeader(){ DataContext=hi };
                header.OnChangeHostAllState += new OnHostChangeAllStateHandler(header_OnChangeHostAllState);
                AccordionItem item = new AccordionItem() { Header = header, Content = new ListBox() { Width = this.ActualWidth } };
                   
               
                item.Tag = hi;
                accordion.Items.Add(item);
              

                client.GetProcessInfoCompleted += (o, a) =>
                {
                   
                    
                        loadHostCnt++;

                        if (e.Error != null)
                        {
                          //  MessageBox.Show(e.Error.Message);
                            return;
                        }

                            if (a.Result != null)
                                foreach (ProcessService.ProcessInfo info in a.Result)
                                {
                                    ProcessInfoCtl ctl = new ProcessInfoCtl();
                                    ctl.Tag=info;
                                    ctl.Name = info.ProcessName;
                                    dict.Add(info.ProcessName, ctl);
                                    if (info.ProcessName.ToUpper() == "HOST")
                                    {
                                        ctl.SetProgressMaxValue(10000);
                                        ctl.ProgressBar.SetBinding(ProgressBar.ValueProperty, new System.Windows.Data.Binding() { Path =new PropertyPath( "DataQueueCnt"), Converter = new HostQueueCntProgressConverter() });
                                       // ctl.ProgressBar.GetBindingExpression(ProgressBar.ValueProperty).ParentBinding.Converter = new HostQueueCntProgressConverter();
                                    }
                                    else
                                        ctl.ProgressBar.SetBinding(ProgressBar.ValueProperty, new System.Windows.Data.Binding() { Path = new PropertyPath("DataQueueCnt"), Converter = new MFCCQueueCntProcessConverter() });
                                       // ctl.ProgressBar.GetBindingExpression(ProgressBar.ValueProperty).ParentBinding.Converter = new MFCCQueueCntProcessConverter();
                                
                                }
                            if (loadHostCnt == hostinfos.Count)
                            {
                                for (int i = 0; i < dict.Count; i++)
                                {    ProcessInfoCtl ctl;
                                    ctl= dict.Values.ToArray<ProcessInfoCtl>()[i];
                                   
                                    ProcessInfo info = ctl.Tag as ProcessInfo;

                                    foreach (AccordionItem aitem in accordion.Items)
                                    {
                                        //if(aitem.Content==null)
                                        //  aitem.Content = new ListBox() { Width = this.ActualWidth };
                                        if ((aitem.Tag as HostInfo).IP == info.HostIP)
                                        {
                                            (aitem.Content as ListBox).Items.Add(ctl);
                                            ctl.OnChangeState += new OnChangeStateHandler(ctl_OnChangeState);
                                            ctl.setDataContext(info);
                                            break;
                                        }
                                    }
                                  
                                }

                              

                                tmr30sec.Start();

                            }
                       
                 
                 //   item.IsEnabled = true;
                  
                 
                  //  lock(client)
                  //  System.Threading.Monitor.PulseAll(client);

                };
             
                client.GetProcessInfoAsync(hi.IP);
                this.accordion.SelectedIndex = 1;

            }

        }

        void header_OnChangeHostAllState(HostInfo hostinfo, bool isAllPlay)
        {
            //throw new NotImplementedException();
            if (!isAllPlay)
            {
                ConfirmChild child = new ConfirmChild("TCS", "確定要關閉所有" + hostinfo.HostName + "的程序?");
                child.Closed += (s, a) =>
                    {
                        if (child.DialogResult == true)
                        {
                            //  bool res;
                            //  VisualStateManager.GoToState(this,"Stop",true);
                            for (int i = 0; i < dict.Values.Count; i++)
                            {
                                ProcessInfoCtl ctl;
                                ctl = dict.Values.ToArray<ProcessInfoCtl>()[i];
                                if ((ctl.DataContext as ProcessInfo).HostIP == hostinfo.IP)
                                {
                                    VisualStateManager.GoToState(ctl, "Stop", true);

                                    new ProcessService.ServiceSoapClient().ChangeProcessStateAsync(hostinfo.IP, (ctl.DataContext as ProcessInfo).ProcessName,false);
                                }
                            }

                        }

                    };
                child.Show();

            }
            else
            {

                for (int i = 0; i < dict.Values.Count; i++)
                {
                    ProcessInfoCtl ctl;
                    ctl = dict.Values.ToArray<ProcessInfoCtl>()[i];
                    if ((ctl.DataContext as ProcessInfo).HostIP == hostinfo.IP)
                    {
                        VisualStateManager.GoToState(ctl, "Normal", true);
                        new ProcessService.ServiceSoapClient().ChangeProcessStateAsync(hostinfo.IP, (ctl.DataContext as ProcessInfo).ProcessName, true);
                    }
                }
            }
        }

        void ctl_OnChangeState(ProcessInfoCtl ctl,ProcessInfo  info, bool bPlay)
        {
            //throw new NotImplementedException();

            if (!bPlay)
            {
                ConfirmChild child = new ConfirmChild("TCS", "確定要停止" + info.ProcessName + "?");
                child.Closed += (s, a) =>
                    {
                        if (child.DialogResult == true)
                        {
                            ProcessService.ServiceSoapClient client = new ServiceSoapClient();
                            client.ChangeProcessStateAsync(info.HostIP, info.ProcessName, false);

                            VisualStateManager.GoToState(ctl, "Stop", true);
                        }
                    };
                child.Show();
            }
            else
            {
                new ServiceSoapClient().ChangeProcessStateAsync(info.HostIP, info.ProcessName, true);
                VisualStateManager.GoToState(ctl, "Normal",true);
            }

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (AccordionItem item in accordion.Items)
               ( item.Content as ListBox).Width = this.ActualWidth;
        }
    }
}
