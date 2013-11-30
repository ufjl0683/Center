using System;
using System.Collections.Generic;
using AForge.Neuro.Learning;
using AForge.Neuro;
using System.Text;

namespace NeuralTest
{
    class Program
    {
        static  System.Collections.Generic.List<TrainData> traindatas = new List<TrainData>();
        static void Main(string[] args)
        {



                LoadTrainData();
                ExactTrainingData();
                Console.ReadKey();
               
                 
        }

        public static void ExactTrainingData()
        {
            TrainData[] tdatas;

           
           // AForge.Neuro.Learning.BackPropagationLearning beural=new AForge.Neuro.Learning.BackPropagationLearning(new AForge.Neuro.ActivationNetwork(
            tdatas = traindatas.ToArray();

            double[][] output = new double[tdatas.Length-1][];
            double[][] input = new double[tdatas.Length-1][];

            for (int i = 1; i < tdatas.Length; i++)
            {
                int spddiff = 0, voldiff = 0, occdiff = 0, u_spddifft = 0, u_voldifft = 0, u_occdifft = 0, d_spddifft = 0, d_voldifft = 0, d_occdifft = 0,level=0;
                tdatas[i].getTrainData(ref voldiff, ref spddiff, ref occdiff);

                u_spddifft = tdatas[i].vd1.AvgSpd - tdatas[i - 1].vd1.AvgSpd;
                u_voldifft = tdatas[i].vd1.Volume - tdatas[i-1].vd1.Volume;
                u_occdifft = tdatas[i].vd1.Occupancy - tdatas[i - 1].vd1.Occupancy;

                d_spddifft = tdatas[i].vd2.AvgSpd - tdatas[i - 1].vd2.AvgSpd;
                d_voldifft = tdatas[i].vd2.Volume - tdatas[i - 1].vd2.Volume;
                d_occdifft = tdatas[i].vd2.Occupancy - tdatas[i - 1].vd2.Occupancy;
                level = tdatas[i-1].Level;
                output[i-1] = new double[1];
                output[i-1][0] = level;
                input[i-1] = new double[9];
                input[i-1][0] = spddiff;
                input[i-1][1] = voldiff;
                input[i-1][2] = occdiff;
                input[i-1][3] = u_spddifft;
                input[i-1][4] = u_voldifft;
                input[i-1][5] = u_occdifft;
                input[i-1][6] = d_spddifft;
                input[i-1][7] = d_voldifft;
                input[i-1][8] = d_occdifft;
                
                Console.WriteLine(spddiff + "," + voldiff + "," + occdiff + "," + u_spddifft + "," + u_voldifft + "," + u_occdifft + "," + d_spddifft + "," + d_voldifft + "," + d_occdifft+","+level);

            }
 
            ActivationNetwork    network = new ActivationNetwork(
                new BipolarSigmoidFunction(1.5),9, 25, 1 );
            // create teacher
            BackPropagationLearning teacher = new BackPropagationLearning(network);
            teacher.Momentum = 0.1;
            teacher.LearningRate = 0.01;
            // loop
              double err=100;
              int cnt = 0;
              while (err / tdatas.Length >0.00079)
                {
                    // run epoch of learning procedure
                     err = teacher.RunEpoch( input, output );
                     Console.WriteLine(err / tdatas.Length);
                     cnt++;
                }

              for (int i = 0; i < tdatas.Length-1; i++)
              {
                  if (System.Convert.ToInt32(output[i][0]) != System.Convert.ToInt32(network.Compute(input[i])[0]))
                      Console.WriteLine("fail");

                //  Console.WriteLine("chreck:"+Math.Abs((output[i][0] - network.Compute(input[i])[0]))>0,5?:"fail","");
              }

           

        }



        public static void LoadTrainData()
        {
        
            using (System.IO.StreamReader rd = new System.IO.StreamReader(System.IO.File.Open(AppDomain.CurrentDomain.BaseDirectory + "aidtraindata.csv", System.IO.FileMode.Open)))
            {
                rd.ReadLine();  //skip header
                //   

                string s = null;

                TrainData tdata = null;
                try
                {
                    int seqno = -1;

                    while ((s = rd.ReadLine()) != "")
                    {
                        string[] items = s.Split(new char[] { ',' });
                        try
                        {
                            if (seqno != System.Convert.ToInt32(items[1]))
                            {
                                if (tdata != null)
                                {
                                    traindatas.Add(tdata);

                                //    Console.WriteLine(tdata.ToString());
                                }


                                tdata = new TrainData();
                                seqno = System.Convert.ToInt32(items[1]);
                                //--------------------------------------------
                            }
                            string vdname = "";
                            int smallvol, smallspd, bigvol, bigspd, connectvol, connectspd, occupancy, level;
                            vdname = items[0].Trim();
                            smallvol = System.Convert.ToInt32(items[3]);
                            bigvol = System.Convert.ToInt32(items[4]);
                            connectvol = System.Convert.ToInt32(items[5]);
                            smallspd = System.Convert.ToInt32(items[6]);
                            bigspd = System.Convert.ToInt32(items[7]);
                            connectspd = System.Convert.ToInt32(items[8]);
                            occupancy = System.Convert.ToInt32(items[9]);
                            level = System.Convert.ToInt32(items[10]);
                            tdata.AddLaneData(vdname, new LaneData(smallvol, bigvol, connectvol, smallspd, bigspd, connectspd, occupancy, level));

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message + "," + ex.StackTrace);
                }
        }
           
        }
    }
}
