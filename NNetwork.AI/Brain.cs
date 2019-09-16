using Accord.IO;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NNetwork.AI
{
    public class Brain
    {
        private string _name;
        private string _pathNetwork => $"{Directory.GetCurrentDirectory()}/{this._name}";
        private double[][] _inputs;
        private int _inputsCount;
        private double[][] _outputs;
        private int[] _neuronsCount;
        private double _error;
        private IActivationFunction _activationFunction;
        private ActivationNetwork _activationNetwork;
        private ISupervisedLearning _teacher;

        public Brain(string name)
        {
            this._name = name;
        }

        public Brain(string name
            , double[][] inputs
            , double[][] outputs
            , int[] neuronsCount
            , IActivationFunction activationFunction
            , int tol)
        {
            this.CheckArgs(name, inputs, outputs, neuronsCount, tol);
            this._name = name;
            this._inputs = inputs;
            this._inputsCount = this._inputs[0].Length;
            this._outputs = outputs;
            this._neuronsCount = neuronsCount;
            this._error = Math.Exp(tol);
            this._activationFunction = activationFunction;
        }

        private void CheckArgs(string name, double[][] inputs, double[][] outputs, int[] neuronsCount, int tol)
        {
            if (String.IsNullOrEmpty(name)
                || inputs.GetLength(0) == 0
                || outputs.GetLength(0) == 0
                || neuronsCount.Length == 0
                || ((IEnumerable<int>)neuronsCount).Where(x => x <= 0).Count() > 0
                || neuronsCount.Last() != outputs[0].Length
                || tol >= 0)
            {
                
                var param = String.IsNullOrEmpty(name) ? "Name is empty"
                : inputs.GetLength(0) == 0 ? "Inputs is empty"
                : outputs.GetLength(0) == 0 ? "Outputs is empty"
                : neuronsCount.Length == 0 ? "NeuronsCount is empty"
                : ((IEnumerable<int>)neuronsCount).Where(x => x <= 0).Count() > 0 ? "There is a layer without neurons"
                : neuronsCount.Last() != outputs[0].Length ? "Last layer must have the same neurons that outputs length"
                : "Tolerance must be less than 0;";
                throw new ArgumentException(param);
            }
        }

        public async Task Run()
        {
            var isLoaded = this.Load();
            if (!isLoaded)
            {
                this.InitBrain();
                await this.Learn();
                this.Save();
            }
        }

        private void InitBrain()
        {
            this._activationNetwork = new ActivationNetwork(this._activationFunction, this._inputsCount, this._neuronsCount);
            new NguyenWidrow(this._activationNetwork).Randomize();
            this._teacher = new ParallelResilientBackpropagationLearning(this._activationNetwork);
        }

        public async Task Learn()
        {
            await Task.Run(() => 
            {
                double error = 1.0;
                while (error > this._error)
                {
                    error = _teacher.RunEpoch(_inputs, _outputs);
                }
            });
            
        }

        public double[][] Check(double[][] inputsToCheck)
        {
            //DataTable table = new ExcelReader("examples.xls").GetWorksheet("Classification - CircleCheck");

            // Convert the DataTable to input and output vectors
            //var inputs = table.ToJagged<double>("X", "Y", "X2", "Y2");
            //var coordenates = table.ToJagged<double>("X", "Y");
            //var outputs = table.Columns["G"].ToArray<int>();

            //var output = this._activationNetwork.Compute(new double[4] { x, y, Math.Pow(x, 2), Math.Pow(y, 2) });

            return inputsToCheck.Apply(this._activationNetwork.Compute);

            //Console.WriteLine(String.Format(@"{0,-10} {1,-10} {2,-40} {3,-40}", "X", "Y", "In s2", "In s5"));
            //Console.WriteLine(String.Format(@"{0,-10} {1,-10} {2,-40} {3,-40}", x, y, Math.Round(output[0]) == 1 ? "TRUE" : "FALSE", Math.Round(output[1]) == 1 ? "TRUE" : "FALSE"));
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
            this._activationNetwork.Save(this._pathNetwork);
        }

        public bool Load()
        {
            try
            {
                this._activationNetwork = (ActivationNetwork)Network.Load(this._pathNetwork);
                return this._activationNetwork != null;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public static bool ActivationNetworkExists(string name)
        {
            try
            {
                var pathNetwork = $"{Directory.GetCurrentDirectory()}/{name}";
                var activationNetwork = (ActivationNetwork)Network.Load(pathNetwork);
                return activationNetwork != null;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }


}
