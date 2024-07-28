namespace CSConvert.SharedServices
{
  public class FileSaver
  {
    public static void SaveFile(string directoryPath, string fileName, string fileContent)
    {
      Directory.CreateDirectory(directoryPath);

      string filePath = Path.Combine(directoryPath, fileName);
      File.WriteAllText(filePath, fileContent);
    }

  }
}
