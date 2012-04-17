using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using grid_games;
using System.Xml.Serialization;
using System.IO;

namespace AgentBenchmark
{
    class Program
    {
        static GridGameExperiment experiment;

        static void Main(string[] args)
        {
            GridGameParameters gg = GridGameParameters.GetParameters(args);
            if (gg == null)
                return;

            using (TextWriter writer = new StreamWriter(gg.ConfigPath))
            {
                XmlSerializer ser = new XmlSerializer(typeof(GridGameParameters));
                ser.Serialize(writer, gg);
            }

            experiment = new GridGameExperiment(gg.ConfigPath);
            

        }
    }
}
