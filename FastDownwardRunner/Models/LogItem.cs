using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDownwardRunner.Models
{
    public class LogItem
    {
        public enum ItemType { None, Log, Error }
        public string Content { get; }
        public ItemType Type { get; }
        public DateTime Time { get; }

        public LogItem(string content, ItemType type)
        {
            Content = content;
            Time = DateTime.Now;
            Type = type;
        }

        public override string ToString()
        {
            if (Type == ItemType.Error)
                return $"[{Time}] (ERR) {Content}";
            else if (Type == ItemType.Log)
                return $"[{Time}]       {Content}";
            else if (Type == ItemType.None)
                return $"[{Time}] (???) {Content}";

            return base.ToString();
        }
    }
}
