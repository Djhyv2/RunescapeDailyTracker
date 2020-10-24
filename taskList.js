const { ipcRenderer } = require('electron');
const Handlebars = require('handlebars');
const Moment = require('moment');
require('moment-duration-format');
const Utilities = require('./utilities');

let timer;

//Handlebars template, print if task is complete
Handlebars.registerHelper('isComplete', (completeTime, options) =>
{
    if (isComplete(completeTime))
    {
        return options.fn(this);
    }
    return '';
});

//Handlebars template, print if task is recompletable or incomplete
Handlebars.registerHelper('isRecompletableOrIncomplete', (task, options) =>
{
    if (!isComplete(task.complete) || task.recompletable)
    {
        return options.fn(task);
    }
    return '';
});

//Handlebars template, html escapes string
Handlebars.registerHelper('escape', (value) => escape(value));

//Sends task to be completed to server
//eslint-disable-next-line no-unused-vars
function completeTask(location, name)
{
    ipcRenderer.send('taskComplete', { location, name });
}

//Sends task to be toggled to server
//eslint-disable-next-line no-unused-vars
function toggleTask(location, name)
{
    ipcRenderer.send('taskToggle', { location, name });
}

//Returns true if time is in future. completeTime represents when task will be available to complete again.
function isComplete(completeTime)
{
    return Moment() < Moment(completeTime);
}

//Adds task to sorted object.
//sortedTasks is in format of {enabled: {complete: [{location: '', tasks: [{taskObject}]}], incomplete: []}, disabled: {}}
//Utilizes arrays and objects as reference to avoid branching at each level
function sortTask(sortedTasks, task)
{
    let enabledObject;
    if (task.enabled)
    {
        Utilities.setIfNull(sortedTasks, 'enabled', {});
        enabledObject = sortedTasks.enabled;
    }
    else
    {
        Utilities.setIfNull(sortedTasks, 'disabled', {});
        enabledObject = sortedTasks.disabled;
    }
    let completeArray;
    if (isComplete(task.complete))
    {
        Utilities.setIfNull(enabledObject, 'complete', []);
        completeArray = enabledObject.complete;
    }
    else
    {
        Utilities.setIfNull(enabledObject, 'incomplete', []);
        completeArray = enabledObject.incomplete;
    }
    const locationArray = completeArray.find((e) => e.location === task.location);
    if (locationArray)
    {
        locationArray.tasks.push(task);
    }
    else
    {
        completeArray.push({ location: task.location, tasks: [task] });
    }
}

//Clears and resets UI on taskComplete
ipcRenderer.on('taskComplete', (_, tasks) =>
{
    clearUI();
    setupUI(tasks);
});

//Clears and resets UI on taskToggle
ipcRenderer.on('taskToggle', (_, tasks) =>
{
    clearUI();
    setupUI(tasks);
});

//Does initial setup after server sends data
ipcRenderer.on('setup', (_, tasks) =>
{
    setupUI(tasks);
});

//Clears UI lists
function clearUI()
{
    clearInterval(timer);
    document.querySelector('#lstCheckboxes').innerHTML = '';
    document.querySelector('#lstIncompleteTasks').innerHTML = '';
    document.querySelector('#lstCompleteTasks').innerHTML = '';
}

//Creates checkbox lists, lists of tasks, and starts timer
function setupUI(tasks)
{
    //Loads handlebars template and loads checkboxes
    let template = document.querySelector('#templateCheckbox').innerHTML;
    let templateScript = Handlebars.compile(template);
    let html = templateScript(tasks);
    document.querySelector('#lstCheckboxes').innerHTML = html;

    //Converts list from json into object sorted by enabled status, completion status, and location
    const sortedTasks = {};
    tasks.forEach((task) =>
    {
        if (task.subtasks)
        {
            //Propogates enabled status, name, and recompletable status from parent to subtask
            task.subtasks.forEach((subtask) =>
            {
                //eslint-disable-next-line no-param-reassign
                subtask.name = task.name;
                //eslint-disable-next-line no-param-reassign
                subtask.recompletable = task.recompletable;
                if (false === task.enabled)
                {
                    //eslint-disable-next-line no-param-reassign
                    subtask.enabled = false;
                }
                sortTask(sortedTasks, subtask);
            });
        }
        else
        {
            sortTask(sortedTasks, task);
        }
    });

    //Sorts arrays of locations by location same
    Object.keys(sortedTasks).forEach((enabledStatus) =>
    {
        Object.keys(sortedTasks[enabledStatus]).forEach((completeStatus) =>
        {
            sortedTasks[enabledStatus][completeStatus].sort((locationObject1, locationObject2) =>
            {
                if (locationObject1.location < locationObject2.location)
                {
                    return -1;
                }
                if (locationObject1.location > locationObject2.location)
                {
                    return 1;
                }
                return 0;
            });
        });
    });

    //Loads handlebars template and loads task lists
    template = document.querySelector('#templateTasks').innerHTML;
    templateScript = Handlebars.compile(template);
    html = templateScript(sortedTasks.enabled.incomplete);
    document.querySelector('#lstIncompleteTasks').innerHTML = html;
    html = templateScript(sortedTasks.enabled.complete);
    document.querySelector('#lstCompleteTasks').innerHTML = html;

    //Create timer that updates cooldowns
    timer = setInterval(() =>
    {
        sortedTasks.enabled.complete.forEach((location) =>
        {
            location.tasks.forEach((task) =>
            {
                const label = document.querySelector(`[data-location='${escape(task.location)}'][data-name='${escape(task.name)}']`);
                const duration = Moment.duration(Moment(task.complete).diff(Moment())).format('d [days] h:mm:ss');
                label.innerHTML = duration;
                //Resets UI when task completed
                if (0 >= duration)
                {
                    clearUI();
                    setupUI(tasks);
                }
            });
        });
    }, 1000);
}

//Send custom signal when DOM and JS are finished loading
ipcRenderer.send('finishedLoad');
