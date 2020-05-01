using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.girlsfrontline.demo.Code
{
    /// <summary>
    /// 拒绝冲塔，有你有我
    /// </summary>
    class Filter
    {
        string filePath;

        public Filter(string filePath)
        {
            this.filePath = filePath;
        }

        public bool Wordfilter(string word)
        {
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Rule").Element("wordRule");

            IEnumerable<XElement> en = root.Elements();

            foreach(XElement ele in en)
            {
                if(word.Contains(ele.Value))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Rankfilter(string rank)
        {
            int num = int.Parse(rank);
            XDocument doc = XDocument.Load(filePath);
            XElement root = doc.Element("Rule").Element("rankRule");
            int rankceil = int.Parse(root.Element("rankCeil").Value);
            int rankfloor= int.Parse(root.Element("rankFloor").Value);

            if(num<=rankceil && num>=rankfloor )
            {
                return true;
            }

            return false;
        }

    }
}
