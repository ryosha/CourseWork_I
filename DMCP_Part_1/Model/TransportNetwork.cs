using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;

namespace DMCP_Part_1
{
    public class IntermediateTransportNetEventArgs : EventArgs
    {
        private TransportGraph _flowGraph;
        public TransportGraph FlowGraph
        {
            get { return _flowGraph; }
        }
        private TransportGraph _incrementalGraph;
        public TransportGraph IncrementalGraph
        {
            get { return _incrementalGraph; }
        }

        public IntermediateTransportNetEventArgs(TransportGraph incrementalGraph,TransportGraph flowGraph)
        {
            _flowGraph = flowGraph;
            _incrementalGraph = incrementalGraph;
        }
 

    }
    class TransportNetwork
    {
        public event DrawGraph IntermediateTransportNetResult;
        private const int INFINITY = 9999;

        private int[][] _capacity;//пропускная способность
        private int[][] _costFlow;//стоимость потока
       
        public int EdgesCount{
            get
            {
                int count = 0;
                for (int i = 0; i < _costFlow.Length; ++i)
                    for (int j = 0; j < _costFlow.Length; ++j)
                        if (_costFlow[i][j] != INFINITY)
                            count++;
                return count;
            }
        }
        public TransportNetwork(int[][] capacity, int[][] costFlow)
        {
            _capacity = capacity;
            _costFlow = costFlow;
        }
        //
        public TransportNetwork() { }
        //
        private TransportGraph FormIncrementalGraph(int[][] incrementalNet) {
            TransportGraph intermediateGrap = new TransportGraph(0, 0);
            string edgeID;
            List<GVertex> vertexList = new List<GVertex>();
            for (int i = 0; i < incrementalNet.Length; ++i)
            {
                vertexList.Add(new GVertex(i));
            }
            foreach (GVertex i in vertexList)
                intermediateGrap.AddVertex(i);

            List<GEdge> edgesList = new List<GEdge>();
            for (int i = 0; i < incrementalNet.Length; ++i)
                for (int j = 0; j < incrementalNet.Length; ++j)
                    if (incrementalNet[i][j] != INFINITY)
                    {
                        edgesList.Add(new GEdge(vertexList[i], vertexList[j], -1, incrementalNet[i][j]));
                        edgesList.Add(new GEdge(vertexList[j], vertexList[i], -1, incrementalNet[j][i]));
                    }
            foreach (GEdge i in edgesList)
                intermediateGrap.AddEdge(i);

            return intermediateGrap;
        }
        private TransportGraph FormFlowGraph(int[][] flowNet,int[][] incrementalGraph,int currnetFlow,int deltaFlow)
        {
            TransportGraph intermediateGrap = new TransportGraph(currnetFlow,deltaFlow);
            string edgeID;
            List<GVertex> vertexList = new List<GVertex>();
            for (int i = 0; i < flowNet.Length; ++i)
            {
                vertexList.Add(new GVertex(i));
            }
            foreach (GVertex i in vertexList)
                intermediateGrap.AddVertex(i);

            List<GEdge> edgesList = new List<GEdge>();
            for (int i = 0; i < flowNet.Length; ++i)
                for (int j = 0; j < flowNet.Length; ++j)
                    if (incrementalGraph[i][j] != INFINITY)
                    {
                        edgesList.Add(new GEdge(vertexList[i], vertexList[j],flowNet[i][j],-1));
                    }
            foreach (GEdge i in edgesList)
                intermediateGrap.AddEdge(i);

            return intermediateGrap;
        }

        public int[][] FlowMinCost(int givenFlow)
        {
            int graphSize = _capacity.Length;
            int currentFlow = 0;
            int[][] flowsGraph = GetStartFlowsGraph();
            int[][] incrementalGraph = new int[graphSize][];
            for (int i = 0; i < graphSize; ++i)
            {
                incrementalGraph[i] = new int[graphSize];
            }
            int[] layouts = new int[graphSize];
            int[] T = new int[graphSize];
            int[] vect = new int[graphSize];

            while (givenFlow > currentFlow)
            {
                UpdateIncrementalGraph(flowsGraph, incrementalGraph);
                for (int i = 1; i < graphSize; ++i)
                    layouts[i] = INFINITY;
                layouts[0] = 0;
                bool exit = true;
                while (exit)
                {
                    exit = false;
                    for (int j = 1; j < graphSize; j++)
                    {
                        for (int i = 0; i < graphSize; ++i)
                        {
                            if (layouts[j] > layouts[i] + incrementalGraph[i][j])
                            {
                                layouts[j] = layouts[i] + incrementalGraph[i][j];
                                T[j] = i;
                                exit = true;
                            }
                        }
                    }
                }
                for (int i = 0; i < graphSize; i++)
                    vect[i] = INFINITY;
                int m = graphSize - 1;
                while (m != 0)
                {
                    if (T[m] < m)
                    {
                        vect[m] = _capacity[T[m]][m] - flowsGraph[T[m]][m];
                    }
                    else
                    {
                        vect[m] = flowsGraph[m][T[m]];
                    }
                    m = T[m];
                }
                int deltaF = Min(vect);
                deltaF = Math.Min(deltaF, givenFlow - currentFlow);
                m = graphSize - 1;
                while (m != 0)
                {
                    if (T[m] < m)
                        flowsGraph[T[m]][m] = flowsGraph[T[m]][m] + deltaF;
                    else
                        flowsGraph[m][T[m]] = flowsGraph[m][T[m]] - deltaF;
                    m = T[m];
                }
                currentFlow = currentFlow + deltaF;


                var args = new IntermediateTransportNetEventArgs(
                    FormIncrementalGraph(incrementalGraph),
                    FormFlowGraph(flowsGraph,incrementalGraph,currentFlow,deltaF)
                    );
                IntermediateTransportNetResult(this, args);
            }
            return flowsGraph;
        }


        private int Min(int[] vect)
        {
            int min = INFINITY;
            for (int i = 0; i < vect.Length; ++i)
            {
                if (min > vect[i])
                {
                    min = vect[i];
                }
            }
            return min;

        }
        private void UpdateIncrementalGraph(int[][] currentFlowsGraph, int[][] incrementalGraph)
        {

            for (int i = 0; i < currentFlowsGraph.Length - 1; ++i)
            {
                for (int j = i + 1; j < currentFlowsGraph.Length; ++j)
                {
                    if (_capacity[i][j] > currentFlowsGraph[i][j])
                    {
                        incrementalGraph[i][j] = _costFlow[i][j];
                    }
                    else
                    {
                        incrementalGraph[i][j] = INFINITY;
                    }
                    if (currentFlowsGraph[i][j] > 0)
                    {
                        incrementalGraph[j][i] = (-_costFlow[i][j]);
                    }
                    else
                    {
                        incrementalGraph[j][i] = INFINITY;
                    }
                }
            }
        }
        private int[][] GetStartFlowsGraph()
        {
            int[][] flowsGraph = new int[_capacity.Length][];

            for (int i = 0; i < _capacity.Length; ++i)
            {
                flowsGraph[i] = new int[_capacity.Length];
            }
            for (int i = 0; i < _capacity.Length; ++i)
            {
                for (int j = 0; j < flowsGraph.Length; ++j)
                {
                    flowsGraph[i][j] = 0;
                }
            }
            return flowsGraph;

        }
     

    }
}
