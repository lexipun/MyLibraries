using System;
using System.IO;

namespace IOFiles
{
    public class IOFiles
    {
        public bool XMLSerialize<T>(T obj, string path)
        {
            if(obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);
            }

            return true;
        }

        public bool BinarySerialize<T>(T obj, string path)
        {
            if (obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, obj);
            }

            return true;
        }

        public bool JsonSerializer<T>(T obj, string path)
        {
            if (obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                formatter.WriteObject(fs, obj);
            }

            return true;
        }

        public bool XMLDeserialize<T>(ref T obj, string path)
        {
            if (obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Xml.Serialization.XmlSerializer(typeof(T));
            
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    obj = (T)formatter.Deserialize(fs);

                }
                return true;
        }

        public bool JsonDeserialize<T>(ref T obj, string path)
        {
            if (obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                obj = (T)formatter.ReadObject(fs);

            }
            return true;
        }

        public bool BinaryDeserialize<T>(ref T obj, string path)
        {
            if (obj == null || path == null)
            {
                return false;
            }

            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                obj = (T)formatter.Deserialize(fs);

            }
            return true;
        }

        public string ReadAllText(string? path)
        {
            string rez;

            using(var reader = new StreamReader(path))
            {
                rez = reader.ReadToEnd();    
            }

            return rez;
        }

        public System.Collections.Generic.List<string> ReadLines(Predicate<string> predicate, string path)
        {

            var listrez = new System.Collections.Generic.List<string>();

            using (var reader = new StreamReader(path))
            {

                do
                {
                    listrez.Add(reader.ReadLine());

                } while (!reader.EndOfStream && predicate(listrez[listrez.Count - 1]));

            }

            return listrez;
        }

        public System.Collections.Generic.List<char> Read(Predicate<char> predicate, string path)
        {

            var listrez = new System.Collections.Generic.List<char>();

            using (var reader = new StreamReader(path))
            {

                do
                {
                    listrez.Add((char)reader.Read());

                } while (!reader.EndOfStream && predicate(listrez[listrez.Count - 1]));

            }

            return listrez;
        }

        public bool Write(string? path, string? whatwrite)
        {

            using (var writer = new StreamWriter(path))
            {
                writer.Write(whatwrite);
            }

            return true;
        }

        public string ReadFromDoc(string path)
        {
            var app = new Microsoft.Office.Interop.Word.Application();
            var doc = app.Documents.Open(path);
            string rez = doc.Content.Text;
            doc.Close();
            return rez;
           
        }

    }
}
