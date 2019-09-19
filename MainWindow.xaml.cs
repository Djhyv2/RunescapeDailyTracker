using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

namespace RunescapeDailyTracker
{
    public partial class MainWindow : Window
    {
        System.Timers.Timer timer;
        public MainWindow()
        {
            InitializeComponent();
            if (false == TaskTracker.Initialize())//Initializes tasks from file
            {
                return;
            }//Will return if intiialization errored
            lstTasks.ItemsSource = TaskTracker.Tasks;//Binds tasks to list


            ICollectionView routeCollectionView = CollectionViewSource.GetDefaultView(TaskTracker.EnabledTasks);
            routeCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("NotCompleted"));
            routeCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Location"));
            routeCollectionView.SortDescriptions.Add(new SortDescription("NotCompleted", ListSortDirection.Descending));
            routeCollectionView.SortDescriptions.Add(new SortDescription("Location", ListSortDirection.Ascending));
            routeCollectionView.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Ascending));
            routeCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            lstRoute.ItemsSource = routeCollectionView;//Binds enabled tasks to sorted and grouped list

            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Start();//Initializes timer
        }

        private void ChkTask_Click(object sender, RoutedEventArgs e)
        {
            bool enabled = (bool)((CheckBox)e.OriginalSource).IsChecked;
            string name = ((CheckBox)e.OriginalSource).Content.ToString();
            TaskTracker.Update(enabled, name);//Update enabled list
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            Task completedTask = TaskTracker.Complete(((Button)sender).Tag.ToString(), AutomationProperties.GetHelpText((Button)sender));//Sets cooldownEnd and completed status of task
            TaskTracker.Save();//Saves after every update
            CollectionViewSource.GetDefaultView(TaskTracker.EnabledTasks).Refresh();//Will move items between groups if needed
            Label label = ((Grid)((Button)sender).Parent).Children.OfType<Label>().First();
            timer.Elapsed += new System.Timers.ElapsedEventHandler((unusedSender, unusedE) => Clock(label, completedTask));//Adds handler to timer
        }

        private void Clock(Label lblTime, Task task)
        {
            if (task.CooldownEnd > DateTime.Now)
            {
                lblTime.Dispatcher.Invoke((Action)(() =>
                {
                    TimeSpan difference = task.CooldownEnd.Subtract(DateTime.Now);
                    lblTime.Content = (0 != difference.Days ? difference.Days.ToString() + "d " : "") + (0 != difference.Hours ? difference.Hours.ToString() + ":" : "") + difference.ToString(@"mm\:ss");//Print difference with optional days and hours
                }));
            }//Continually update label every second
            else
            {
                task.NotCompleted = true;//Reenabled task after cooldown
                lstRoute.Dispatcher.Invoke((Action)(() =>
                {
                    CollectionViewSource.GetDefaultView(TaskTracker.EnabledTasks).Refresh();//Will move items between groups if needed
                }));
                lblTime.Dispatcher.Invoke((Action)(() =>
                {
                    lblTime.Content = "";
                }));
            }
        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            Task associatedTask = (from task in TaskTracker.EnabledTasks where ((Label)sender).Tag.ToString() == task.Name && (AutomationProperties.GetHelpText((Label)sender) == task.Location || String.IsNullOrEmpty(task.Location) ) select task).FirstOrDefault();
            if (null != associatedTask && null != associatedTask.CooldownEnd && associatedTask.CooldownEnd > DateTime.Now)
            {
                timer.Elapsed += new System.Timers.ElapsedEventHandler((unusedSender, unusedE) => Clock((Label)sender, associatedTask));
            }//Starts clock thread if task has an active cooldown
        }//Start time thread on loaded

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !TaskTracker.Save();//Stops cancel if save unsuccessful
        }


    }
}
