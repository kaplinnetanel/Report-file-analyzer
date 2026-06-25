using System;
using System.IO;
using System.Net.NetworkInformation;
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
        if (Enum.TryParse<Status>(cleanLine[i].Trim(), true, out s))
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

        return Enum.TryParse<ReportType>(cleanLine[i].Trim(), true, out type);
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
                    unitName[trueCount] = cleanLine[0].Trim();
                    reportType[trueCount] = type;
                    priority[trueCount] = p;
                    score[trueCount] = s;
                    status[trueCount] = st;
                    trueCount ++;
                              ///  Console.Write($"Unit: {cleanLine[0].Trim()}\n Type: {type}\n Priority:{p}\n Score:{s}\n Status:{st} ");
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

    static double FindMaxScore(double[] score, int countTrue)
        {
            if (countTrue == 0) return 0;
            double scoreMax = 0;
            for (int i = 0; i < countTrue; i++)
               {

                    if (score[i] > scoreMax)
                          { scoreMax = score[i];  }

                }
            return scoreMax;
       }
            
     static double FindMinScore(double[] score, int countTrue)
      {
                if (countTrue == 0) { return 0; }
                double miniScore = 100.0; ;
                for (int i = 0; i < countTrue; i++)
                {

                    if (score[i] < miniScore)
                    { miniScore = score[i]; }

                }
        return miniScore;
      }

    static int CountByStatus(Status[] status , Status s,int countTrue)
        {
           int count = 0; 
           for(int i = 0; i< countTrue;  i++)
           {
                if (status[i] == s)
                   { count = count + 1; }
           }
           return count; 
    
        }

    static int CountByType(ReportType[] reportType , ReportType r, int countTrue)
       {

            int count = 0;
            for (int i = 0; i < countTrue; i++)
            {
                if (reportType[i] == r)
                { count = count + 1; }
            }
            return count;


    }

    static void DisplayBasicStatistics(double[] score, int countTrue)
    {
        double average = CalculateAverage(score, countTrue);
        double maxScore = FindMaxScore(score, countTrue);
        double miniScore = FindMinScore(score, countTrue);
        Console.WriteLine("=== Report Statistics ===");
        Console.WriteLine($"Total Reports: {countTrue}");
        Console.WriteLine($"Average Score: {average:F2}");
        Console.WriteLine($"Highest Score: {maxScore:F1}");
        Console.WriteLine($"Lowest Score: {miniScore:F1}");


    }

    static void DisplayStatusCounts(Status[]status, int countTrue)
    {
        Console.WriteLine("=== Reports by Status ===");
        Console.WriteLine($"Pending: {CountByStatus(status, Status.Pending, countTrue)}");
        Console.WriteLine($"Approved: {CountByStatus(status, Status.Approved, countTrue)}");
        Console.WriteLine($"Rejected: {CountByStatus(status, Status.Rejected, countTrue)}");

    }

    static void DisplayTypeCounts(ReportType[] r, int countTrue)
    {
        Console.WriteLine("=== Reports by Type ===");
        Console.WriteLine($"Collect: {CountByType(r, ReportType.Collect, countTrue)}");
        Console.WriteLine($"Analyze: {CountByType(r, ReportType.Analyze, countTrue)}");
        Console.WriteLine($"Recon: {CountByType(r, ReportType.Recon, countTrue)}");
        Console.WriteLine($"Intel: {CountByType(r, ReportType.Intel, countTrue)}");

    }

    static void DisplayHighestPriorityApproved(string[] unitName, ReportType[] reportType, int[] priority, double[] score, Status[] status, int countTrue)
    {
        string u ="None";
        ReportType r = ReportType.Intel;
        int p = 0;
        double sc = 0.0;
        Status s = Status.Rejected ;
        for (int i = 0; i < countTrue; i++)
        {
            if (priority[i] > p || (priority[i] == p && score[i] > sc))
            {
                if (priority[i] > p)
                {
                    u = unitName[i];
                    r = reportType[i];
                    p = priority[i];
                    sc = score[i];
                    s = status[i];
                }
            
            }

        }
        Console.WriteLine("=== Highest Priority Approved Report ===");
        Console.WriteLine($"Unit: {u}\nType: {r}\nPriority: {p}\nScore: {sc}");

    }

    static void DisplayAverageByPriority(int[] priority, double[] score, int countTrue)
    {
        double[] p = new double[6];
        int[] count = new int[6];
        for (int i = 0; i < countTrue; i++)
        {
            switch (priority[i])
            {
                case 1:
                    p[1] = p[1] + score[i];
                    count[1]++;
                    break;
                case 2:
                    p[2] = p[2] + score[i];
                    count[2]++;
                    break;
                case 3:
                    p[3] = p[3] + score[i];
                    count[3]++;
                    break;
                case 4:
                    p[4] = p[4] +score[i];
                    count[4]++;
                    break;
                case 5:
                    p[5] = p[5] + score[i];
                    count[5]++;
                    break;

            }
        }

        Console.WriteLine("===Display Average By Priority===");
        for (int i = 1; i <= 5; i++)
        {
            if (count[i] == 0)
            {
                Console.WriteLine($"Priority {i}: No reports");
            }
            else
            {
                double average = p[i] / count[i];
                Console.WriteLine($"Priority {i}: {average:F2}");
            }
        }

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
            double average = CalculateAverage(score, countTrue);
            double maxScore = FindMaxScore(score, countTrue);
            double miniScore = FindMinScore(score, countTrue);
            DisplayBasicStatistics(score, countTrue);
            DisplayStatusCounts(status,countTrue);
            DisplayTypeCounts(reportType, countTrue);
            DisplayHighestPriorityApproved(unitName, reportType, priority, score, status, countTrue);
            DisplayAverageByPriority(priority, score, countTrue);

            }


    }

}





