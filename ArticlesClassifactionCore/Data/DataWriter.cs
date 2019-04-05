using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Features.FeatureExtractors;
using ArticlesClassifactionCore.Metrics;

namespace ArticlesClassifactionCore.Data
{
    public class DataWriter
    {
        public static void SaveResultsToFile(string filename, ResultData data)
        {
            using (StreamWriter output = new StreamWriter(filename, true))
            {
                output.WriteLine("-------------------------------------------");
                output.WriteLine($"Category: {data.Category}");
                output.WriteLine();
                output.WriteLine("Tags:");
                data.Tags.ForEach(output.WriteLine);
                output.WriteLine($"Percent of learning data: {data.TrainingPercentage}");
                output.WriteLine($"Key words extractor: {data.KeyWordsExtractor}");
                output.WriteLine($"Key words Per Tag: {data.KeyWordsPerTag}");
                output.WriteLine("Feature Extractors:");
                data.FeatureExtractors.ForEach(output.WriteLine);
                output.WriteLine($"Metric: {data.Metric}");
                output.WriteLine($"Parameter K: {data.K}");
                output.WriteLine($"Cold start data: {data.ColdStartData}");
                output.WriteLine("RESULTS");
                output.WriteLine("Confusion Matrix:");
                output.Write(data.Matrix);
                output.WriteLine("Precision(1) and recall(2):");
                int maxLength = data.PrecisionRecall.Keys.Select(c => c.Length).Max();
                foreach (var row in data.PrecisionRecall)
                {
                    output.Write(row.Key.PadLeft(maxLength));
                    output.Write($"  {Math.Round(row.Value.Item1 * 100, 2)}%  {Math.Round(row.Value.Item2 * 100, 2)}%\n");
                }
                output.WriteLine($"Overall accuracy:  {Math.Round(data.Accuracy * 100, 2)}%");
                output.WriteLine($"Time: {data.Time}");
                output.WriteLine("-------------------------------------------");


            }
        }
    }

    public class ResultData
    {
        public Dictionary<string, (double, double)> PrecisionRecall { get; set; }
        public double Accuracy { get; set; }
        public string Matrix { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public double TrainingPercentage { get; set; }
        public double Time { get; set; }
        public string KeyWordsExtractor { get; set; }
        public int KeyWordsPerTag { get; set; }
        public List<IFeatureExtractor> FeatureExtractors { get; set; }
        public IMetric Metric { get; set; }
        public int K { get; set; }
        public int ColdStartData { get; set; }
    }
}
