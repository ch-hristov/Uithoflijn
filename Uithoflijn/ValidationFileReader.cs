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

            for (int i = 1; i < lines.Length; i++)
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

            foreach (var file in Directory.EnumerateFiles(dir))
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
            From = int.Parse(data[1]);
            To = int.Parse(data[2]);
            PassIn = double.Parse(data[3]);
            PassOut = double.Parse(data[4]);
        }

        public string Stop { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public double PassIn { get; set; }
        public double PassOut { get; set; }
    }
}
