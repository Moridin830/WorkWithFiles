using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace FinalTask
{
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
            Console.WriteLine("Please input path to file: ");
            string FileURL = Console.ReadLine();

            //Заменяем слэши
            FileURL = NormalizeURL(FileURL);

            if (!File.Exists(FileURL))
            {
                Console.WriteLine("Incorrect path entered.");
                return;
            }

            ParseFile(FileURL);

            Console.Write("Press any key...");
            Console.ReadKey();

        }

        private static void ParseFile(string FileURL)
        {
            //static private string URL = "D:/Learning/SkillFactory/Module 8/Work with files";
            string DesktopDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            string DirectoryURL = DesktopDirectory + "/" + "Students";

            if (Directory.Exists(DirectoryURL))
            {
                Console.WriteLine($"Directory '{DirectoryURL}' has already been created. \n " +
                                  $"When the program continues to run, it will be cleared. Continue (yes/no)?");

                string UserAnswer = Console.ReadLine();
                if (!(UserAnswer.ToLower() == "yes"))
                {
                    Console.WriteLine("The program is interrupted.");
                    return;
                }    
                
                DirectoryInfo WorkFolder = new (DirectoryURL);
                try
                {
                    WorkFolder.Delete(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to delete directory for reason: \n" + ex.Message);
                    return;
                }
            }

            try
            {
                Directory.CreateDirectory(DirectoryURL);
            }catch(Exception ex)
            {
                Console.WriteLine($"Failed to create directory '{DirectoryURL}' for reason: \n" + ex.Message);
                return;
            }

            using (FileStream ProcessedFileStream = new FileStream(FileURL, FileMode.Open))
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                Formatter.Binder = new CustomizedBinder();
                //while (ProcessedFileStream.CanRead) 
                //{
                    var CurrentStudent = (Student[])Formatter.Deserialize(ProcessedFileStream);
                //}
            }


        }

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
