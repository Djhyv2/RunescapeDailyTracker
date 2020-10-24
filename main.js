const { app, BrowserWindow, Menu } = require('electron');
const Server = require('./server');

//Creates window from html file and creates menubar
app.on('ready', () =>
{
    const mainWindow = new BrowserWindow({
        width: 1920,
        height: 1080,
        webPreferences: {
            nodeIntegration: true,
        },
    });
    mainWindow.loadFile('taskList.html');
    mainWindow.on('closed', () => app.quit());

    const menuTemplate = [{
        label: 'File',
        submenu: [
            {
                label: 'Quit',
                click()
                {
                    app.quit();
                },
            }],
    }];
    if ('production' !== process.env.NODE_ENV)
    {
        menuTemplate.push({
            label: 'Developer Tools',
            accelerator: 'F12',
            click(item, focusedWindow)
            {
                focusedWindow.toggleDevTools();
            },
        });
        menuTemplate.push({
            role: 'reload',
            accelerator: 'Ctrl+R',
        });
    }
    const menu = Menu.buildFromTemplate(menuTemplate);
    Menu.setApplicationMenu(menu);

    new Server(mainWindow).start();
});
