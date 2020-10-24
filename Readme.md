Runescape Daily Tracker utilizes a JSON document to manage tasks and their cooldowns.

When providing a JSON document, it should be in the following format
```
[
	{
		"name": "(String)",
		"interval": "(String DailyReset, WeeklyReset, MonthlyReset, ClanReset, 160Hour, 60Hour, 2Day, 1Day, 6Hour, 4Hour, 270Min, 1Hour, 80Min, 5Sec)",
		"recompletable": (Boolean),
		"location": "(String)",
		"enabled": (Boolean),
		"complete": "(String ISODate, Use 0 for new task)"
	},
	{
		"name": "(String)",
		"interval": "(String)",
		"recompletable": (Boolean),
		"subtasks": [
			{
				"location": "(String)",
				"enabled": (Boolean),
				"complete": "(String)"
			}
		],
		"enabled": (Boolean)
	},
]
```
