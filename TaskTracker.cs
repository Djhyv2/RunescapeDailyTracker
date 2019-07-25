using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
                if(null == Tasks)
                {
                    return false;
                }//Exit if empty parse
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;//Return false is open unsuccessful and display message
            }

            enabledTasks = new ObservableCollection<Task>(from task in Tasks where true == task.Enabled && null == task.SubTasks select task);//Gets all enabled tasks without subtasks
            foreach (Task task in Tasks)
            {
                if (null != task.SubTasks && true == task.Enabled)
                {
                    foreach (Task subTask in task.SubTasks)
                    {
                        enabledTasks.Add(subTask);
                        subTask.Name = task.Name;
                        subTask.Time = task.Time;//Propogate values to subtasks
                    }
                }
            }//Adds all subtasks to list being shown
            return true;//Return true if successful
        }

        public static Task Complete(string taskName, string location)
        {
            Task completedTask = (from task in Tasks where taskName == task.Name select task).First();//Gets task to mark as complete
            if(null != completedTask.SubTasks)
            {
                foreach(Task subTask in completedTask.SubTasks)
                {
                    if(location == subTask.Location)
                    {
                        completedTask = subTask;
                        break;
                    }
                }
            }//Find if subtask clicked
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
                case Interval.Min2: //2 Minute and 5 Seconds are used only in testing
                    completedTask.CooldownEnd = DateTime.Now.AddMinutes(2);//Cooldown ends in 2 minutes
                    break;
                case Interval.Sec5:
                    completedTask.CooldownEnd = DateTime.Now.AddSeconds(5);//Cooldown ends in 5 seconds
                    break;
            }//Runescape is based in UTC timezone

            return completedTask;
        }

        internal static bool Save()
        {
            try
            {
                string json;
                StreamWriter streamWriter;
                streamWriter = File.CreateText("tasks.json");
                json = Newtonsoft.Json.JsonConvert.SerializeObject(Tasks);
                streamWriter.Write(json);
                streamWriter.Close();//Reads text from json file
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;//Return false if close unsuccessful and display message
            }
        }

        public static List<Task> Update(bool enabled, string name)
        {
            List<Task> changedTasks = new List<Task>();
            if (true == enabled)
            {
                Task newTask = (from task in Tasks where name == task.Name && null == task.SubTasks select task).FirstOrDefault();
                if (null != newTask)
                {
                    enabledTasks.Add(newTask);
                    changedTasks.Add(newTask);
                    return changedTasks;
                }//Adds newly enabled task if no subtasks

                newTask = (from task in Tasks where name == task.Name select task).First();
                if( null != newTask)
                {
                    foreach (Task subTask in newTask.SubTasks)
                    {
                        enabledTasks.Add(subTask);
                        changedTasks.Add(subTask);
                        subTask.Name = name;//Propogate name to subtasks
                    }//Adds subtask of newly checked item
                }
            }
            else
            {
                List<Task> removedTasks = (from task in EnabledTasks where name == task.Name select task).ToList();
                foreach(Task task in removedTasks)
                {
                    enabledTasks.Remove(task);
                    changedTasks.Add(task);
                    task.TimerThread?.Abort();
                }//Removes tasks from list
            }
            return changedTasks;
        }


    }
}
