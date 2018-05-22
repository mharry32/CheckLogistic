using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Checklogistics
{
    public class DataKuaiDi100
    {
        public DateTime? time { get; set; }
        public String location { get; set; }
        public String context { get; set; }
        public DateTime? ftime { get; set; }
    }
    public class KuaiDi100Model
    {
        public String message { get; set; }
        public String nu { get; set; }
        public String ischeck { get; set; }
        public String com { get; set; }
        public String updatetime { get; set; }
        public String status { get; set; }
        public String condition { get; set; }
        public List<DataKuaiDi100> data { get; set; }
        public String state { get; set; }
    }
    public class DataAiKuaiDi
    {
        public DateTime? time { get; set; }
        public string content { get; set; }
    }
    public class AiKuaiDiModel
    {
        public String id { get; set; }
        public String name { get; set; }
        public String order { get; set; }
        public String message { get; set; }
        public String errcode { get; set; }
        public long status { get; set; }
        public List<DataAiKuaiDi> data { get; set; }
    }

    public class KuanDi
    {
        public KuanDi()
        {

        }
        static public KuaiDi100Model Query100(string NO, string Type)
        {

            string TypeStr = Type;

            //http://www.kuaidi100.com/query?type=shentong&postid=3300111279603
            string url = "http://www.kuaidi100.com/query?type=" + TypeStr + "&postid=" + NO;
            WebRequest req = WebRequest.Create(url);
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string jsonstr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                resp.Close();
                return JsonConvert.DeserializeObject<KuaiDi100Model>(jsonstr);
            }
            catch
            { return null; }
        }
        static public KuaiDi100Model AiKuaiDiModelToKuaiDi100Model(AiKuaiDiModel aikuaidimodel)
        {
            if (aikuaidimodel == null)
            { return null; }
            KuaiDi100Model mod = new KuaiDi100Model();
            mod.com = aikuaidimodel.name;
            mod.nu = aikuaidimodel.order;
            mod.state = aikuaidimodel.status.ToString();
            mod.data = new List<DataKuaiDi100>();
            if (aikuaidimodel.data != null)
            {
                foreach (DataAiKuaiDi item in aikuaidimodel.data)
                {
                    DataKuaiDi100 d = new DataKuaiDi100();
                    d.context = item.content;
                    d.time = item.time;
                    d.ftime = item.time;
                    mod.data.Add(d);
                }
                DataKuaiDi100[] arr = mod.data.ToArray();
                Array.Reverse(arr);
                mod.data = arr.ToList();
            }
            return mod;
        }
        static public AiKuaiDiModel QueryAiKuaiDi(string NO, string Type)
        {

            string TypeStr = Type;
            string url = "http://www.aikuaidi.cn/query/";
            string inputstr = "order=" + NO + "&id=" + Type;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "post";
            req.Referer = "http://www.aikuaidi.cn/";
            byte[] buff = Encoding.UTF8.GetBytes(inputstr);
            req.ContentLength = buff.Length;
            req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML,like Gecko) Chrome/38.0.2125.111 Safari/537.36";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Accept = "application/json,text/javascript,*/*; q=0.01";
            Stream ms = req.GetRequestStream();
            ms.Write(buff, 0, buff.Length);
            ms.Close();
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                string jsonstr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                resp.Close();
                return JsonConvert.DeserializeObject<AiKuaiDiModel>(jsonstr);
            }
            catch
            { return null; }
        }

        static public KuaiDi100Model Query100ByCp_code(string NO, string cp_code)
        {
            return Query100(NO, getTypeByCp_code(cp_code));
        }
        static public KuaiDi100Model QueryAiKuaiDiByCp_code(string NO, string cp_code)
        {
            AiKuaiDiModel med = QueryAiKuaiDi(NO, getTypeByCp_code(cp_code));
            return AiKuaiDiModelToKuaiDi100Model(med);
        }
        static public string QueryTextByCp_Code(string NO, string cp_code, string kd100oraikd = "kd100")
        {
            string txt = "数据来源:" + kd100oraikd + ";  快递公司" + cp_code + ";物流号" + NO + Environment.NewLine;
            KuaiDi100Model mod = new KuaiDi100Model();
            if (kd100oraikd == "kd100")
            { mod = Query100ByCp_code(NO, cp_code); }
            if (kd100oraikd == "aikd")
            { mod = QueryAiKuaiDiByCp_code(NO, cp_code); }
            if (mod != null && mod.data != null)
            {
                foreach (DataKuaiDi100 d in mod.data)
                {
                    txt += d.time.Value.ToString("yyyy-MM-dd HH:mm:ss") + "||" + d.context + Environment.NewLine;
                }
            }
            return txt;
        }
        static public string getTypeByCp_code(string cp_code)
        {
            string com = "";
            switch (cp_code.ToLower())
            {
                case "sto": com = "shentong"; break;
                case "yto": com = "yuantong"; break;
                case "ems": com = "ems"; break;
                case "eyb": com = "ems"; break;
                case "sf": com = "shunfeng"; break;
                case "zto": com = "zhongtong"; break;
            }
            return com;
        }
    }
}
