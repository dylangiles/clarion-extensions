using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redcat.TXA
{
    internal class Embed
    {
        public string Name { get; private set; }
        public int Priority { get; private set; }

        public Embed(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Substring(0, Constants.PROPERTY_EMBED.Length) == Constants.PROPERTY_EMBED)
                {
                    Name = lines[i].Substring(Constants.PROPERTY_EMBED.Length).Replace("%",
                        string.Empty);
                }
            }
        }
    }
}
