using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.girlsfrontline.demo.Code
{
    class CharacterRandom
    {
        int[] weightOfRank = new int[] { 5, 4, 3 ,3};
        // 2star 3star 4star 5star

        static int GetRandomSeed()
        {
            byte[] bytes = new byte[4];

            System.Security.Cryptography.RNGCryptoServiceProvider rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();

            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int RankSelect()
        {
            int sum = 0;
            int rankResult = 0;
            Random random = new Random(GetRandomSeed());

            for (int i = 0; i < weightOfRank.Length; i++)
            {
                sum += weightOfRank[i];
            }

            int numRand = random.Next(0, sum);
            int sumTemp = 0;

            for (int i = 0; i < 4; i++)
            {
                sumTemp += weightOfRank[i];
                if (numRand <= sumTemp)
                {
                    return (i + 2);
                }
            }

            return (rankResult + 2);
        }

        public string CardSelect(int a, int b, string filePath)
        {
            CharacterLoder cl = new CharacterLoder(filePath);
            int rank = RankSelect();
            IEnumerable<XElement> list = cl.NodesWith("rank", rank);
            IEnumerator<XElement> enumerator = list.GetEnumerator();
            int length = list.Count();
            Random random = new Random(GetRandomSeed());
            int rad = random.Next(1, length);

            for (int i = 0; i <= length; i++)
            {
                enumerator.MoveNext();
                if (i == rad)
                {
                    return enumerator.Current.Attribute("id").Value;
                }

            }

            return "-1";
        }

    }
}
