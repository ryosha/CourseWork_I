using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using GraphSharp.Controls;

namespace DMCP_Part_1
{
    public delegate void DrawGraph(object sender, IntermediateTransportNetEventArgs args);

    class MainWindowViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TransportNetwork _currentTransportNet;
        public TransportNetwork CurrentTransportNet {
            get { return _currentTransportNet; }
            set
            {
                _currentTransportNet = value;
                NotifyPropertyChanged("Graph");
            }
        }

        private List<TransportGraph> _incrementalGraphs;
        private List<TransportGraph> _flowGraphs;


        public MainWindowViewModel()
        {
            _currentTransportNet = new TransportNetwork();
            _currentTransportNet.IntermediateTransportNetResult += new DrawGraph(AddIntermediateGraph);
        }

        public void AddIntermediateGraph(object sender, IntermediateTransportNetEventArgs args)
        {

            _incrementalGraphs.Add(args.IncrementalGraph);
            _flowGraphs.Add(args.FlowGraph);
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
