using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elected
{
    public class Representative
    {
        public string Name { get; set; }
        public string Party { get; set; }
        public string OfficeName { get; set; }
        public string Address { get; set; }
        public List<string> Emails { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public List<string> Urls { get; set; }
        public byte[] PhotoBytes { get; set; }
        public List<Channel> Channels { get; set; }

    }
    public class Channel
    {
        public string Type { get; set; }
        public string Id { get; set; }

        public override string ToString()
        {
            return Type + ": " +  Id;
        }

        public Channel(string type, string id)
        {
            Type = type;
            Id = id;
        }

    }
}