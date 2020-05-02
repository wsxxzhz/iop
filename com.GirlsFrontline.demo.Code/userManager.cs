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

        public string TeamAttack(string playerid)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Userlist");

            foreach (var ele in root.Elements())
            {
                if (ele.Attribute("qq").Value == playerid)
                {
                    return ele.Element("team").Element("attack").Value;
                }
            }

            return "ERROR";
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

            IEnumerable<XElement> cards = from ele in keyuser.Elements()
                                          where ele.Name == "card"
                                          select ele;

            foreach(XElement ele in cards)
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

        public IEnumerable<XElement> TeamOfUser(string qq)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Userlist");

            XElement ans = null;

            foreach (XElement ele in root.Elements())
            {
                if (ele.Attribute("qq").Value == qq)
                {
                    ans = ele;
                }
            }

            if (ans == null)
            {
                return null;
            }


            IEnumerable<XElement> members = from ele in ans.Element("team").Elements()
                                            where ele.Name == "member"
                                            select ele;

            return members;
        }

        public IEnumerable<XElement> CardOfUser(string qq)
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

            if(ans==null)
            {
                return null;
            }

            IEnumerable<XElement> cards = from ele in ans.Elements()
                                          where ele.Name == "card"
                                          select ele;

            return cards;
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
                XElement team = new XElement("team");
                user.SetAttributeValue("qq", qq);

                card.SetAttributeValue("id", id);
                card.SetElementValue("num", 1);

                user.Add(card);

                team.SetElementValue("attack", 0);
                for(int i=0; i<5;i++)
                {
                    XElement mem = new XElement("member");
                    mem.Value = "0";
                    team.Add(mem);
                }
                user.Add(team);

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
