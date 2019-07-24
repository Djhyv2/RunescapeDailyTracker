using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace RunescapeDailyTracker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TaskTracker.Initialize();//Initializes tasks from file
            lstTasks.ItemsSource = TaskTracker.Tasks;//Binds tasks to list
            lstRoute.ItemsSource = TaskTracker.EnabledTasks;//Binds enabled tasks to list
        }

        private void ChkTask_Click(object sender, RoutedEventArgs e)
        {
            TaskTracker.Update();//Update enabled list
            lstRoute.ItemsSource = TaskTracker.EnabledTasks;//Binds enabled tasks to list
        }

        private void BtnComplete_Click(object sender, RoutedEventArgs e)
        {
            Thread timerThread;
            Task completedTask = TaskTracker.Complete(((Button)sender).Tag.ToString());//Sets cooldownEnd and completed status of task

            Label taskLabel = ((Grid)((Button)sender).Parent).Children.OfType<Label>().First();//Gets label that is sibling of button

            timerThread = new Thread(() => Clock(taskLabel, completedTask.CooldownEnd));//Passes sibling label and new task cooldown to thread
            timerThread.IsBackground = true;
            timerThread.Start();//Creates and Starts Thread to run timer, background to allow for graceful exit

        }

        private void Clock(Label lblTime,DateTime cooldownEnd)
        {
            while(true)
            {
                lblTime.Dispatcher.Invoke((Action)(() => lblTime.Content = cooldownEnd.Subtract(DateTime.Now).ToString()));
                Thread.Sleep(1000);
            }//Continually update label every second
        }
    }
}
