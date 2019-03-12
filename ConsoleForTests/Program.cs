using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore;

namespace ConsoleForTests
{
    class Program
    {
        static void Main(string[] args)
        {
            SgmParser parser=new SgmParser();
            var models=parser.ReadAllSgmFromDirectory().Where(n=>n.Tags.ContainsKey("orgs")).ToList();
        }
    }
}
