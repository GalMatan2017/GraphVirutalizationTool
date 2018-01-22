using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphVirtualizationTool.Model
{
    class AdjacencyMatrix : FileHandlerInterface
    {
        public List<List<T>> ParseFile<T>(string filename)
        {
            try
            {
                List<List<bool>> matrix = new List<List<bool>>();

                StreamReader reader = File.OpenText(filename);

                string line;
                int columns = 0,
                    rows = 0;

                //read line
                while ((line = reader.ReadLine()) != null)
                {
                    //split by whitespace
                    string[] items = line.Split(',');
                    //convert to integers
                    List<bool> convertedItems = new List<bool>();
                    foreach (var integer in items)
                    {
                        int item;
                        if (GraphGlobalVariables.getInstance().TryParseInt32(integer, out item) != -1 && (item == 0 || item == 1))
                            convertedItems.Add(item == 0 ? false : true);
                    }
                    ++rows;
                    if (!(items.Length > 1))
                        throw new Exception($"Row {rows} is corrupted!");

                    if (rows == 1)
                    {
                        matrix.Add(convertedItems.ToList());
                        //columns constant integer is initiliazed
                        columns = convertedItems.Count;
                    }
                    else if (convertedItems.Count == columns)
                    {
                        matrix.Add(convertedItems.ToList());
                    }
                    else
                    {
                        throw new Exception($"Row #{rows} is corrupted!");
                    }
                }
                if (columns != rows)
                {
                    if (rows < columns)
                        throw new Exception("columns is bigger than rows");
                    else
                        throw new Exception("rows is bigger than columns");
                }
                reader.Close();
                return (List<List<T>>)Convert.ChangeType(matrix, typeof(List<List<T>>)) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (List<List<T>>)Convert.ChangeType(new List<List<bool>>(), typeof(List<List<T>>)) ;
            }
        }
    }
}
