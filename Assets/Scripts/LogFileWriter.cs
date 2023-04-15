using System.IO;
using UnityEngine;

public class LogFileWriter
{
    private static string logFilePath = Application.dataPath + "/LogFile.txt";

    public static void WriteLog(string logMessage)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine(logMessage);
        }
    }
}
