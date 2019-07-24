using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace RunescapeDailyTracker
{

    class TaskTracker
    {
        private static ObservableCollection<Task> tasks;
        private static ObservableCollection<Task> enabledTasks;
        public static ObservableCollection<Task> Tasks { get => tasks; set => tasks = value; }
        public static ObservableCollection<Task> EnabledTasks { get => enabledTasks; set => enabledTasks = value; }

        public static bool Initialize()
        {
            try
            {
                string jsonText = "";
                StreamReader streamReader;
                streamReader = File.OpenText("Tasks.json");
                while (false == streamReader.EndOfStream)
                {
                    jsonText += streamReader.ReadLine();
                }
                streamReader.Close();//Reads text from json file
                Tasks = JsonConvert.DeserializeObject<ObservableCollection<Task>>(jsonText);//Converts json to list
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;//Return false is open unsuccessful and display message
            }

            Update();
            return true;//Return true if successful
        }

        internal static Task Complete(string taskName)
        {
            Task completedTask = (from task in Tasks where taskName == task.Name select task).First();//Gets task to mark as complete
            completedTask.NotCompleted = false;
            switch (completedTask.Time)
            {
                case Interval.Reset:
                    completedTask.CooldownEnd = DateTime.UtcNow.Date.AddDays(1).ToLocalTime();//Converts UTC midnight tomorrow to localtime
                    break;
                case Interval.WeeklyReset:
                    completedTask.CooldownEnd = DateTime.UtcNow.Date.AddDays((DayOfWeek.Tuesday - DateTime.UtcNow.DayOfWeek + 7) % 7 + 1).ToLocalTime();//Calculates the next UTC midnight on a Wednesday and converts to localtime
                    break;
                case Interval.MonthlyReset:
                    completedTask.CooldownEnd = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.AddMonths(1).Month, 1).ToLocalTime();//Converts UTC midnight of first of next month to localtime
                    break;
                case Interval.Hour24:
                    completedTask.CooldownEnd = DateTime.Now.AddDays(1);//Cooldown ends in 24 hours
                    break;
                case Interval.Hour60:
                    completedTask.CooldownEnd = DateTime.Now.AddHours(60);//Cooldown ends in 60 hours
                    break;
                case Interval.Hour160:
                    completedTask.CooldownEnd = DateTime.Now.AddHours(160);//Cooldown ends in 160 hours
                    break;
                case Interval.Min80:
                    completedTask.CooldownEnd = DateTime.Now.AddMinutes(80);//Cooldown ends in 80 minutes
                    break;
                case Interval.Min270:
                    completedTask.CooldownEnd = DateTime.Now.AddMinutes(270);//Cooldown ends in 270 minutes
                    break;
            }//Runescape is based in UTC timezone

            return completedTask;
        }

        public static void Update()
        {
            enabledTasks = new ObservableCollection<Task>(from task in Tasks where true == task.Enabled select task);//Gets all enabled tasks
        }


    }
}
