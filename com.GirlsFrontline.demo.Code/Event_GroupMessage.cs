using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using Native.Sdk.Cqp;
using System.Text.RegularExpressions;
using Native.Sdk.Cqp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace com.girlsfrontline.demo.Code
{
    public class Event_GroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            int gunInArmoy;
            int rand;
            userManager userManager = new userManager("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\UserInfo.xml");
            Filter filter = new Filter("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\filterRule.xml");
            CharacterLoder characterLoder = new CharacterLoder("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml");
            gunInArmoy = characterLoder.NumOfCharacter();

            if (e.Message.Text.StartsWith("添加卡牌"))
            {
                //添加卡片 姓名 星级 留言

                if (Regex.IsMatch(CQApi.CQDeCode(e.Message.Text),
                    @"^\w+\s[a-zA-z0-9()\-[!.\u4E00-\u9FA5]+\s\d\s[a-zA-z0-9()\-[!.\u4E00-\u9FA5]") == false || filter.Wordfilter(e.Message.Text) == false)
                {
                    e.FromGroup.SendGroupMessage("加入失败");
                    return;
                }

                string[] info = CQApi.CQDeCode(e.Message.Text).Split(new char[] { ' ', ' ', ' ' });
                foreach (string ele in info)
                {
                    if (ele == null)
                    {
                        return;
                    }
                }

                string name = info[1];
                string rank = info[2];
                string message = info[3];

                if (characterLoder.AddCard(name, rank, message) && filter.Rankfilter(rank))
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

                
                e.FromGroup.SendGroupMessage(
                    "池子存有人形数：",
                    gunInArmoy.ToString(),
                    "\n本次抽取到的人形是：\n",
                    characterLoder.CharacterName(rand),
                    "\n星级：",
                    characterLoder.CharacterRank(rand),
                    "\n",
                    characterLoder.CharacterMsg(rand)
                    ); ;

                userManager.AddCardToUser(rand, e.FromQQ.Id.ToString());

            }

            if (e.Message.Text.Equals("读取数据"))
            {
                e.FromGroup.SendGroupMessage("获取：",
                    e.FromGroup.GetGroupMemberInfo(1256893006).Nick
                    );

            }

            if (e.Message.Text == "我的卡牌")
            {
                StringBuilder ans = new StringBuilder();
                XElement userinfo = userManager.CardOfUser(e.FromQQ.Id.ToString());

                if(userinfo==null ||userinfo.Elements().Count()==0)
                {
                    e.FromGroup.SendGroupMessage("您暂无卡牌");
                    return;
                }

                ans.Append("您拥有：");

                foreach(XElement ele in userinfo.Elements())
                {
                    ans.Append(characterLoder.CharacterName(int.Parse(ele.Attribute("id").Value)));
                    ans.Append(' ');
                    ans.Append("数量：");
                    ans.Append(ele.Element("num").Value);
                    ans.Append("\n");
                }

                e.FromGroup.SendGroupMessage(ans);

            }

            if(e.Message.Text.StartsWith("删除卡牌"))
            {
                int start= CQApi.CQDeCode(e.Message.Text).IndexOf(' ');
                string name = CQApi.CQDeCode(e.Message.Text).Substring(start+1);
                int flag=
                    userManager.DeleteCardById(characterLoder.CharacterId(name), e.FromQQ.Id.ToString());

                switch (flag)
                {
                    case 0:
                        e.FromGroup.SendGroupMessage("你现在空无一卡");
                        return;
                    case 1:
                        e.FromGroup.SendGroupMessage("你没有这张卡");
                        return;
                    case 2:
                        e.FromGroup.SendGroupMessage("删除成功");
                        return;
                }

            }

            if(e.Message.Text.StartsWith("发起对战"))
            {
                GroupMemberInfo playerinfo = e.FromGroup.GetGroupMemberInfo(e.FromQQ.Id);
                string playername = playerinfo.Nick;
                
                DarwinGame darwinGame = new DarwinGame("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\UserInfo.xml",
                    "F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml");

                string rivalqq="N/A";

                foreach(var cqcode in e.Message.CQCodes.FindAll(c => c.Function == CQFunction.At))
                {
                    foreach(var item in cqcode.Items)
                    {
                        rivalqq = item.Value;
                    }
                }

                if(rivalqq=="N/A")
                {
                    e.FromGroup.SendGroupMessage("请重新@你的对手");
                    return;
                }
                //GroupMemberInfo rivalinfo = e.FromGroup.GetGroupMemberInfo(long.Parse(rivalqq));

                GroupMemberInfo rivalinfo = e.FromGroup.GetGroupMemberInfo(long.Parse(rivalqq));
                
                string rivalname = rivalinfo.Nick;

                if(darwinGame.IsPlayer(e.FromQQ.Id.ToString())==false
                    ||darwinGame.IsPlayer(rivalqq)==false 
                    ||e.FromQQ.Id.ToString()==rivalqq
                    ||CQApi.CQCode_At(e.CQApi.GetLoginQQ()).ToString()==rivalqq)
                {
                    e.FromGroup.SendGroupMessage("对战不成立");
                    return;
                }

                e.FromGroup.SendGroupMessage(playername , "与" + rivalname , "开始对战！");

                if(darwinGame.fight(e.FromQQ.Id.ToString(),rivalqq)==3)
                {
                    e.FromGroup.SendGroupMessage("平局");
                    return;
                }
                else if(darwinGame.fight(e.FromQQ.Id.ToString(), rivalqq) == 1)
                {
                    e.FromGroup.SendGroupMessage(playername, "获胜");
                }
                else if (darwinGame.fight(e.FromQQ.Id.ToString(), rivalqq) == 2)
                {
                    e.FromGroup.SendGroupMessage(rivalname, "获胜");
                }
            }


            if (e.Message.Text.Contains(CQApi.CQCode_At(e.CQApi.GetLoginQQ()).ToString()))
            {
                e.FromGroup.SendGroupMessage("使用格式：\n" +
                    "[抽卡] 抽取一张卡片\n" +
                    "[添加卡片 名称 星级 留言]将卡片放入池子\n" +
                    "例如：\n" +
                    "添加卡片 绍沙 4 绍沙M1915，向您报到。\n" +
                    "星级范围2-5\n"+
                    "[我的卡牌] 查看拥有的卡牌\n" +
                    "[删除卡牌 名称]删除卡片\n" +
                    "例如：删除卡牌 抽卡机\n" +
                    "[发起对战 @某人]向某人发起对战\n");
            }

        }

    }
}
