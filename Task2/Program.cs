using System;
using System.IO;

namespace Task2
{
    class Task2
    {
        static void Main(string[] args)
        {
            long CalculatedSize = 0;

            //Получаем адрес каталога от пользователя
            Console.WriteLine("Please input URL: ");
            string URL = Console.ReadLine();

            //Заменяем слэши
            URL = NormalizeURL(URL);

            //Проверяем существование каталога
            if (Directory.Exists(URL))
            {
                //Вычислим размер каталога
                CurrentDirectorySize(URL, ref CalculatedSize);

                Console.WriteLine($"Directory Size: {CalculatedSize} byte");
            }
            else
            {
                Console.WriteLine("Incorrect URL entered.");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void CurrentDirectorySize(string CurrentURL, ref long CalculatedSize)
        {
            //Получаем список файлов в каталоге
            string[] FileList = Directory.GetFiles(CurrentURL);

            //Получаем список вложенных каталогов
            string[] DirectoryList = Directory.GetDirectories(CurrentURL);

            //Обходим список файлов
            foreach (string CurrentFileName in FileList)
            {
                //Если файл имеется, продолжаем. Если отсутствует, идем дальше по циклу
                if (File.Exists(CurrentFileName))
                {
                    //Получаем объект файла
                    FileInfo CurrentFile = new FileInfo(CurrentFileName);

                    //Добавляем к результату размер файла
                    CalculatedSize += CurrentFile.Length;

                }
            }

            //Рекурсивно обходим список вложенных каталогов
            foreach (string DirectoryName in DirectoryList)
            {
                CurrentDirectorySize(DirectoryName, ref CalculatedSize);
            }
        }

        /// <summary>
        /// Заменяет символ '\' на '/'
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private static string NormalizeURL(string URL)
        {
            string NormalizedURL = URL.Replace('\u005C', '\u002F');

            return NormalizedURL;
        }
    }
}
