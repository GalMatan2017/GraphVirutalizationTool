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
                {
                    reader.Close();
                    throw new Exception($"Row {rows} is corrupted!");
                }
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

            foreach (var _line in list)
            {
                for (int i = 1; i < _line.Count; i++)
                    if (_line[i] == _line[0])
                        throw new Exception("Cannot open a directed graph");
            }

            bool pass = false;

            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 1; j < list[i].Count; j++)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (list[k][0] == list[i][j])
                        {
                            for (int m = 0; m < list[k].Count; m++)
                            {
                                if (list[k][m] == list[i][0])
                                {
                                    pass = true;
                                    break;
                                }
                            }
                        }
                        if (pass)
                            break;
                    }
                    if (!pass)
                        throw new Exception("Cannot open a directed graph");
                    pass = false;
                }
            }


            return (List<List<T>>)Convert.ChangeType(list, typeof(List<List<T>>));
        }
    }
}
