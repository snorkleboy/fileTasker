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
    private List<Task<Status>> jobs;
    public Manager(String dir)
    {
        directory = dir;
        fileStatusMap = new Dictionary<String, string>();
        jobs = new List<Task<Status>>();
    }
    public void Run()
    {
        while (true)
        {
            ScanDirectory();
            CheckTasks();
            Thread.Sleep(2000);
        }
    }
    private void CheckTasks()
    {
        List<Task<Status>> todelete = new List<Task<Status>>();
        for(int i =0;i<jobs.Count;i++) 
        {
            Task<Status> task = jobs[i];
            if ( task.Status.ToString() == "RanToCompletion")
            {
                todelete.Add(task);
                Console.WriteLine("\n\nTASK COMPLETE, id:{0}", task.Id);
                Console.WriteLine("RESULT:" );
                foreach(int fib in task.Result.doneWork)
                {
                    Console.Write(" {0} ", fib);
                }
            }

        }
        foreach(Task<Status> task in todelete)
        {
            jobs.Remove(task);
        }
    }
    private void ScanDirectory()
    {
        try
        {
            var txtFiles = Directory.EnumerateFiles(directory, "*.work");
            Action<Status> ProgressCallback = (Status status) =>
            {
                Console.WriteLine("task:{0},i:{1},status:{2}",status.taskName, status.i, status.status);
                Console.WriteLine("done work :");
                foreach (int fib in status.doneWork) { Console.Write($" {fib} "); };
                Console.WriteLine("\n-------");

            };

            foreach (string currentFile in txtFiles)
            {
                string fileName = currentFile.Substring(directory.Length + 1);

                if (!fileStatusMap.ContainsKey(fileName)){
                    Console.WriteLine("contatains {0},{1}", fileName, currentFile);

                    fileStatusMap[fileName] = "init";
                    Task<Status> task = Task<Status>.Factory.StartNew(() => { return new Job(20, 5, ProgressCallback).work().getStatus(); });

                    jobs.Add(
                        task
                    );
                }
                else
                {
                   // Console.WriteLine("no new files");
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
    public int taskName;

    public Status(int taskId)
    {
        status = "init";
        i = 0;
        doneWork = new int[i];
        taskName = taskId;
    }
    public Status(int iin, int[] doneWorkin, String statusIn, int taskId)
    {
        i = iin;
        doneWork = doneWorkin;
        status = statusIn;
        taskName = taskId;
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
        status = new Status((int)Task.CurrentId);
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
                status = new Status(i, res, "in progress", (int)Task.CurrentId);
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
