using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.girlsfrontline.demo.Code
{
    class userManager
    {
        string filePath;
        public userManager(string filePath)
        {
            this.filePath = filePath;
        }

        public int DeleteCardById(string id,string user)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Userlist");

            XElement keyid = null;
            XElement keyuser = null;

            foreach(XElement ele in root.Elements())
            {
                if(ele.Attribute("qq").Value==user)
                {
                    keyuser = ele;
                }
            }

            if(keyuser==null)
            {
                return 0; //查无此人
            }

            foreach(XElement ele in keyuser.Elements())
            {
                if(ele.Attribute("id").Value==id)
                {
                    keyid = ele;
                }
            }

            if(keyid==null)
            {
                return 1; //查无此卡
            }

            keyid.Remove();
            doc.Save(filePath);
            return 2;
        }

        public XElement CardOfUser(string qq)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Userlist");

            XElement ans = null;

            foreach(XElement ele in root.Elements())
            {
                if(ele.Attribute("qq").Value==qq)
                {
                    ans = ele;
                }
            }

            return ans;
        }

        public void AddCardToUser(int id, string qq)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Userlist");

            XElement userqq = null;

            foreach(XElement ele in root.Elements())
            {
                if(ele.Attribute("qq").Value==qq)
                {
                    userqq = ele;
                    break;
                }
            }

            if (userqq == null)
            {
                XElement user = new XElement("user");
                XElement card = new XElement("card");
                user.SetAttributeValue("qq", qq);
                card.SetAttributeValue("id", id);
                card.SetElementValue("num", 1);
                user.Add(card);
                root.Add(user);
                doc.Save(filePath);
            }
            else
            {
                XElement card = new XElement("card");
                foreach (XElement ele in userqq.Elements())
                {
                    if(ele.Name=="card"&& ele.Attribute("id").Value==id.ToString())
                    {
                        ele.Element("num").Value = (int.Parse(ele.Element("num").Value) + 1).ToString();
                        doc.Save(filePath);
                        return;
                    }
                }
                card.SetAttributeValue("id", id);
                card.SetElementValue("num", 1);

                userqq.Add(card);
                doc.Save(filePath);
            }
        }

    }
}
