using Accord.Controls;
using Accord.IO;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace NNetwork.AI
{
    public class Brain
    {
        double[][] _inputs;
        int[] _outputs;
        IActivationFunction _function;
        ISupervisedLearning _teacher;
        Network _network;

        public void Learn()
        {
            SetValuesToLearn();
            _function = new BipolarSigmoidFunction();

            _network = new ActivationNetwork(_function,
                inputsCount: 2, neuronsCount: new[] { 5, 1 });

            _teacher = new BackPropagationLearning((ActivationNetwork)_network);

            var y = _outputs.ToDouble().ToJagged();
            double error = double.PositiveInfinity;
            double previous;

            do
            {
                previous = error;

                error = _teacher.RunEpoch(_inputs, y);

            } while (Math.Abs(previous - error) < Math.Exp(previous));
        }

        public void SetValuesToLearn()
        {
            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - Yin Yang");

            // Convert the DataTable to input and output vectors
            _inputs = table.ToJagged<double>("X", "Y");
            _outputs = table.Columns["G"].ToArray<int>();
        }

        public void Check()
        {
            int[] answers = _inputs.Apply(_network.Compute).GetColumn(0).Apply(System.Math.Sign);

            // Plot the results
            ScatterplotBox.Show("Expected results", _inputs, _outputs);
            ScatterplotBox.Show("Brain results", _inputs, answers);
        }

        public void Save()
        {
            var pathTeacher = $"{Directory.GetCurrentDirectory()}/teacher";
            var pathNetwork = $"{Directory.GetCurrentDirectory()}/network";
            //_teacher.Save(pathTeacher);
            _network.Save(pathNetwork);
        }

        public void Load()
        {
            var pathTeacher = $"{Directory.GetCurrentDirectory()}/teacher";
            var pathNetwork = $"{Directory.GetCurrentDirectory()}/network";
            _network= Network.Load(pathNetwork);            
        }
    }

    
}
