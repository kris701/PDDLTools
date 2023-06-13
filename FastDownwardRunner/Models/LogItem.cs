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
    }
}
