using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SortAndMerge
{
    public class SortAndMergeClass
    {
        const long long_max = 9223372036854775807; // максимальное значение int (для поиска минимального значения)
        int split_num = 0; 
        public void Split(string file) // разрезание массива на файлы
        {
            split_num++;
            StreamWriter sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, string.Format("split{0:d5}.dat", split_num)));

            using (StreamReader sr = new StreamReader(file)) // чтение файла
            {
                while (sr.Peek() >= 0) // пока не -1 (говорит о том, что из потока больше не читаются символы)
                {
                    // Записываем числа в файл в текстовом виде
                    sw.WriteLine(sr.ReadLine());

                    if (sw.BaseStream.Length > 100000000 && sr.Peek() >= 0) // если файл слишком большой
                    {
                        sw.Close();
                        split_num++;
                        sw = new StreamWriter(string.Format("split{0:d5}.dat", split_num));
                    }
                }
            }
            sw.Close();
        }
        public void SortTheParts()
        {
            foreach (string path in Directory.GetFiles(Environment.CurrentDirectory, "split*.dat"))
            {
                // я понимаю, что в этом месте можно немного оптимизировать, используя не лист, но я сделала по-простому
                List<long> content = new List<long>();
                // Чтение строк из файла
                using (StreamReader sr = new StreamReader(path)) 
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        content.Add(long.Parse(s));
                    }
                }

                content.Sort(); // сортировка чисел

                string newpath = path.Replace("split", "sorted");

                //запись чисел в новый файл
                using (StreamWriter sw = new StreamWriter(newpath))
                {
                   foreach(long el in content)
                        sw.WriteLine(el);
                }
                
                File.Delete(path); // удаление неотсортированного файла

                content = null;
                GC.Collect();
            }
        }
        public void MergeTheParts()
        {
            string[] paths = Directory.GetFiles(Environment.CurrentDirectory, "sorted*.dat");
            int parts = paths.Length; // количество разделений
            int recordsize = 100; // предполагаемый размер записи
            int maxusage = 500000000; // максимальное использование памяти
            int buffersize = maxusage / parts; // байты на каждую из очередей
            double recordoverhead = 7.5; // сколько раз будет использоваться очередь (Queue<>)
            int bufferlen = (int)(buffersize / recordsize / recordoverhead); // число записей в каждой очереди

            // Открываем файлы
            StreamReader[] readers = new StreamReader[parts];
            for (int i = 0; i < parts; i++)
                readers[i] = new StreamReader(paths[i]);

            // Массив очередей
            Queue<string>[] queues = new Queue<string>[parts];
            for (int i = 0; i < parts; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Загружаем очереди
            for (int i = 0; i < parts; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge!
            StreamWriter sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "MergeAndSortFile.txt"));
            bool done = false;
            int lowest_index, j;
            long lowest_value;
            while (!done)
            {
                // Находим часть с наименьшим значением
                lowest_index = -1;
                lowest_value = long_max;
                for (j = 0; j < parts; j++)// проходимся по всем частям
                {
                    if (queues[j] != null) // если массив с очередячми не пуст
                    {
                        if (lowest_index < 0 || long.Parse(queues[j].Peek()) < lowest_value) 
                        {
                            lowest_index = j;
                            lowest_value = long.Parse(queues[j].Peek());
                        }
                    }
                }

                // Если в очередях ничего не соталось, завершаем цикл.
                if (lowest_index == -1) { done = true; break; }

                sw.WriteLine(lowest_value);// В итоговый файл пишем самое маленькое значение
                queues[lowest_index].Dequeue();// удаление из очереди

                // Если очередь пуста
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index], readers[lowest_index], bufferlen); // если очередь опустела, подгружаем в нее из файла бОльшие значения
                    // Was there nothing left to read?
                    if (queues[lowest_index].Count == 0) // если ничего не подгрузилось (все уже отсортировали)
                    {
                        queues[lowest_index] = null; // обнуляем
                    }
                }
            }
            sw.Close();

            // Close and delete the files
            for (int i = 0; i < parts; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }
        void LoadQueue(Queue<string> queue, StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }
    }
}
