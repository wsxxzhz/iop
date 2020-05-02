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

                string[] info = CQApi.CQDeCode(e.Message.Text).Split(new char[] { ' ' },options:StringSplitOptions.RemoveEmptyEntries);
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

                int ans = characterLoder.AddCard(name, rank, message);

                if (ans!=0 && filter.Rankfilter(rank))
                {
                    e.FromGroup.SendGroupMessage("加入成功，效能赋值：",ans);
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
                    characterLoder.CharacterMsg(rand),
                    "\n战斗效能：",
                    characterLoder.CharacterAttack(rand)
                    ); ;

                userManager.AddCardToUser(rand, e.FromQQ.Id.ToString());

            }

            if (e.Message.Text.Equals("读取数据"))
            {
                e.FromGroup.SendGroupMessage("获取：",
                    e.FromGroup.GetGroupMemberInfo(1256893006).Nick
                    );

            }

            if(e.Message.Text=="我的梯队")
            {
                DarwinGame darwinGame = new DarwinGame("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\UserInfo.xml",
                    "F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml");

                if(darwinGame.IsPlayer(e.FromQQ.Id.ToString())==false)
                {
                    e.FromGroup.SendGroupMessage("玩家不存在");
                    return;
                }

                StringBuilder ans = new StringBuilder();
                IEnumerable<XElement> members = userManager.TeamOfUser(e.FromQQ.Id.ToString());

                ans.Append("您的梯队：\n");

                foreach(var ele in members)
                {
                    if(int.Parse(ele.Value)==0)
                        {
                        ans.Append("N/A");
                    }
                    else
                    {
                        ans.Append(characterLoder.CharacterName(int.Parse(ele.Value)));
                        ans.Append(' ');
                        ans.Append("等级");
                        ans.Append(characterLoder.CharacterRank(int.Parse(ele.Value)));
                        ans.Append(' ');
                        ans.Append("战斗效能");
                        ans.Append(characterLoder.CharacterAttack(int.Parse(ele.Value)));
                    }

                    ans.Append('\n');

                }

                ans.Append("战斗效能总和");
                ans.Append(userManager.TeamAttack(e.FromQQ.Id.ToString()));
                e.FromGroup.SendGroupMessage(ans);
            }

            if (e.Message.Text == "我的卡牌")
            {
                StringBuilder ans = new StringBuilder();
                IEnumerable<XElement> userinfo = userManager.CardOfUser(e.FromQQ.Id.ToString());

                if(userinfo==null ||userinfo.Elements().Count()==0)
                {
                    e.FromGroup.SendGroupMessage("您暂无卡牌");
                    return;
                }

                ans.Append("您拥有：\n");

                foreach(XElement ele in userinfo)
                {
                    ans.Append("ID: ");
                    ans.Append(ele.Attribute("id").Value);
                    ans.Append(' ');
                    ans.Append(characterLoder.CharacterName(int.Parse(ele.Attribute("id").Value)));
                    ans.Append(' ');
                    ans.Append("等级");
                    ans.Append(characterLoder.CharacterRank(int.Parse(ele.Attribute("id").Value)));
                    ans.Append(' ');
                    ans.Append("战斗效能");
                    ans.Append(characterLoder.CharacterAttack(int.Parse(ele.Attribute("id").Value)));
                    ans.Append(' ');
                    ans.Append("数量");
                    ans.Append(ele.Element("num").Value);
                    ans.Append("\n");
                }

                e.FromGroup.SendGroupMessage(ans);

            }

            if(e.Message.Text.StartsWith("删除卡牌"))
            {
                int start= CQApi.CQDeCode(e.Message.Text).IndexOf(' ');
                string id = CQApi.CQDeCode(e.Message.Text).Substring(start+1);
                int flag=
                    userManager.DeleteCardById(id, e.FromQQ.Id.ToString());

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
                int result = darwinGame.fight(e.FromQQ.Id.ToString(), rivalqq);

                if (result == 3)
                {
                    e.FromGroup.SendGroupMessage("平局");
                }
                else if(result  == 1)
                {
                    e.FromGroup.SendGroupMessage(playername, "获胜");
                }
                else if (result == 2)
                {
                    e.FromGroup.SendGroupMessage(rivalname, "获胜");
                }
            }

            if(e.Message.Text.StartsWith("设置梯队"))
            {
                int[] id = {0,0,0,0,0 };
                char[] space = { ' ' };
                string[] res = e.Message.Text.Split(space, options: StringSplitOptions.RemoveEmptyEntries);
                
                if(res.Length>6)
                {
                    e.FromGroup.SendGroupMessage("最多只能容许5个成员");
                    return;
                }

                for(int i=1;i<res.Length;i++)
                {
                    for(int j=i+1;j<res.Length;j++)
                    {
                        if(res[i]==res[j])
                        {
                            e.FromGroup.SendGroupMessage("一个队伍中不能有重复角色");
                            return;
                        }
                    }
                }

                for(int i=1;i<res.Length;i++)
                {
                    id[i-1] = int.Parse(res[i]);
                }

                foreach(var item in id)
                {
                    if(item<0 || item>characterLoder.NumOfCharacter())
                    {
                        e.FromGroup.SendGroupMessage("存在非法id");
                        return;
                    }
                }

                DarwinGame darwinGame = new DarwinGame("F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\UserInfo.xml", 
                    "F:\\CoolQAir\\dev\\com.girlsfrontline.demo\\Guns.xml");

                switch(darwinGame.SetMemder(e.FromQQ.Id.ToString(), id))
                {
                    case 0:
                        e.FromGroup.SendGroupMessage("玩家不存在");
                        return;
                    case 1:
                        e.FromGroup.SendGroupMessage("设置的卡牌中有不存在的卡牌");
                        return;
                    case 2:
                        StringBuilder ans = new StringBuilder();
                        ans.Append("设置完毕，当前战斗效能");
                        ans.Append(userManager.TeamAttack(e.FromQQ.Id.ToString()));
                        e.FromGroup.SendGroupMessage(ans);
                        return;
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
                    "[删除卡牌 id]删除卡片\n" +
                    "例如：删除卡牌 抽卡机\n" +
                    "[发起对战 @某人]向某人发起对战\n" +
                    "[设置梯队 id id id id id]设置一个梯队\n" +
                    "[我的梯队]查看自己的梯队");
            }

        }

    }
}
