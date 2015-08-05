using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway
{
    public class CompactFormatReader
    {

    }

    public class CompactFormatWriter
    {
        private StringBuilder builder = new StringBuilder();
        private bool delimiterNeeded = false;
        private string specialChars = "\"\\,()|";

        public void WriteString(string str)
        {
            if (delimiterNeeded)
                builder.Append(',');
            builder.Append(str);
            delimiterNeeded = true;
        }

        public void WriteInt(int num)
        {
            WriteString(num.ToString());
        }

        public void WriteQuotedString(string str)
        {
            foreach (char chr in specialChars)
                str = str.Replace(chr.ToString(), "\\" + chr);
            WriteString("\"" + str + "\"");
        }

        public void NextItem()
        {
            builder.Append('|');
            delimiterNeeded = false;
        }

        public void OpenParens()
        {
            builder.Append("(");
            delimiterNeeded = false;
        }

        public void  CloseParens()
        {
            builder.Append(")");
            delimiterNeeded = false;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
