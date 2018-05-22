using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using HtmlAgilityPack;


namespace Checklogistics
{
   public class Util
    {

        public static string GetLogisticCode(string originalName)
        {
            string convertedName = "";
            if (originalName.Contains("德邦"))
            {
                convertedName = "debangwuliu";
            }

            if (originalName.Contains("DHL"))
            {
                convertedName = "dhl";
            }

            if (originalName.Contains("汇通"))
            {
                convertedName = "huitongkuaidi";
            }

            if (originalName.Contains("速尔"))
            {
                convertedName = "suer";
            }

            if (originalName.Contains("天天"))
            {
                convertedName = "tiantian";
            }


            if (originalName.Contains("优速"))
            {
                convertedName = "youshuwuliu";
            }

            if (originalName.Contains("圆通"))
            {
                convertedName = "yuantong";
            }

            if (originalName.Contains("韵达"))
            {
                convertedName = "yunda";
            }

            if (originalName.Contains("中通"))
            {
                convertedName = "zhongtong";
            }
            if (originalName.Contains("中邮"))
            {
                convertedName = "zhongyouwuliu";
            }
            if (originalName.Contains("申通"))
            {
                convertedName = "shentong";
            }
            if (originalName.Contains("EMS"))
            {
                convertedName = "ems";
            }
            if (originalName.Contains("宅急送"))
            {
                convertedName = "zhaijisong";
            }
            if (originalName.Contains("顺丰"))
            {
                convertedName = "shunfeng";
            }

            return convertedName;
        }
    }
}
