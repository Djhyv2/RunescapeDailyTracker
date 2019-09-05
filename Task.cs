using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace RunescapeDailyTracker
{
    public enum Interval
    {
        Sec5, Min2, Min80, Min270, Hour24, Reset, Hour60, Hour160, WeeklyReset, MonthlyReset, ClanReset,
        Hour48
    }
    public class Task : INotifyPropertyChanged
    {
        private string name;
        private Interval time;
        private bool notCompleted;
        private string location;
        private bool enabled;
        private List<Task> subTasks;
        private DateTime cooldownEnd;
        private Thread timerThread;
        private bool recompletable;

        public string Name { get => name; set { if (value != name) { name = value; OnPropertyChanged("Name"); } } }
        public Interval Time { get => time; set => time = value; }
        public bool NotCompleted { get => notCompleted; set { if (value != notCompleted) { notCompleted = value; OnPropertyChanged("NotCompleted"); } } }//This is notCompleted to avoid using a converter to invert it in IsEnabled in XAML
        public string Location { get => location; set => location = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public List<Task> SubTasks { get => subTasks; set => subTasks = value; }
        public DateTime CooldownEnd { get => cooldownEnd; set => cooldownEnd = value; }
        [JsonIgnore]
        public Thread TimerThread { get => timerThread; set => timerThread = value; }
        public bool Recompletable { get => recompletable; set => recompletable = value; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }//This method was copied almost exactly from StackOverflow, it enables the properties to be updated in the listbox


    }
}
