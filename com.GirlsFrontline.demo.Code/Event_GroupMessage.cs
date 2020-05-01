using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using Native.Sdk.Cqp;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
namespace com.girlsfrontline.demo.Code
{
    public class Event_GroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            int gunInArmoy;
            int rand;
            Filter filter = new Filter("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\filterRule.xml");
            CharacterLoder characterLoder = new CharacterLoder("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml");
            gunInArmoy = characterLoder.NumOfCharacter();

            if (e.Message.Text.StartsWith("添加卡片"))
            {
                //添加卡片 姓名 星级 留言

                if (Regex.IsMatch(CQApi.CQDeCode(e.Message.Text),
                    @"^\w+\s[a-zA-z0-9()\-[!.\u4E00-\u9FA5]+\s\d\s[a-zA-z0-9()\-[!.\u4E00-\u9FA5]") == false || filter.Wordfilter(e.Message.Text)==false)
                {
                    e.FromGroup.SendGroupMessage("加入失败");
                    return;
                }

                string[] info =CQApi.CQDeCode(e.Message.Text).Split(new char[] { ' ', ' ', ' ' });
                foreach(string ele in info)
                {
                    if(ele==null)
                    {
                        return;
                    }
                }

                string name = info[1];
                string rank = info[2];
                string message = info[3];



                if(characterLoder.AddCard(name, rank, message) && filter.Rankfilter(rank))
                {
                    e.FromGroup.SendGroupMessage("加入成功");
                }
                else
                {
                    e.FromGroup.SendGroupMessage("加入失败");
                }

            }

            if (e.Message.Text.Equals("抽卡"))
            {
                CharacterRandom characterrandom = new CharacterRandom();
                rand = int.Parse(characterrandom.CardSelect(1, gunInArmoy, "F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml"));

                e.FromGroup.SendGroupMessage("池子存有人形数：",
                    gunInArmoy.ToString(),
                    "\n本次抽取到的人形是：\n",
                    characterLoder.CharacterName(rand),
                    "\n星级：",
                    characterLoder.CharacterRank(rand),
                    "\n",
                    characterLoder.CharacterMsg(rand)
                    );

            }

            if(e.Message.Text.Equals("实验抽卡"))
            {
                e.FromGroup.SendGroupMessage(characterLoder.CharacterName(57));
            }

            if (e.Message.Text.Contains(CQApi.CQCode_At(e.CQApi.GetLoginQQ()).ToString()))
            {
                e.FromGroup.SendGroupMessage("使用格式：\n" +
                    "[抽卡] 抽取一张卡片\n" +
                    "[添加卡片 名称 星级 留言]将卡片放入池子" +
                    "例如：添加卡片 [MG]绍沙 4 您就是这里的指挥官吗？那么，绍沙M1915，向您报到。\n" +
                    "星级范围2-5\n"+
                    "不要添加令人疑惑的信息，qqqxx");
            }

        }

    }
}
