using System.Collections.Generic;
using System.IO;

namespace Uithoflijn
{
    public class ValidationFileReader
    {
        public List<InputRow> ReadFile(string path)
        {
            var lines = File.ReadAllLines(path);
            var results = new List<InputRow>();

            //skip header
            for (var i = 1; i < lines.Length; i++)
                results.Add(new InputRow(lines[i].Split(';')));

            return results;
        }

        /// <summary>
        /// Read all the validation files in a directory
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public List<List<InputRow>> ReadDirectory(string dir)
        {
            var validations = new List<List<InputRow>>();

            //read all csv files for testing
            foreach (var file in Directory.EnumerateFiles(dir, "*.csv"))
                validations.Add(ReadFile(file));

            return validations;
        }

        public static List<List<InputRow>> ReadValidationFolder()
        {
            return new ValidationFileReader().ReadDirectory("validation");
        }
    }

    public class InputRow
    {
        public InputRow(string[] data)
        {
            Stop = data[0];
            Direction = int.Parse(data[1]);
            From = double.Parse(data[2]);
            To = double.Parse(data[3]);
            PassIn = double.Parse(data[4]);
            PassOut = double.Parse(data[5]);
        }

        public int Direction { get; set; }
        public string Stop { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public double PassIn { get; set; }
        public double PassOut { get; set; }
    }
}
