using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphVirtualizationTool.Model
{
    class AdjacencyList : FileHandlerInterface
    {
        public List<List<T>> ParseFile<T>(string filename)
        {
            try
            {
                //return value
                List<List<int>> list = new List<List<int>>();
                //open file
                StreamReader reader = File.OpenText(filename);
                string line;
                int columns = 0,
                    rows = 0;
                //read line
                while ((line = reader.ReadLine()) != null)
                {
                    //split by comma
                    string[] items = line.Split(':', ',');
                    //split by whitespace
                    //convert to integers
                    List<int> convertedItems = new List<int>();
                    foreach (var integer in items)
                    {
                        int item;
                        if (GraphGlobalVariables.getInstance().TryParseInt32(integer, out item) != -1)
                            convertedItems.Add(item);
                    }
                    ++rows;
                    if (!(items.Length > 1))
                        throw new Exception($"Row {rows} is corrupted!");
                    //convert to integers
                    if (rows == 1)
                    {
                        list.Add(convertedItems.ToList());
                        //columns constant integer is initiliazed
                        columns = convertedItems.Count;
                    }
                    else
                    {
                        list.Add(convertedItems.ToList());
                    }
                }
                reader.Close();
                return   (List<List<T>>)Convert.ChangeType(list, typeof(List<List<T>>));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (List < List<T>>)Convert.ChangeType(new List<List<int>>(), typeof(List<List<T>>));
            }
        }
    }
}
