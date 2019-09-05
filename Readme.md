Runescape Daily Tracker utilizes a JSON document to manage tasks and their cooldowns.
When providing a JSON document or adding a new task, it should be in the following format

[
{
	Name: (String),
	Time: (Enum  Min80, Min270, Hour24, Reset, Hour60, Hour160, WeeklyReset, MonthlyReset, ClanReset, Hour48),
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
The NotCompleted property is inverted from the expected Completed property to facilitate ease of use in code. This is only optional if Subtasks is not null.
The Subtasks property contains an array of objects with their own NotCompleted and Location properties. This is used for multi-step tasks.
This json document should be located in the same directory as the RunescapeDailyTracker.exe
If you open the json document later, you may find extra attributes have been added by the application, those can be safely ignored