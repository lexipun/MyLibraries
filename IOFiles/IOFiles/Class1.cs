using System;
using System.IO;
using System.Runtime.InteropServices;

namespace IOFiles
{
    public class IOFiles
    {
        enum ABC{
            A = 0,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            O,
            P,
            Q,
            R,
            S,
            T,
            U,
            V,
            W,
            X,
            Y,
            Z,
        }



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
        
        public string[][] ReadFromExel(string path)
        {
            string[][] rez;

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(path);
            Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = (Microsoft.Office.Interop.Excel._Worksheet)xlWorkbook.Sheets[1];
            Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            rez = new string[rowCount][];

            for (int i = 1; i <= rowCount; i++)
            {
                rez[i] = new string[colCount];
                for (int j = 1; j <= colCount; j++)
                {
                    if (xlRange.Cells[i, j] != null && ((Microsoft.Office.Interop.Excel.Range)xlRange.Cells[i, j]).Value2 != null)
                        rez[i][j] = ((Microsoft.Office.Interop.Excel.Range)xlRange.Cells[i, j]).Value2.ToString();
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return rez;
        }

        public void WriteToExcel(string[][] content, string path)
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;
            int rowContent = content.Length;
            int columContent = content[0].Length;
            int countABC = content.Length / ((int)ABC.Z + 1) + (content.Length % ((int)ABC.Z + 1) != 0 ? 1 : 0);

            string endABC = "";

            for(int i =0; i < countABC; ++i)
            {
                if(i + 2 == countABC)
                {
                    endABC += (ABC)(content.Length % ((int)ABC.Z + 1));
                }
                else
                {
                    endABC += "Z";
                }

            }

            try
            {
                oXL = new Microsoft.Office.Interop.Excel.Application();
                oXL.Visible = true;

                oWB = (oXL.Workbooks.Add(""));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

                oSheet.get_Range("A1", endABC + columContent).Value2 = content;

                oXL.Visible = false;
                oXL.UserControl = false;
                oWB.SaveAs(path, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();
                oXL.Quit();
            }
            catch(Exception ex)
            {

            }

        }

    }
}
