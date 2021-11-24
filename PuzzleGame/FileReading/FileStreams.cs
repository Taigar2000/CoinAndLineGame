using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FileReading
{
    public class FileStreams
    {
        #region Exception

        [System.Serializable]
        public class CSVException : Exception
        {
            public CSVException() { }
            public CSVException(string message) : base(message) { }
            public CSVException(string message, Exception inner) : base(message, inner) { }
            protected CSVException(
              SerializationInfo info,
              StreamingContext context) : base(info, context) { }
        }

        #endregion

        static KeyValuePair<KeyValuePair<string, string>, int> makePpair(string task, string res, string num)
        {
            int n;
            if (!int.TryParse(num, out n))
                n = -1;
            // Console.WriteLine("\t" + task + " " + res + " " + n);
            return new KeyValuePair<KeyValuePair<string, string>, int>(new KeyValuePair<string, string>(task, res), n);
        }
        
        /// <summary>
        /// Read lines from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="n">Number of readed lines (read all -1)</param>
        /// <param name="encode">Encoding of file</param>
        /// <exception name="CSVException"></exception>
        /// <returns>Array of strings from file</returns>
        public static List<KeyValuePair<KeyValuePair<string, string>, int>> 
            readCsvFile(string path, Encoding encode = null, int n = -1)
        {
            string str = "Problem with reading file";
            bool isread = false;
            if (encode == null) { encode = Encoding.Default; }

            try
            {
                List<KeyValuePair<KeyValuePair<string, string>, int>> res =
                    new List<KeyValuePair<KeyValuePair<string, string>, int>>();
                str = "Problem with data in file";
                isread = true;
                foreach (string i in File.ReadAllLines(path, encode))
                {
                    // Console.WriteLine(i);
                    if(i.Length == 0)
                        continue;
                    Match val = Regex.Match(i,
                        @"^(?<task>[\w\d\+\-\*\/= ]+)[,;](?<result>[\w\d\+\-\*\/= ]+)[,;](?<num>\d+)$");
                    // (?<task>[\w\d\+\-\*\/=]+) - group; "task" - alias for group; "[\w\d\+\-\*\/=]+" - condition for group
                    res.Add(makePpair(val.Groups["task"].Value, val.Groups["result"].Value,
                        val.Groups["num"].Value));
                }

                return res;
            }
            catch (FormatException e)
            {
                throw new CSVException(" because you give wrong value of amount in file", e);
            }
            catch (ArgumentNullException e)
            {
                if(isread)
                    throw new CSVException(" because you give null or incorrect string instead task or result in file", e);
                throw new CSVException(" because you give null or incorrect string instead path", e);
            }
            catch (ArgumentException e)
            {
                throw new CSVException(str + " because you give path in wrong format", e);
            }
            catch (PathTooLongException e)
            {
                throw new CSVException(str + " because given path is too long", e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent directory", e);
            }
            catch (FileNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent file", e);
            }
            catch (IOException e)
            {
                throw new CSVException(str + " because program has error when reading data from file", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new CSVException(str + " because your permissions is insufficient to open this file", e);
            }
            catch (NotSupportedException e)
            {
                throw new CSVException(str + " because stream does not support invoked functionality", e);
            }
            catch (System.Security.SecurityException e)
            {
                throw new CSVException(str + " because program take security error", e);
            }
            catch (Exception e)
            {
                throw new CSVException(str + "Unknown exception", e);
            }
            return null;
        }

        /// <summary>
        /// Read lines from file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="n">Number of readed lines (read all -1)</param>
        /// <param name="encode">Encoding of file</param>
        /// <exception name="CSVException"></exception>
        /// <returns>Array of strings from file</returns>
        public static string[] readFileAllLines(string path, Encoding encode = null, int n = -1)
        {
            string str = "Problem with reading file";
            bool isread = false;
            if (encode == null) { encode = Encoding.Default; }

            try
            {
                str = "Problem with data in file";
                isread = true;
                return File.ReadAllLines(path, encode);
            }
            catch (FormatException e)
            {
                throw new CSVException(" because you give wrong value of amount in file", e);
            }
            catch (ArgumentNullException e)
            {
                if (isread)
                    throw new CSVException(" because you give null or incorrect string instead task or result in file", e);
                throw new CSVException(" because you give null or incorrect string instead path", e);
            }
            catch (ArgumentException e)
            {
                throw new CSVException(str + " because you give path in wrong format", e);
            }
            catch (PathTooLongException e)
            {
                throw new CSVException(str + " because given path is too long", e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent directory", e);
            }
            catch (FileNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent file", e);
            }
            catch (IOException e)
            {
                throw new CSVException(str + " because program has error when reading data from file", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new CSVException(str + " because your permissions is insufficient to open this file", e);
            }
            catch (NotSupportedException e)
            {
                throw new CSVException(str + " because stream does not support invoked functionality", e);
            }
            catch (System.Security.SecurityException e)
            {
                throw new CSVException(str + " because program take security error", e);
            }
            catch (Exception e)
            {
                throw new CSVException(str + "Unknown exception", e);
            }
            return null;
        }

        /// <summary>
        /// Write lines to file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="path">Data to writing to file</param>
        /// <param name="encode">Encoding of file</param>
        public static void writeToFile(string path, string data, Encoding encode = null)
        {
            string str = "Problem with writing to file";
            if (encode == null) { encode = Encoding.Default; }

            try
            {
                File.AppendAllText(path, data, encode);
            }
            catch (FormatException e)
            {
                throw new CSVException(" because you give wrong value of amount in file", e);
            }
            catch (ArgumentNullException e)
            {
                throw new CSVException(" because you give null or incorrect string instead path", e);
            }
            catch (ArgumentException e)
            {
                throw new CSVException(str + " because you give path in wrong format", e);
            }
            catch (PathTooLongException e)
            {
                throw new CSVException(str + " because given path is too long", e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent directory", e);
            }
            catch (FileNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent file", e);
            }
            catch (IOException e)
            {
                throw new CSVException(str + " because program has error when reading data from file", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new CSVException(str + " because your permissions is insufficient to open this file", e);
            }
            catch (NotSupportedException e)
            {
                throw new CSVException(str + " because stream does not support invoked functionality", e);
            }
            catch (System.Security.SecurityException e)
            {
                throw new CSVException(str + " because program take security error", e);
            }
            catch (Exception e)
            {
                throw new CSVException(str + "Unknown exception", e);
            }

            return;
        }

        /// <summary>
        /// Write lines to file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="path">Data to writing to file</param>
        /// <param name="encode">Encoding of file</param>
        public static void writeToNewFile(string path, string data, Encoding encode = null)
        {
            string str = "Problem with writing to file";
            if (encode == null) { encode = Encoding.Default; }

            try
            {
                File.WriteAllText(path, data, encode);
            }
            catch (FormatException e)
            {
                throw new CSVException(" because you give wrong value of amount in file", e);
            }
            catch (ArgumentNullException e)
            {
                throw new CSVException(" because you give null or incorrect string instead path", e);
            }
            catch (ArgumentException e)
            {
                throw new CSVException(str + " because you give path in wrong format", e);
            }
            catch (PathTooLongException e)
            {
                throw new CSVException(str + " because given path is too long", e);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent directory", e);
            }
            catch (FileNotFoundException e)
            {
                throw new CSVException(str + " because you give path to nonexistent file", e);
            }
            catch (IOException e)
            {
                throw new CSVException(str + " because program has error when reading data from file", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new CSVException(str + " because your permissions is insufficient to open this file", e);
            }
            catch (NotSupportedException e)
            {
                throw new CSVException(str + " because stream does not support invoked functionality", e);
            }
            catch (System.Security.SecurityException e)
            {
                throw new CSVException(str + " because program take security error", e);
            }
            catch (Exception e)
            {
                throw new CSVException(str + "Unknown exception", e);
            }

            return;
        }

    }
}