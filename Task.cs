using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RunescapeDailyTracker
{
    public enum Interval
    {
        Reset, WeeklyReset, MonthlyReset, Hour24, Hour160, Hour60, Min80, Min270  
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

        public string Name { get => name; set { if (value != name) { name = value; OnPropertyChanged("Name"); } } }
        public Interval Time { get => time; set => time = value; }
        public bool NotCompleted { get => notCompleted; set { if (value != notCompleted) { notCompleted = value; OnPropertyChanged("NotCompleted"); } } }//This is notCompleted to avoid using a converter to invert it in IsEnabled in XAML
        public string Location { get => location; set => location = value; }
        public bool Enabled { get => enabled; set => enabled = value; }
        public List<Task> SubTasks { get => subTasks; set => subTasks = value; }
        public DateTime CooldownEnd { get => cooldownEnd; set => cooldownEnd = value; }


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
