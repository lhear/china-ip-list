using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace china_ip_list
{
    class Program
    {
        public static string cn_ipv4 = "", cn_ipv6 = "";
        static void Main(string[] args)
        {
            string apnic_ip = GetResponse("http://ftp.apnic.net/apnic/stats/apnic/delegated-apnic-latest");
            //string apnic_ip = "apnic|IN|ipv4|103.16.104.0|1024|20130205|allocated\napnic|CN|ipv4|103.16.108.0|65536|20130205|allocated\napnic|ID|ipv4|103.16.112.0|1024|20130205|assigned\napnic|BN|ipv4|103.16.120.0|1024|20130206|assigned\napnic|CN|ipv4|103.16.124.0|1024|20130206|allocated\napnic|AU|ipv4|103.16.128.0|1024|20130206|allocated\napnic|ID|ipv4|103.16.132.0|512|20130206|assigned\n";
            string[] ip_list = apnic_ip.Split(new string[] { "\n" }, StringSplitOptions.None);
            int i = 0, j = 0;
            string save_txt_path = AppContext.BaseDirectory;
            foreach (string per_ip in ip_list)
            {
                if (per_ip.Contains("CN|ipv4|"))
                {
                    string[] ip_information = per_ip.Split('|');
                    string ip = ip_information[3];
                    string ip_mask = Convert.ToString(32 - (Math.Log(Convert.ToInt32(ip_information[4])) / Math.Log(2)));
                    cn_ipv4 += ip + "/" + ip_mask + "\n";
                    i++;
                }
                if (per_ip.Contains("CN|ipv6|"))
                {
                    string[] ip_information = per_ip.Split('|');
                    string ip = ip_information[3];
                    string ip_mask = ip_information[4];
                    cn_ipv6 += ip + "/" + ip_mask + "\n";
                    j++;
                }
            }
            ////Console.Write(cn_ipv4);
            ////Console.Write(cn_ipv6);
            File.WriteAllText(save_txt_path + "cn_ipv4.txt", cn_ipv4);
            File.WriteAllText(save_txt_path + "cn_ipv6.txt", cn_ipv6);
            Console.Write("本次共获取" + i + "条CN IPv4的记录，" + j + "条CN IPv6的记录，文件保存于:" + save_txt_path);
        }

        private static string GetResponse(string url)
        {
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }
    }
}
