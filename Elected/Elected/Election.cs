using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elected
{
    public class Election
    {
        public string id { get; set; }
        public string name { get; set; }
        public string date { get; set; }

    public override string ToString()
    {
            return name + "\n" + date;
    }
    }

}
