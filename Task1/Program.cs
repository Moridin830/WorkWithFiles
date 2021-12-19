using System;
using System.IO;

namespace Task1
{
    class Task1
    {
        //static private string PathToCatatlog = "D:/Learning/SkillFactory/Module 8/Work with files";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Please input path: ");
            string PathToCatatlog = Console.ReadLine();

            EraseFiles(PathToCatatlog);

            Console.Write("Press any key...");
            Console.ReadKey();
        }

        static void EraseFiles(string PathToCatatlog)
        {
            if (Directory.Exists(PathToCatatlog))
            {
                //Получаем список файлов в каталоге
                string[] FileList = Directory.GetFiles(PathToCatatlog);
                
                //Если каталог пуст, завершаем работу
                if(FileList.Length == 0)
                {
                    Console.WriteLine("There are no files in the selected directory. The program has ended.");
                    return;
                }

                Console.WriteLine($"There are {FileList.Length} files in the selected directory.");
                
                //Обходим полученный список файлов
                foreach(string CurrentFileName in FileList)
                {
                    //Если файл имеется, продолжаем. Если отсутствует, идем дальше по циклу
                    if (File.Exists(CurrentFileName))
                    {
                        
                        //Получаем объект файла
                        FileInfo CurrentFile = new FileInfo(CurrentFileName);
                        
                        DateTime CurrentTime = DateTime.Now;
                        DateTime FileTime = CurrentFile.LastAccessTime;
                        
                        Console.WriteLine("------------------------------------------------------------------------------------");
                        Console.WriteLine($"File: {CurrentFileName}    Date of last use: {FileTime.ToString()}");
                        
                        //Если файл не использовался более 30 минут - пытаемся удалить.
                        if (FileTime.AddMinutes(30) < CurrentTime)
                        {
                            try
                            {
                                Console.WriteLine("The file has not been used for more than 30 minutes. Trying to remove ...");
                                CurrentFile.Delete();
                                Console.WriteLine("Deletion completed successfully.");
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine("Failed to delete file for reason: \n" + ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("File unused for less than 30 minutes. Go to the next file.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Incorrect path entered.");
            }
        }
    }
}
,klm