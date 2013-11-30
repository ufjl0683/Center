using System;
using System.Collections.Generic;
using System.Text;

namespace Comm.DataStore
{
  
    public   class DataStorage<T>
    {
        System.Collections.Generic.SortedDictionary<DateTime, StoreData<T>> datas;
        System.Threading.Timer tmrClear;
        public DataStorage()
        {
            if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory+"store.dat"))

                datas = new SortedDictionary<DateTime, StoreData<T>>();
            else
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ft = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                datas = ft.Deserialize(System.IO.File.OpenRead(AppDomain.CurrentDomain.BaseDirectory+"store.dat")) as System.Collections.Generic.SortedDictionary<DateTime, StoreData<T>>;
            }
            if (datas == null)
            {
                Console.WriteLine("store.dat read fail!");
                Environment.Exit(-1);
            }
            Console.WriteLine("reload store data cnt:" + datas.Count);
            tmrClear = new System.Threading.Timer(TmrclearTask);
            tmrClear.Change(TimeSpan.FromSeconds(600),TimeSpan.FromMinutes(1));
          
        }
        public void TmrclearTask(object state)
        {
            try
            {
                StoreData<T>[] temp=new StoreData<T>[datas.Values.Count];//=new DataStorage[datas.Values.Count];
                datas.Values.CopyTo(temp, 0);
                foreach(StoreData<T> sdata in temp)
                {
                    if (sdata.DateTime >= DateTime.Now.Subtract(TimeSpan.FromDays(3)))
                        break; 

                    lock (datas)
                    {
                        datas.Remove(sdata.DateTime);
                    }

                }
                SaveStorage();
            }
            catch(Exception ex)
            {
              Console.WriteLine(ex.Message+","+ex.StackTrace)  ;
            }
        }
        public void PutStoreData( StoreData<T> data)
        {
            lock (datas)
            {
                if (!datas.ContainsKey(data.DateTime))
                    datas.Add(data.DateTime, data);
            }
        }
        
        public StoreData<T>[] GetStoreData(DateTime dt, int avgmin)
        {
            System.Collections.Generic.List<StoreData<T>> list = new List<StoreData<T>>();
            lock (datas)
            {
                for (DateTime d = dt.AddMinutes(-avgmin); d < dt; d = d.AddMinutes(1))
                {
                    if (datas.ContainsKey(d))
                        list.Add(datas[d]);
                }

                if (list.Count == 0)
                    return new StoreData<T>[0];
                 
                return list.ToArray();
                // StoreData storedata=new StoreData(dt,

                //if (datas.ContainsKey(dt))
                //    return datas[dt];
                //else
                //    return null;
            }
        }


      
        void SaveStorage()
        {
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter ft = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.FileStream fs=System.IO.File.OpenWrite(AppDomain.CurrentDomain.BaseDirectory+"store.dat");
            ft.Serialize(fs, datas);
            fs.Flush();
            fs.Close();
           
        }




      
    }

    [Serializable]

    public class StoreData<T>:IComparable
    {
        public  DateTime DateTime;
        T[] values;

        public StoreData(DateTime dt,params T [] data)
        {
            if (data.Length == 0)
                return;
         values=data;
         DateTime = dt;
        }
        public T[] GetValue()
        {
            return values;
        }


        public int CompareTo(object obj)
        {
            StoreData<T> other = obj as StoreData<T>;
            return (int) ((this.DateTime - other.DateTime).TotalSeconds);
        }
    }
}
