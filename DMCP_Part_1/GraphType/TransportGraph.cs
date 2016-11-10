using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;


namespace DMCP_Part_1
{
    public class TransportGraph:AdjacencyGraph<GVertex,GEdge>{
        private int _flow;
        public int Flow
        {
            get { return _flow; }
        }
        private int _deltaFlow;
        public int DeltaFlow
        {
            get { return _deltaFlow; }
        }
        public TransportGraph(int flow, int deltaFlow)
        {
            _flow = flow;
            _deltaFlow = deltaFlow;
        }
    }
}
