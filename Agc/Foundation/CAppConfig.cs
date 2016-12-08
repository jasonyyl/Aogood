using System;
using System.Collections.Generic;
using System.Xml;

namespace Aogood.Foundation
{
    public class CAppConfig
    {

        public Dictionary<string, string> AppConfig { get; set; }
        public CAppConfig()
        {
            if (AppConfig == null)
                AppConfig = new Dictionary<string, string>();
         
        }
        public CAppConfig(string path)
            : this()
        {
            LoadConfig(path);
        }

        public void LoadAppConfig(string path)
        {
            LoadConfig(path);
        }
        private bool LoadConfig(string path)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                XmlNodeList nodeList = xmlDoc.SelectNodes("/Configs/Config");
                int length = nodeList.Count;
                AppConfig.Clear();
                for (int i = 0; i < length; i++)
                {
                    XmlElement ele = nodeList[i] as XmlElement;
                    AppConfig.Add(ele.GetAttribute("name"), nodeList[i].InnerText);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
