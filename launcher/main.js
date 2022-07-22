const { app, autoUpdater, dialog, BrowserWindow } = require('electron')
const path = require('path')

if (require('electron-squirrel-startup')) return;

const m_URL = 'localhost'
autoUpdater.setFeedURL(m_URL)

function createWindow () {
    const mainWindow = new BrowserWindow({
        width: 300,
        height: 500,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            enableRemoteModule: true,
            contextIsolation: true
        },

        resizable: false,
        autoHideMenuBar: true,
        frame: false
    })

    mainWindow.loadFile('./src/index.html')
    mainWindow.center();
    mainWindow.setMenuBarVisibility(false);
}

function createUpdaterWindow() {
    const updaterWindow = new BrowserWindow({
        width: 300,
        height: 450,
        webPreferences: {
            preload: path.join(__dirname, 'preload_updater.js'),
            enableRemoteModule: true,
            contextIsolation: true
        },

        resizable: false,
        autoHideMenuBar: true,
        frame: false
    })

    updaterWindow.loadFile('./src/Dialogs/Updater/index.html')
    updaterWindow.center();
    updaterWindow.setMenuBarVisibility(false);
}

app.whenReady().then(() => {
    autoUpdater.checkForUpdates()

    app.on('activate', function () {
        if (BrowserWindow.getAllWindows().length === 0) createWindow()
    })
})

app.on('window-all-closed', function () {
    if (process.platform !== 'darwin') app.quit()
})

autoUpdater.on('update-not-available', () => {
    createWindow()
})

autoUpdater.on('error', errorMessage => {
    const l_Options = {
        type: "error",
        buttons: ["Ok"],
        title: "Prüfen auf Updates - fehlgeschlagen!",
        message: errorMessage
    }

    dialog.showMessageBoxSync(l_Options)
})

autoUpdater.on('update-available', () => {
    /*const l_Options = {
        type: "info",
        buttons: ["Ok"],
        title: "GVMP Launcher - Update",
        message: "Es wurde ein Update für den GVMP-Launcher gefunden! Es wird nun heruntergeladen..."
    }

    dialog.showMessageBoxSync(l_Options)*/

    createUpdaterWindow()
})

autoUpdater.on('update-downloaded', (event, releaseNotes, releaseName, releaseDate, updateURL) => {
    /*const l_Options = {
        type: "info",
        buttons: ["Ok"],
        title: "GVMP Launcher - Update",
        message: "Update heruntergeladen - der Launcher wird neu gestartet..."
    }

    dialog.showMessageBoxSync(l_Options)*/
    autoUpdater.quitAndInstall()
})