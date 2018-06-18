using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
class FileTask
{
    static void Main(string[] args)
    {
        Manager manager = new Manager(@"C:\filetasker\deposit");
        manager.Run();
    }
}

class Manager
{
    private Dictionary<String, String> fileStatusMap;
    private String directory;
    private List<Job> jobs;
    public Manager(String dir)
    {
        directory = dir;
        fileStatusMap = new Dictionary<String, string>();
        jobs = new List<Job>();
    }
    public void Run()
    {
        while (true)
        {
            ScanDirectory();
            Thread.Sleep(2000);
        }
    }
    private void ScanDirectory()
    {
        try
        {
            var txtFiles = Directory.EnumerateFiles(directory, "*.work");
            Action<Status> callback = (Status status) =>
            {
                Console.WriteLine("{0},{1}", status.i, status.status);
                Console.WriteLine("done work :");
                foreach (int fib in status.doneWork) { Console.Write($" {fib} "); };
            };

            foreach (string currentFile in txtFiles)
            {
                string fileName = currentFile.Substring(directory.Length + 1);
                if (!fileStatusMap.ContainsKey(fileName)){
                    Console.WriteLine("{0},{1}", fileName, currentFile);

                    fileStatusMap[fileName] = "init";
                    jobs.Add(
                        new Job(20,5, callback).work()
                    );
                }
                else
                {
                    Console.WriteLine("no new files");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

}

class Status
{
    public String status;
    public int i;
    public int[] doneWork;

    public Status()
    {
        status = "init";
        i = 0;
        doneWork = new int[i];
    }
    public Status(int iin, int[] doneWorkin, String statusIn)
    {
        i = iin;
        doneWork = doneWorkin;
        status = statusIn;
    }
    
}
class Job
{
    private Status status;
    public Status getStatus() { return status; }

    public Action<Status> progressCallback;
    int reportAfter;
    int getTo;
    public Job(int getToIn, int reportAft, Action<Status> pcb)
    {
        status = new Status();
        progressCallback = pcb;
        reportAfter = reportAft;
        getTo = getToIn;
    }
    public Job work()
    {
        Fibs(getTo, progressCallback);
        return this;
    }
    public void Fibs(int num, Action<Status> CB)
    {
        int[] res = new int[num];
        for(int i = 0; i < num; i++)
        {
            res[i] = Fib(i);
            if (i%reportAfter == 0)
            {
                status = new Status(i, res, "in progress");
                CB(status);
            }
        }
    }
    public int Fib(int num )
    {
        if (num == 0)
        {
            return 0;
        }
        if(num == 1)
        {
            return 1;
        }
        if (num == 2)
        {
            return 1;
        }
        return Fib(num - 1) + Fib(num - 2);
    }
}
