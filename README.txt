Runescape Daily Tracker utilizes a JSON document to manage tasks and their cooldowns.
When providing a JSON document or adding a new task, it should be in the following format

[
{
	Name: (String),
	Time: (Enum  Min80, Min270, Hour24, Reset, Hour60, Hour160, WeeklyReset, MonthlyReset),
	Location: (Optional String),
	NotCompleted: (Optional Bool),
	Enabled: (Bool),
	Subtasks: (Optional)
	[
		{
			Location: (String),
			NotCompleted: (Bool),
		},
	],
},
]

The root is an array of tasks
The Time property's enum values correspond to varying times in minutes, hours, or based on the in-game reset times.
The NotCompleted property is inverted from the expected Completed property to facilitate ease of use in code and avoid XAML converters. This is only optional if Subtasks is not null.
The Subtasks property contains an array of objects with their own NotCompleted and Location properties. This is used for multi-step tasks.
This json document should be located in the same directory as the RunescapeDailyTracker.exe
If you open the json document later, you may find extra attributes have been added by the application, those can be safely ignored


The main GUI consists of two listboxes, a Tasks and an Enabled Tasks list.

The Tasks list contains a list of checkboxes that indicate whether that task should be tracked
The Enabled Tasks list contains a list of tasks grouped by location.
Each Enabled Task contains a button that should be pressed when the task is completed in-game. Pressing the button will cause it to be dithered and a countdown timer to appear.
When the countdown timer hits zero, the button will be activiated, indicating the task can be completed again.
These timers persist across the application being closed

Testing:
The TasksTest.json contains atleast 1 task of every time enum, tasks with subtasks, tasks without locations, and two tasks purely for testing
The 2 Minute Test and 5 Second Test tasks are useful as the next shortest timeframe is 80 minutes
This can be used to test by renaming it to Tasks.json and placing it in the same directory as the RunescapeDailyTracker.exe



Required Technique Locations:
Methods: TaskTracker.cs Line 19 - The Initialize Method is used to fetch data from the Tasks.json file
Classes: TaskTracker.cs contains the TaskTracker class
Loops: MainWindow.xaml.cs Line 50 - While Loop to update labels
Inheritance: Task.cs implements the INotifyPropertyChanged interface
Strings, Arrays, or Lists: TaskTracker.cs Line 14-15 - Observable Collections of Tasks
						   MainWindow.xaml.cs Line 55 - The countdown DateTime is converted into a String
Model-View-Controller (MVC) software architecture: The MainWindow.xaml is the View, MainWindow.xaml.cs is the Controller, TaskTracker.cs is the Model
MultiThreading: MainWindow.xaml.cs Line 69-71 - Creates a thread to control a countdown label
Searching and Sorting, or LINQ: TaskTracker.cs Line 44 - LINQ used to select Enabled Tasks
Exception Handling: TaskTracker.cs Line 38 - Try/Catch when opening the Tasks.json file
