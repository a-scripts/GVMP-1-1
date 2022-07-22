const { contextBridge, remote } = require('electron');

contextBridge.exposeInMainWorld('LauncherAPI', {
    GetVersion: () => {
        return remote.app.getVersion();
    },
    ExitLauncher: () => {
        remote.app.exit(0);
    }
})