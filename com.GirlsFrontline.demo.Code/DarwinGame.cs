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

        public int SetMemder(string playerid,int[]id)
        {
            XDocument doc = XDocument.Load(userfile);
            XElement root = doc.Element("Userlist");

            XElement user = null;
            foreach(XElement ele in root.Elements()) //检查人员
            {
                if(ele.Attribute("qq").Value==playerid)
                {
                    user = ele;
                    break;
                }
            }
            if(user==null)
            {
                return 0; //无人
            }

            userManager userManager = new userManager(userfile);
            IEnumerable<XElement> cards = userManager.CardOfUser(playerid);

            foreach(var item in id)
            {
                if(item==0)
                {
                    break;
                }

                IEnumerable<XElement> card = from ele in cards
                                             where ele.Attribute("id").Value == item.ToString()
                                             select ele;

                if(card.Count()==0)
                {
                    return 1; //无卡
                }

            }
            

            XElement team = user.Element("team");
            XElement attack = team.Element("attack");

            int sum = 0;

            IEnumerable<XElement> memberslist = team.Elements("member");
            IEnumerator < XElement >member= memberslist.GetEnumerator();
            CharacterLoder characterLoder = new CharacterLoder(gunsfile);
            member.MoveNext();
            foreach(var item in id)
            {
                sum += int.Parse(characterLoder.CharacterAttack(item));
                member.Current.Value = item.ToString();
                member.MoveNext();
            }

            team.Element("attack").Value = sum.ToString();
            doc.Save(userfile);
            return 2;
        }

        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];

            System.Security.Cryptography.RNGCryptoServiceProvider rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();

            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int ResultSelect(int player,int rival)
        {
            int sum = 0;
            int Result = 0;
            Random random = new Random(GetRandomSeed());
            int[] attack = { player, rival };

            for (int i = 0; i < attack.Length; i++)
            {
                sum += attack[i];
            }

            int num = random.Next(0, sum);
            int sumTemp = 0;

            for (int i = 0; i < 2; i++)
            {
                sumTemp += attack[i];
                if (num <= sumTemp)
                {
                    return i;
                }
            }

            return Result;
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

            int res = ResultSelect(playerAttack, rivalAttack);

            if (playerAttack>rivalAttack)
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
