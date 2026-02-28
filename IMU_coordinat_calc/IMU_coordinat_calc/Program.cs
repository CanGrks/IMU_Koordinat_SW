using System;

namespace IMU_coordinat_calc
{
    internal class Program
    {
        static void Main()
        {
            //dereceyi radyana dönştürme katsayısı

            double der2rad = Math.PI / 180;

            #region P_SCS Read
            //IMU, Sensor Koordinat Sistemi (SCS) ‘nde başlangıçta aşağıdaki noktayı ölçmüştür.
            //P_SCS = [𝑋 𝑌 𝑍] = [1 2 3]

            Console.WriteLine("IMU, Sensor Koordinat Sistemi (SCS) ‘nde başlangıçta hangi noktayı ölçmüştür? Sırayla giriniz [x y z] m");
            double X = double.Parse(Console.ReadLine());
            double Y = double.Parse(Console.ReadLine());
            double Z = double.Parse(Console.ReadLine());
            double[] P_SCS_Homojen = { X, Y, Z, 1 };

            #endregion


            #region SCS2PCS Dönüşüm Matrisi

            //Sensör, test platformuna göre X ekseni etrafında 30° döndürülmüştür.

            Console.WriteLine("Sensör, test platformuna göre X ekseni etrafında kaç derece döndürülmüştür? Giriniz (°)");
            double RotDereceX = double.Parse(Console.ReadLine());
            double cosTheta = Math.Cos(RotDereceX * der2rad);
            double sinTheta = Math.Sin(RotDereceX * der2rad);

            //X ekseni etrafında dönüşüm matrisi

            double[,] Rx_Theta = {
                { 1,    0     ,     0     },
                { 0, cosTheta , -sinTheta },
                { 0, sinTheta ,  cosTheta }
            };

            //Sensör, test platformu üzerinde X ekseninde 0.2 metre öteleme yapılmıştır. 

            Console.WriteLine("Sensör, test platformu üzerinde X ekseninde kaç metre öteleme yapılmıştır? Giriniz (m)");
            double TraMetreX = double.Parse(Console.ReadLine());

            //H_{SCS → PCS} Homojen dönüşüm matrisi oluşturma

            double[,] H_SCS2PCS = new double[4, 4];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    H_SCS2PCS[i, j] = Rx_Theta[i, j];
                }

            // X ekseninde öteleme

            H_SCS2PCS[0, 3] = TraMetreX;
            H_SCS2PCS[3, 3] = 1;

            Console.WriteLine();
            Console.WriteLine(" H_SCS2PCS Homojen dönüşüm matrisi ");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write("   " + H_SCS2PCS[i, j] + "   ");
                }
                Console.WriteLine();
            }
            #endregion

            Console.WriteLine();

            #region PCS2WCS Dönüşüm Matrisi

            //Test platformu dünya koordinat sistemine göre Z ekseni etrafında 45° döndürülmüştür. 

            Console.WriteLine("Test platformu, dünya koordinat sistemine göre Z ekseni etrafında kaç derece döndürülmüştür? Giriniz (°)");
            double RotDereceZ = double.Parse(Console.ReadLine());
            double cosBeta = Math.Cos(RotDereceZ * der2rad);
            double sinBeta = Math.Sin(RotDereceZ * der2rad);

            //Z ekseni etrafında dönüşüm matrisi

            double[,] Rz_Beta = {
                { cosBeta , -sinBeta ,    0 },
                { sinBeta ,  cosBeta ,    0 },
                {   0     ,    0     ,    1 }
            };

            //Test platformu,  dünya koordinat sistemine göre Z ekseninde 10 metre öteleme yapılmıştır.

            Console.WriteLine("Test platformu,  dünya koordinat sistemine göre Z ekseninde kaç metre öteleme yapılmıştır? Giriniz (m)");
            double TraMetreZ = double.Parse(Console.ReadLine());

            //H_{PCS → WCS} Homojen dönüşüm matrisi oluşturma

            double[,] H_PCS2WCS = new double[4, 4];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    H_PCS2WCS[i, j] = Rz_Beta[i, j];
                }

            // Z ekseninde öteleme

            H_PCS2WCS[2, 3] = TraMetreZ;
            H_PCS2WCS[3, 3] = 1;

            Console.WriteLine();
            Console.WriteLine(" H_PCS2WCS Homojen dönüşüm matrisi ");
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write("   " + H_PCS2WCS[i, j] + "   ");
                }
                Console.WriteLine();
            }

            #endregion

            Console.WriteLine();

            #region H_Total Calculation

            //H_Total = = H_{SCS → WCS}

            double[,] H_Total = new double[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                    {
                        H_Total[i, j] += H_PCS2WCS[i, k] * H_SCS2PCS[k, j];
                    }
            Console.WriteLine(" H_Total = H_PCS2WCS * H_SCS2PCS ");

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write("   " + H_Total[i, j] + "      ");
                }
                Console.WriteLine();
            }

            #endregion

            Console.WriteLine();

            #region P_WCS Calculation

            //Hesaplanan son vektör, Dünya Koordinat Sisteminde sensörün ölçtüğü noktanın konumunu verecektir. 
            //P_WCS = H_Total * P_SCS

            double[] P_WCS = new double[4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    P_WCS[i] += H_Total[i, j] * P_SCS_Homojen[j];
                }
            Console.WriteLine(" P_WCS [x y z] m ");

            for (int i = 0; i < 3; i++)
            {
                Console.Write("      " + P_WCS[i] + " metre   ");
            }
            Console.WriteLine();

            #endregion
            
            Console.Read();
        }
    }
}