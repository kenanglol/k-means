using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace K_means
{
    //We develop this class for produce random number in close times. And we add double randomize option.  
    class rastgele
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public double GetRandomNumber(double minimum, double maximum)
        {
            lock (syncLock)
            { // synchronize
                return random.NextDouble() * (maximum - minimum) + minimum;
            }
        }
    }
    //Center class is k-means algorithm's k value's class. This class contain data in an arraylist.
    class Center
    {
        rastgele rst = new rastgele();
        public Center eskiMerkez;//this object keeps because of comparing old and new center. 
        public double d1, d2, d3, d4;//values of data's attribute numbers 
        public ArrayList liste = new ArrayList();//this list contains data objects
        public Center(double[] alt, double[] ust)
        {

            
            this.d1 = rst.GetRandomNumber(alt[0], ust[0]);
            this.d2 = rst.GetRandomNumber(alt[1], ust[1]);
            this.d3 = rst.GetRandomNumber(alt[2], ust[2]);
            this.d4 = rst.GetRandomNumber(alt[3], ust[3]);
        }
        public Center(Center eski)
        {
            d1 = eski.d1;
            d2 = eski.d2;
            d3 = eski.d3;
            d4 = eski.d4;
        }
        public double[] ort()
        {
            //mean algorithm
            int len = liste.Count;
            double[] orta = new double[4];
            foreach (plant flower in liste)
            {
                orta[0] += flower.deger[0];
                orta[1] += flower.deger[1];
                orta[2] += flower.deger[2];
                orta[3] += flower.deger[3];
            }
            orta.Select(x => x / len);
            return orta;
        }
        public  double[] variance_cal()
        {
            //variance calculation to measure the system of stability
            double[] mean=this.ort();
            double[] varian = new double[4];
            for (int a=0;a<4;a++)
            {
                foreach(plant flo in liste)
                {
                    varian[a]+=Math.Pow((mean[a] - flo.deger[a]), 2);
                }
            }
            varian.Select(x => x /(liste.Count*1000));
            return varian;
        }
        public bool approximate(int r)
        {
            //algorithm stop conditions
            double[] varia=this.variance_cal();
            bool finish = false;
            if (r > 500)
            {
                finish = true;
            } else if (varia[0]==0 && varia[1] == 0 && varia[2] == 0 && varia[3] == 0)
            {
                finish = true;
            }
            return finish;
        }
        public void New_Center()
        {
            //find the mean of arraylist of points that closest center and assign as new center
            double[] elen=this.ort();
            this.eskiMerkez = new Center(this);
            this.d1=elen[0];
            this.d2 = elen[1];
            this.d3 = elen[2];
            this.d4 = elen[3];
        }
        public string tostring()
        {
            string print = this.d1 + "," + this.d2 + "," + this.d3 + "," + this.d4;
            return print;
        }


    }
    class plant
    {
        //keep the data's attribute in a class because of more proper to oop concepts.
        public double[] deger = new double[4];
        public string tur;
        public plant(double d1, double d2, double d3, double d4, string isim)
        {
            deger[0] = d1;
            deger[1] = d2;
            deger[2] = d3;
            deger[3] = d4;
            this.tur = isim;
        }

        public string print()
        {
            string bas = this.deger[0] + "," + this.deger[1] + "," + this.deger[2] + "," + this.deger[3] + "," + this.tur;
            return bas;
        }

    }


    class Program
    {
        static void Main(string[] args)
        {

            ArrayList cicekList = new ArrayList();
            //pull data by string type from the link 
            System.IO.StreamReader sw = new StreamReader(@"C:\Users\Kenan\source\repos\ConsoleApp6\ConsoleApp6\veri.txt");
            String satir;
            //find max and min values for first center's random method.
            double[] ust = { 0.0, 0.0, 0.0, 0.0 };
            double[] alt = { 100.0, 100.0, 100.0, 100.0 };

            int merkez_say = 3;

            while ((satir = sw.ReadLine()) != null)
            {

                string[] koordinatlar = satir.Split(',');
                //convert string to double and create plant object 
                double q = Double.Parse(koordinatlar[0],System.Globalization.CultureInfo.InvariantCulture);
                double b = Double.Parse(koordinatlar[1], System.Globalization.CultureInfo.InvariantCulture);
                double c = Double.Parse(koordinatlar[2], System.Globalization.CultureInfo.InvariantCulture);
                double d = Double.Parse(koordinatlar[3], System.Globalization.CultureInfo.InvariantCulture);
                plant yaprak = new plant(q, b, c, d, koordinatlar[4]);
                cicekList.Add(yaprak);
                for (int a = 0; a < 4; a++)
                {
                    if (alt[a] > yaprak.deger[a])
                    {
                        alt[a] = yaprak.deger[a];
                    }
                    if (ust[a] < yaprak.deger[a])
                    {
                        ust[a] = yaprak.deger[a];
                    }
                }
            }
            sw.Close();
            
            Center[] Merkezler = new Center[merkez_say];
            for (int i = 0; i < merkez_say; i++)
            {
                Merkezler[i] = new Center(alt, ust);

            }
            bool finish = true;
            int r = 0;
            do
            {
                //cluster all of plant by attribute 
                foreach (plant a in cicekList)
                {

                    double m1;
                    double min = 1000000;
                    Center min1 = null;
                    for (int i = 0; i < merkez_say; i++)
                    {
                        //minimum euclid distance for clustring process 
                        m1 = Math.Sqrt((Math.Pow(a.deger[0] - Merkezler[i].d1, 2)) + (Math.Pow(a.deger[1] - Merkezler[i].d2, 2))
                            + (Math.Pow(a.deger[2] - Merkezler[i].d3, 2)) + (Math.Pow(a.deger[3] - Merkezler[i].d4, 2)));

                        if (m1 < min)
                        {
                            min1 = Merkezler[i];
                            
                            min = m1;
                        }
                        
                    }
                    min1.liste.Add(a);
                    

                }
                
                
                for (int p = 0; p < merkez_say; p++)
                {
                    Merkezler[p].New_Center();
                    
                }
                    foreach (Center m in Merkezler)
                    {
                    //print the console datas by clusters
                        Console.WriteLine();
                    foreach(plant fl in m.liste)
                    {
                        Console.WriteLine(fl.print());
                    }
                }
                foreach (Center m in Merkezler)
                {
                    //clean all center's plant list for new clustering
                    m.liste.Clear();
                }
                for (int p = 0; p < merkez_say; p++)
                {
                    //control finish condition
                    if (Merkezler[p].approximate(r)==false)
                    {

                        finish = false;
                        break;
                    }
                }
                r++;
            }
            while (finish!=true);
            Console.ReadKey();
        }
    }
}

