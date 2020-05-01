using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace com.girlsfrontline.demo.Code
{ 
    /// <summary>
    /// 发起对战
    /// 如果死亡，将失去所有卡牌，自己卡牌的前5个战力最高将被对方获取
    /// 如果成功，将获得1000pt来购买卡牌
    /// 
    /// 被对战者只能自动进入对战，如果失败将丢失100pt
    /// 如果成功将获得对方前5战力的卡
    /// </summary>

    //达尔文游戏 为您呈现

    class DarwinGame
    {
        string userfile, gunsfile;

        public DarwinGame(string userfile,string gunsfile)
        {
            this.userfile = userfile;
            this.gunsfile = gunsfile;
        }

        public bool IsPlayer(string qqid)
        {
            XDocument doc = XDocument.Load(userfile);
            XElement root = doc.Element("Userlist");

            foreach(XElement ele in root.Elements())
            {
                if(ele.Attribute("qq").Value==qqid)
                {
                    return true;
                }
            }

            return false;
        }

        public int fight(string playerid,string rivalid)
        {
            XDocument doc = XDocument.Load(userfile);
            XElement root = doc.Element("Userlist");

            int playerAttack=0, rivalAttack=0;

            foreach(XElement ele in root.Elements())
            {
                XElement attack = ele.Element("team").Element("attack");
                if(ele.Attribute("qq").Value==playerid)
                {
                    playerAttack = int.Parse(attack.Value);
                }

                if (ele.Attribute("qq").Value == rivalid)
                {
                   rivalAttack = int.Parse(attack.Value);
                }
            }

            if(playerAttack>rivalAttack)
            {
                return 1; //攻击方获胜
            }
            else if(playerAttack<rivalAttack)
            {
                return 2; //被攻击方获胜
            }
            else
            {
                return 3; //平局
            }

        }
    }
}
