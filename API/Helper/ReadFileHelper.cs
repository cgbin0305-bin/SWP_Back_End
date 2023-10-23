namespace API.Helper
{
    public class ReadFileHelper
    {
        public static string ReadFile(string path)
        {
            string result = "";
            if (File.Exists(path))
            {
                // Reads file line by line 
                StreamReader TextFile = new StreamReader(path);
                string line;
                while ((line = TextFile.ReadLine()) != null)
                {
                    result += line;
                }
                TextFile.Close();
            }
            return result;
        }
    }
}