using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Win32;
using WHC.OrderWater.Commons;
using WHC.Pager.WinControl;

namespace Checklogistics
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string currentOriginFileName = "";
        private DataTable currentOutPutDataTable = null;
        private DataTable currentInPutDataTable = null;

        private void Bt_ImportOrigin_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter= "Excel文件(*.xls,*.xlsx) |*.xls;*.xlsx"
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                Hint.Text = "导入中，请稍候...";
                ThreadPool.QueueUserWorkItem((w) =>
                {
                    string strError = "";

                    var Importresult = AsposeExcelTools.ExcelFileToDataTable(openFileDialog.FileName, out currentInPutDataTable, out strError);
                    if (Importresult == true)
                    {
                        Update(() => { Hint.Text = "导入成功，请执行第二步";
                            Bt_StartCheck.IsEnabled = true;
                        });
                    }
                    else
                    {
                        Update(() =>
                        {
                            Hint.Text = "导入失败。请重试。"+strError;
                        });
                    }
                });
              
            }
        }

        private void Bt_StartCheck_OnClick(object sender, RoutedEventArgs e)
        {
            Bt_ImportOrigin.IsEnabled = false;
            Bt_StartCheck.IsEnabled = false;
            currentOutPutDataTable = DataTableHelper.CreateTable("物流单号,物流公司,状态");
            Hint.Text = "正在处理第0个,共" + (currentInPutDataTable.Rows.Count-1) + "个";
            ThreadPool.QueueUserWorkItem((w) =>
            {
                bool isFirst = true;
                int current = 1;
                int total = (currentInPutDataTable.Rows.Count - 1);
                foreach (DataRow r in currentInPutDataTable.Rows)
                {
                    if (isFirst == true)
                    {
                        isFirst = false;
                        continue;
                    }

                    Update(() =>
                    {
                        Hint.Text = "正在处理第"+current+"个,共" + total + "个";

                    });
                    var lnum = r[0].ToString();
                    var lname = r[1].ToString();
                    var finalresult = "无物流";
                    var kresult = KuanDi.Query100(lnum, Util.GetLogisticCode(lname));
                    if (kresult != null)
                    {
                        if (kresult.status == "200")
                        {
                            finalresult = "";
                            foreach (var d in kresult.data)
                            {
                                finalresult += d.ftime.ToString()+" " + d.context + "\n";
                            }
                        }
                    }

                    DataRow newRow = currentOutPutDataTable.NewRow();
                    newRow["物流单号"] = lnum;
                    newRow["物流公司"] = lname;
                    newRow["状态"] = finalresult;
                    currentOutPutDataTable.Rows.Add(newRow);
                    current++;
                }

                Update(() =>
                {
                    Hint.Text = "处理完毕，请执行第三步";
                    Bt_Export.IsEnabled = true;
                });

            });
        }

        private void Bt_Export_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel文件(*.xls) |*.xls";  //设置文件类型         
            sfd.FilterIndex = 1; //设置默认文件类型显示顺序            
            sfd.RestoreDirectory = true;//保存对话框是否记忆上次打开的目录 
            //点了保存按钮进入 
            if (sfd.ShowDialog() == true)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径 

               
                   NPOIHelper.ExportExcel(currentOutPutDataTable, localFilePath);
                   Hint.Text = "保存完成，可以继续执行第一步";
                        Bt_Export.IsEnabled = false;
                        Bt_ImportOrigin.IsEnabled = true;
              
            }


        }



        private void Update(Action updateAction)
        {
            this.Dispatcher.BeginInvoke(updateAction);
        }
    }
}
