using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace progect;
enum Status
{
    Rejected, 
    Approved, 
    Pending
}


enum ReportType
{
    Intel,
    Recon, 
    Analyze, 
    Collect
}
class Magerment
{
    static string[]? LoaadFile(string path)

    {
        string name = Path.GetFileName(path);
        if (!File.Exists(path))
        {
            Console.Write($"found not{name}File :Error.");
            return null;
        }
        string[] lines = File.ReadAllLines(path);
        if (lines.Length != 0)
        {
            Console.Write($"File loaded:{lines.Length} lines found\n");
            return lines;
        }

        else
        {
            Console.WriteLine($"empty is {name} :Error.");
            return null;
        }
    }


    static string[] CleanLine(string row)
    {
        string[] cleanLine = row.Trim().Split(',');
        return cleanLine;
    }

    static bool TryParseEnum(string[] cleanLine, int i, out Status s)
    {
        if (Status.TryParse(cleanLine[i].Trim(), true, out s))
            { return true; }
        Console.Write($"reason {cleanLine[i]}");
        return false;
    }

    static bool TryParseDouble(string[] cleanLine, int i, out double score)
    {

        if (double.TryParse(cleanLine[i].Trim(), out score))
        { 
            return score >= 0.0 && score <= 100.0; 
        }
        Console.Write($"reason {cleanLine[i]}");
        return false;

    }

    static bool TryParseInt(string[] cleanLine, int i, out int priority)
    {
        if (int.TryParse(cleanLine[i].Trim(), out priority))
        {
            {
                return priority >= 1 && priority <= 5;
            }
        }
        return false;
    }

    static bool ParseReportType(string[] cleanLine, int i, out ReportType type)
    {

        return ReportType.TryParse(cleanLine[i].Trim(), true, out type);
    }

    static bool ParseUnitype(string[] cleanLine, int i)
    {
        return cleanLine[i].Trim() != null;
    }

    static int ProcessReports(string[] lines, string[] unitName, ReportType[] reportType, int[] priority, double[] score, Status[] status)
    {
        int count = 0;
        int trueCount = 0;
        foreach (string line in lines)
        {
            string[] cleanLine = CleanLine(line);
            if (cleanLine.Length == 5)
            {
                if ((ParseUnitype(cleanLine, 0) &&
                      ParseReportType(cleanLine, 1, out ReportType type) &&
                      TryParseInt(cleanLine, 2, out int p) &&
                      TryParseDouble(cleanLine, 3, out double s) &&
                      TryParseEnum(cleanLine, 4, out Status st)))
                {
                    unitName[count] = cleanLine[0].Trim();
                    reportType[count] = type;
                    priority[count] = p;
                    score[count] = s;
                    status[count] = st;
                    trueCount = trueCount + 1;
                                Console.Write($"Unit: {cleanLine[0].Trim()}\n Type: {type}\n Priority:{p}\n Score:{s}\n Status:{st} ");
                }
            }
            count++;
        }
        return trueCount;
    }

    static double CalculateAverage(double[] score, int countTrue)
    {
        if (countTrue == 0) return 0;

        double sum = 0;
        for (int i = 0; i < countTrue; i ++)
            {

                sum = sum + score[i]; 
        
            }
        return sum / countTrue;  
    }



    static void Main()
    {
        string path = @".\reports.txt";
        string[]? lin = LoaadFile(path);
        if (lin != null)
        {
            int larg = lin.Length;

            string[] unitName = new string[larg];
            ReportType[] reportType = new ReportType[larg];
            int[] priority = new int[larg];
            double[] score = new double[larg];
            Status[] status = new Status[larg];
            int countTrue = ProcessReports(lin, unitName, reportType, priority, score, status);
            Console.WriteLine($"\n {CalculateAverage(score, countTrue)}");
        }


    }

}





