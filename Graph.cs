using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegeriAntahBerantah
{
    public class Graph
    {
        //ATRIBUTE
        public int size;
        public bool[,] data;

        //METHOD
        public Graph(int m)
        {
            data = new bool[m, m];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    this.data[i,j] = false;
                }
            }
            this.size = m;
        }

        public void setData(int m, int n, bool input)
        { //mengubah nilai dari graf, menghubungkan atau memutus antar kota
            data[m - 1,n - 1] = input;
            data[n - 1,m - 1] = input;
        }

        public int getSize()
        { //getter
			return size;
		}

        public bool getData(int m, int n)
        { //getter
			return data[m,n];
		}

        public void Algorithm(int condition, int hider, int seeker, int index, List<int> connection, ref bool result, ref List<int> result_list)
        {
            bool connected = false;
            int index_seeker = -1;
            //index_seeker is different from seeker as parameter. index_seeker is the index of seeker position in list of connection

            //Recurtion
            for (int i = 1; i < this.getSize(); i++)
            {
                if (this.getData(index, i))
                {
                    bool exist = false;
                    //finding the i (is there 'i' in list of connection

                    for (int j = 0; j < connection.Count; j++)
                    {
                        if (connection[j] == i)
                        {
                            exist = true;
                        }
                    }

                    if (!exist)
                    {
                        connected = true;
                        connection.Add(i);
                        Algorithm(condition, hider, seeker, i, connection, ref result, ref result_list);
                        connection.RemoveAt(connection.Count - 1);
                    }
                }
            }

            //Processing in Recurtion (hitting base)
            for (int i = 0; i < connection.Count; i++)
            {
                if (connection[i] == seeker - 1)
                {
                    index_seeker = i;
                }
            }

            if (!connected && (index_seeker != -1)) //get the leaf and the seeker exist
            {
                if (condition == 0)
                {
                    //approaching house 1
                    for (int i = 0; i <= index_seeker; i++)
                    {
                        if (connection[i] == hider - 1)
                        {
                            result = true;
                            for(int j = index_seeker; j >= i; j--)
                            {
                                result_list.Add(connection[j] + 1);
                            }
                        }
                    }
                }
                else //condtion == 1
                {
                    //leaving house 1
                    for (int i = index_seeker; i < connection.Count; i++)
                    {
                        if (connection[i] == hider - 1)
                        {
                            result = true;
                            for (int j = index_seeker; j <= i; j++)
                            {
                                result_list.Add(connection[j] + 1);
                            }
                        }
                    }
                }
            }
        }
    }
}
