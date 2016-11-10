using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCP_Part_1
{
    public class GVertex{
        public int ID
        {
            get { return ID; }
            private set { ID = value; }
        }
        public GVertex(int id)
        {
            ID = id;
        }
    }
}
