using UnityEngine;
using System;
using System.IO;
using ThunderFireUnityEx;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelToJsonConverter
{
    public static void ConvertExcelFileToJson(string[] filePaths, string outputPath)
    {
        List<LocalizationTextRow> table = new List<LocalizationTextRow>();
        foreach(string file in filePaths)
        {
            try
            {
                using(FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using(ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet sheet = package.Workbook.Worksheets[1];
                        for(int m = 2; m <= sheet.Dimension.End.Row; m++)
                        {
                            LocalizationTextRow item = new LocalizationTextRow();
                            if(sheet.GetValue(m, 1) == null) continue;
                            item.key = sheet.GetValue(m, 1).ToString();
                            item.translates = new string[LocalizationLanguage.Length];
                            for (int j = sheet.Name == "Runtime" ? 4 : 2, k = 0; k < LocalizationLanguage.Length; j++, k++)
                            {
                                item.translates[k] = sheet.GetValue(m, j)?.ToString() ?? "";
                            }
                            table.Add(item);
                        }
                    }
                }
            }
            catch(FileNotFoundException)
            {
                continue;
            }
            catch(DirectoryNotFoundException)
            {
                continue;
            }
        }
        string spreadSheetJson = JsonUtilityEx.ToJson(table);
        if (String.IsNullOrEmpty(spreadSheetJson)) return;
        WriteTextToFile(spreadSheetJson, outputPath);
		Debug.Log("Convert Successfully!");
    }

    private static void WriteTextToFile(string text, string filePath)
    {
        System.IO.File.WriteAllText(filePath, text);
    }
}
