using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace com.girlsfrontline.demo.Code
{
    class XMLFileReader
    {
        protected string FilePath;

        public XMLFileReader(string filePath)
        {
            this.FilePath = filePath;
        }

        public int sum(string rt, string key)
        {
            XDocument gunList = XDocument.Load(FilePath);
            XElement root = gunList.Element(rt);

            IEnumerable<XElement> gunEnum = from ele in root.Elements()
                                            where ele.Value == key
                                            select ele;

            return root.Elements().Count();
        }

        public string Reader(int id, string name)
        {
            XDocument gunList = XDocument.Load(FilePath);
            XElement root = gunList.Element("armory");
            IEnumerable<XElement> gunEnum = root.Elements();

            foreach (XElement ele in gunEnum)
            {
                if (ele.Attribute("id").Value == id.ToString())
                {
                    IEnumerable<XElement> desEnum = ele.Elements();

                    foreach (XElement e in desEnum)
                    {
                        if (e.Name == name)
                        {
                            return e.Value;
                        }
                    }
                }
            }

            return "ERROR";
        }

    }

    class CharacterLoder
    {
        string filePath;

        public CharacterLoder(string filePath)
        {
            this.filePath = filePath;
        }

        public string CharacterAttack(int id)
        {
            XMLFileReader reader = new XMLFileReader(filePath);
            if(id==0)
            {
                return "0";
            }
            return reader.Reader(id, "attack");
        }

        public string CharacterRank(int id)
        {
            XMLFileReader XMLFileReader = new XMLFileReader(filePath);
            return XMLFileReader.Reader(id, "rank");
        }

        public string CharacterId(string name)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("armory");

            foreach(XElement ele in root.Descendants())
            {
                if(ele.Name=="name" && ele.Value==name)
                {
                    return ele.Parent.Attribute("id").Value;
                }
            }

            return "ERROR";
        }

        public string CharacterMsg(int id)
        {
            XMLFileReader XFR = new XMLFileReader(filePath);
            return XFR.Reader(id, "message");
        }

        public string CharacterName(int id)
        {
            XMLFileReader XFR = new XMLFileReader(filePath);
            return XFR.Reader(id, "name");
        }

        public int AddCard(string name, string rank, string message)
        {
            XDocument doc = XDocument.Load(filePath);
            IEnumerable<XElement> namelist = from ele in doc.Descendants()
                                          where ele.Name == "name" && ele.Value == name
                                          select ele;

            if(namelist.Count()!=0)
            {
                return 0;
            }

            XElement root = doc.Element("armory");
            int id = NumOfCharacter();
            XElement add = new XElement("gun");
            add.SetAttributeValue("id", (id+1).ToString());
            add.SetElementValue("rank", rank);
            add.SetElementValue("name", name);
            add.SetElementValue("message", message);

            Random random = new Random();
            int attack = random.Next(int.Parse(rank) * 1000, (int.Parse(rank) + 1) * 1000);
            add.SetElementValue("attack",attack);

            root.Add(add);
            doc.Save(filePath);

            return attack;
        }

        public IEnumerable<XElement> NodesWith(string name, int key)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("armory");

            IEnumerable<XElement> allranks = from ele in root.Descendants()
                                             where ele.Name == name && ele.Value == key.ToString()
                                             select ele.Parent;

            return allranks;
        }

        public int NumOfCharacter()
        {
            XMLFileReader XFR = new XMLFileReader(filePath);
            return XFR.sum("armory", "gun") + 1;
        }

    }
}
