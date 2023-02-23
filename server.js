const { ipcMain } = require('electron');
const path = require('path');
const fs = require('fs');
const Moment = require('moment');

module.exports = class TaskServer
{
    constructor(mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    start()
    {
        this.loadTasks();

        //Custom IPC message is sent from client after all clientside JS has loaded
        ipcMain.on('finishedLoad', () =>
        {
            this.mainWindow.webContents.send('setup', this.tasks);
        });

        //Sets new complete time of task
        ipcMain.on('taskComplete', (_, completedTask) =>
        {
            //Finds task and its interval if it is a subtask
            const task = this.findJsonTask(completedTask);
            let { interval } = task;
            if (!interval)
            {
                const parentTask = this.findJsonTask({ location: 'undefined', name: completedTask.name });
                interval = parentTask.interval;
            }

            switch (interval)
            {
            case 'DailyReset':
                task.complete = Moment.utc().startOf('day').add({ days: 1 }).toISOString();
                break;
            case 'WeeklyReset':
                if (3 > Moment.utc().weekday())
                {
                    task.complete = Moment.utc().startOf('day').weekday(3).toISOString();
                }
                else
                {
                    task.complete = Moment.utc().startOf('day').add({ weeks: 1 }).weekday(3)
                        .toISOString();
                }
                break;
            case 'MonthlyReset':
                task.complete = Moment.utc().startOf('month').add({ months: 1 }).toISOString();
                break;
            case 'ClanReset':
                if (0 === Moment.utc().weekday() && Moment.utc() < Moment.utc().hour(22).minute(30))
                {
                    task.complete = Moment.utc().startOf('day').weekday(0).hour(22)
                        .minute(30)
                        .toISOString();
                }
                else
                {
                    task.complete = Moment.utc().startOf('day').add({ weeks: 1 }).weekday(0)
                        .hour(22)
                        .minute(30)
                        .toISOString();
                }
                break;
            case '217Hour':
                task.complete = Moment.utc().add({ hours: 217 }).toISOString();
                break;
            case '6Day':
                task.complete = Moment.utc().add({ days: 6 }).toISOString();
                break;
            case '160Hour':
                task.complete = Moment.utc().add({ hours: 160 }).toISOString();
                break;
            case '60Hour':
                task.complete = Moment.utc().add({ hours: 60 }).toISOString();
                break;
            case '2Day':
                task.complete = Moment.utc().add({ days: 2 }).toISOString();
                break;
            case '1Day':
                task.complete = Moment.utc().add({ days: 1 }).toISOString();
                break;
            case '20Hour':
                task.complete = Moment.utc().add({ hours: 20 }).toISOString();
                break;
            case '8Hour':
                task.complete = Moment.utc().add({ hours: 8 }).toISOString();
                break;
            case '6Hour':
                task.complete = Moment.utc().add({ hours: 6 }).toISOString();
                break;
            case '270Min':
                task.complete = Moment.utc().add({ minutes: 270 }).toISOString();
                break;
            case '4Hour':
                task.complete = Moment.utc().add({ hours: 4 }).toISOString();
                break;
            case '2Hour':
                task.complete = Moment.utc().add({ hours: 2 }).toISOString();
                break;
            case '1Hour':
                task.complete = Moment.utc().add({ hours: 1 }).toISOString();
                break;
            case '80Min':
                task.complete = Moment.utc().add({ minutes: 80 }).toISOString();
                break;
            case '5Sec':
                task.complete = Moment.utc().add({ seconds: 5 }).toISOString();
                break;
            default:
                console.log(`Invalid Time for ${task}`);
            }

            this.saveTasks();
            this.mainWindow.webContents.send('taskComplete', this.tasks);
        });

        //Toggles enabled status on task
        ipcMain.on('taskToggle', (_, toggledTask) =>
        {
            const task = this.findJsonTask(toggledTask);
            task.enabled = !task.enabled;
            this.saveTasks();
            this.mainWindow.webContents.send('taskToggle', this.tasks);
        });
    }

    //Finds task by name and location, if no location then returns a parent of subtasks
    findJsonTask(unescapedTask)
    {
        const escapedTask = { name: unescape(unescapedTask.name), location: unescape(unescapedTask.location) };
        const jsonTask = this.tasks.find((task) => escapedTask.name === task.name);
        if ([jsonTask.location, 'undefined'].includes(escapedTask.location))
        {
            return jsonTask;
        }
        const jsonSubtask = jsonTask.subtasks.find((subtask) => subtask.location === escapedTask.location);
        return jsonSubtask;
    }

    //Load tasks from json file
    loadTasks()
    {
        this.tasks = JSON.parse(fs.readFileSync(path.resolve(__dirname, 'tasks.json')));
    }

    //Save tasks to json file
    saveTasks()
    {
        fs.writeFileSync(path.resolve(__dirname, 'tasks.json'), JSON.stringify(this.tasks));
    }
};
