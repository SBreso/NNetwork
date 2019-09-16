using Accord.Controls;
using Accord.IO;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace NNetwork.AI
{
    public class Brain
    {
        double[][] _inputs;
        double[][] _coordenates;
        double[][] _outputs;

        IActivationFunction _function;
        ISupervisedLearning _teacher;
        ActivationNetwork _network;

        public void Learn()
        {
            SetValuesToLearn();
            _function = new BipolarSigmoidFunction();

            //_network = new ActivationNetwork(_function,
            //    inputsCount: 4, neuronsCount: new[] { 5, 1 });
            _network = new ActivationNetwork(_function, 4,6,5, 2);
            new NguyenWidrow(_network).Randomize();
            //_teacher = new BackPropagationLearning(_network);
            _teacher = new ParallelResilientBackpropagationLearning(_network);
            //var y = _outputs.ToDouble().ToJagged();
            
            //double error = double.PositiveInfinity;
            double error = 1.0;
            while (error > 1e-2)
            {
                error = _teacher.RunEpoch(_inputs, _outputs);
            }
            //double previous;
            //int epoch = 1;
            //do
            //{
            //    previous = error;

            //    error = _teacher.RunEpoch(_inputs, y);

            //    Console.WriteLine($"EPOCH {epoch}: {Math.Abs(previous - error)} - {Math.Exp(previous)}");

            //} while (Math.Abs(previous - error) < 1e-10 * previous);
        }

        public void SetValuesToLearn()
        {
            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - Circle");

            // Convert the DataTable to input and output vectors
            _inputs = table.ToJagged<double>("X", "Y", "X2", "Y2");
            _coordenates = table.ToJagged<double>("X", "Y");
            _outputs = table.ToJagged<double>("G1","G2");
        }

        public void Check(double x, double y)
        {
            DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - CircleCheck");

            // Convert the DataTable to input and output vectors
            var inputs = table.ToJagged<double>("X", "Y", "X2", "Y2");
            var coordenates = table.ToJagged<double>("X", "Y");
            //var outputs = table.Columns["G"].ToArray<int>();

            var output = _network.Compute(new double[4] { x, y, Math.Pow(x, 2), Math.Pow(y, 2) });

            //int[] answers = inputs.Apply(_network.Compute).GetColumn(0).Apply(System.Math.Sign);

            Console.WriteLine(String.Format(@"{0,-10} {1,-10} {2,-40} {3,-40}","X","Y","In s2","In s5"));
            Console.WriteLine(String.Format(@"{0,-10} {1,-10} {2,-40} {3,-40}", x, y, Math.Round(output[0])==1?"TRUE":"FALSE", Math.Round(output[1]) == 1 ? "TRUE" : "FALSE"));
            //for (int i = 0; i < inputs.Length; i++)
            //{
            //    double[] answer = _network.Compute(inputs[i]);
            //    int actual;
            //    answer.Max();
                
            //    Console.WriteLine($"[{inputs[i][0]}, {inputs[i][1]}] -> [{Math.Round(answer[0])} - {Math.Round(answer[1])}]");
            //    //Console.WriteLine($"[{inputs[i][0]}, {inputs[i][1]}] -> {answers[i]}");
            //}
            // Plot the results
            //ScatterplotBox.Show("Expected results", coordenates, outputs);
            //ScatterplotBox.Show("Brain results", coordenates, answers);
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
            _network = (ActivationNetwork)Network.Load(pathNetwork);
        }
    }


}
