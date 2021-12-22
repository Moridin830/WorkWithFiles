using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace FinalTask
{
    /// <summary>
    /// Класс для описания объекта "Студент"
    /// </summary>
    [Serializable]
    class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Student(string Name, string Group, DateTime DateOfBirth)
        {
            this.Name = Name;
            this.Group = Group;
            this.DateOfBirth = DateOfBirth;
        }
    }

    class Task4
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please input path to file:");
            string FileURL = Console.ReadLine();

            //Заменяем слэши
            FileURL = NormalizeURL(FileURL);

            //Если получили некорректный путь - возврат
            if (!File.Exists(FileURL))
            {
                Console.WriteLine("Incorrect path entered.");
                return;
            }

            //Парсим файл
            ParseFile(FileURL);

            Console.Write("Press any key...");
            Console.ReadKey();

        }

        /// <summary>
        /// Парсинг файла с двоичными данными
        /// </summary>
        /// <param name="FileURL"></param>
        private static void ParseFile(string FileURL)
        {
            //static private string URL = "D:/Learning/SkillFactory/Module 8/Work with files";

            // Получаем адрес каталога рабочего стола текущей системы
            string DesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            // Формируем адрес каталога для сохранения файлов
            string DirectoryURL = DesktopDirectory + "/" + "Students";

            // Если каталог "Students" уже создан, то запрашиваем у пользователя разрешения на его удаление.
            // Если пользователь отвечает отказом - завершаем работу программы
            if (Directory.Exists(DirectoryURL))
            {
                Console.WriteLine($"Directory '{DirectoryURL}' has already been created.\n" +
                                  $"When the program continues to run, it will be cleared. Continue (yes/no)?");

                string UserAnswer = Console.ReadLine();
                if (!(UserAnswer.ToLower() == "yes"))
                {
                    Console.WriteLine("The program is interrupted.");
                    return;
                }

                // Пытаемся удалить каталог
                DirectoryInfo WorkFolder = new(DirectoryURL);
                try
                {
                    WorkFolder.Delete(true);
                }
                catch (Exception ex)
                {
                    //Если не получилось - завершаем работу программы
                    Console.WriteLine("Failed to delete directory for reason:\n" + ex.Message);
                    return;
                }
            }

            // Пытаемся создать каталог
            try
            {
                Directory.CreateDirectory(DirectoryURL);
            }
            catch (Exception ex)
            {
                //Если не получилось - завершаем работу программы
                Console.WriteLine($"Failed to create directory '{DirectoryURL}' for reason:\n" + ex.Message);
                return;
            }

            //Начинаем читать файл
            using (FileStream ProcessedFileStream = new FileStream(FileURL, FileMode.Open))
            {
                BinaryFormatter Formatter = new BinaryFormatter();

                //Для решения проблемы с поиском соответствующей сборки при десериализации
                Formatter.Binder = new CustomizedBinder();

                Student[] StudentsList;

                // Пытаемся десериализовать массив объектов из файла
                try
                {
                    StudentsList = (Student[])Formatter.Deserialize(ProcessedFileStream);
                }
                catch (Exception ex)
                {
                    //Если не получилось - завершаем работу программы
                    Console.WriteLine("File deserialization error:\n" + ex.Message);
                    return;
                }

                var GroupList = new List<string>();
                var AvaibleFiles = new List<string>();

                // Заполняем список различных групп и уже имеющихся файлов
                foreach (Student CurrentStudent in StudentsList)
                {

                    if (!GroupList.Contains(CurrentStudent.Group))
                    {
                        GroupList.Add(CurrentStudent.Group);
                        string GroupURL = DirectoryURL + "/" + CurrentStudent.Group + ".dat";
                        if (File.Exists(GroupURL))
                        {
                            AvaibleFiles.Add(CurrentStudent.Group);
                        }

                    }
                }

                // Если уже имеются созданные файлы для части групп, запрашиваем у пользователя разрешение на их удаление.
                // Если пользователь отвечает отказом - завершаем работу программы
                if (AvaibleFiles.Count > 0)
                {
                    Console.WriteLine("Files have already been created for the groups:\n"
                                    + AvaibleFiles.ToString()
                                    + "\nThey will be overwritten if you continue. Continue? (yes/no)");
                    string UserAnswer = Console.ReadLine();
                    if (!(UserAnswer.ToLower() == "yes"))
                    {
                        Console.WriteLine("The program is interrupted.");
                        return;
                    }
                }

                // Обходим список рупп и создаем для каждой из них новый файл
                foreach (string Group in GroupList)
                {
                    string GroupURL = DirectoryURL + "/" + Group + ".dat";
                    EnterDataByGroup(Group, GroupURL, StudentsList);
                }

                Console.WriteLine("The program completed successfully.");
            }

        }


        /// <summary>
        /// Создает новый файл по указанной группе. Если файл уже имеется - предварительно удаляет его.
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="GroupURL"></param>
        /// <param name="StudentList"></param>
        private static void EnterDataByGroup(string Group, string GroupURL, Student[] StudentList)
        {
            // Удаляем существующий файл, если он имеется
            if (File.Exists(GroupURL))
            {
                try
                {
                    File.Delete(GroupURL);

                }
                catch (Exception ex)
                {
                    // Если не вышло - пропускаем эту группу
                    Console.WriteLine("Warning: failed to delete directory for reason:\n"
                                    + ex.Message
                                    + $"\nData for group No.{Group} not recorded.");
                    return;
                }
            }

            var ThisGroup = new List<Student>();

            // Собираем студентов из той же группы
            foreach (Student CurrentStudent in StudentList)
            {
                if (CurrentStudent.Group == Group)
                {
                    ThisGroup.Add(CurrentStudent);
                }
            }

            var ArrayOfStudents = ThisGroup.ToArray();

            // Пишем массив студентов в файл
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(GroupURL, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, ArrayOfStudents);
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


    /// <summary>
    /// Скопипизжено из инета для решения проблемы с поиском соответствующей сборки при десериализации
    /// </summary>
    sealed class CustomizedBinder : System.Runtime.Serialization.SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            string sharedAssemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            assemblyName = Assembly.GetExecutingAssembly().FullName;
            typeName = typeName.Replace(sharedAssemblyName, assemblyName);
            returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));

            return returntype;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out assemblyName, out typeName);
            assemblyName = "SharedAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        }
    }

}
