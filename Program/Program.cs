using System;
using System.IO;
using System.Diagnostics;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rand = new Random(); 

            BinFile binFile = new BinFile(@"F:\Тест сортировки на внешних носителях\file.txt");

            int[] data = new int[100];
            Stopwatch time = new Stopwatch();

            using (StreamWriter writer = new StreamWriter("Test.txt"))
            {
                //Рандомный массив
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = rand.Next(0, 99);
                    writer.Write(data[i] + "  ");
                }
                writer.Write("\n\nMergeSort\n");
                //Пишем массив файл
                binFile.Write(data);

                //Сортируем файл вычисляя время сортировки
                time.Start();
                MergeSort(binFile, 10);
                time.Stop();
                
                //Читаем отсортированный файл в массив
                binFile.Reset();
                data = binFile.Read();

                //вывод результатов
                for (int i = 0; i < data.Length; i++)
                {
                    writer.Write(data[i] + "  ");
                }
                writer.Write("\n\n" + time.ElapsedMilliseconds + " milli seconds");
            }
            binFile.Clear();
        }

        //Быстрая сортировка, для кусков файла
        static void QuickSort(int[] a, int low, int high)
        {
            void Swap(int index1, int index2)
            {
                int t = a[index1];
                a[index1] = a[index2];
                a[index2] = t;
            }

            if (high - low <= 0) return;
            else if (high - low == 1)
            {
                if (a[high] < a[low]) Swap(high, low);
                return;
            }

            int up, down;
            int middle = (high + low) / 2;
            int center = a[middle];
            Swap(middle, low);
            up = low + 1;
            down = high;

            do
            {
                while (up <= down && a[up] <= center) up++;

                while (a[down] > center) down--;

                if (up < down) Swap(up, down);
            }
            while (up < down);

            a[low] = a[down];
            a[down] = center;


            if (low < down - 1) QuickSort(a, low, down - 1);

            if (down + 1 < high) QuickSort(a, down + 1, high);
        }

        //Сортировка естественным слиянием
        static void MergeSort(BinFile fC, int blockSize)
        {
            //Буферы для чтения и сортировки блоков
            int[] bufer1, bufer2;

            fC.Reset();
            //Файлы для сортировки
            BinFile fA = new BinFile(@"F:\Тест сортировки на внешних носителях\fA.txt");
            BinFile fB = new BinFile(@"F:\Тест сортировки на внешних носителях\fB.txt");
            int use = 0;

            //Читать блоки из файла fC пока его размер больше считываемого блока.
            while (fC.Length > blockSize)
            {
                //Читать блоки поочередно в файлы fA и fB
                while (!fC.EOF)
                {
                    if (!(fC.Position + blockSize > fC.Length))
                        bufer1 = fC.Read(n: blockSize);
                    else
                        bufer1 = fC.Read();

                    use++;
                    QuickSort(bufer1, 0, bufer1.Length - 1);

                    if (use % 2 == 1)
                        fA.Write(bufer1);
                    else
                        fB.Write(bufer1);
                }

                fA.Reset();
                fB.Reset();
                fC.Clear();

                //Записать блоки обратно в файл fC
                while(true)
                {
                    bufer1 = null;
                    bufer2 = null;

                    if (!fA.EOF)
                    {
                        if (!(fA.Position + blockSize > fA.Length))
                            bufer1 = fA.Read(n: blockSize);
                        else
                            bufer1 = fA.Read();
                    }
                    if (!fB.EOF)
                    {
                        if (!(fB.Position + blockSize > fB.Length))
                            bufer2 = fB.Read(n: blockSize);
                        else
                            bufer2 = fB.Read();
                    }

                    if (bufer1 == null && bufer2 == null) break;

                    if (bufer1 == null)
                    {
                        fC.Write(bufer2);
                        continue;
                    }
                    if (bufer2 == null)
                    {
                        fC.Write(bufer1);
                        continue;
                    }

                    fC.Write(bufer1);
                    fC.Write(bufer2);
                }

                fA.Clear();
                fB.Clear();
                fC.Reset();
                use = 0;
                blockSize *= 2;
            }

            if (fC.Length <= blockSize)
            {
                bufer1 = fC.Read(fC.Length, 0);
                QuickSort(bufer1, 0, bufer1.Length - 1);
                fC.Write(bufer1, 0);
                return;
            }
        }
    }
}
