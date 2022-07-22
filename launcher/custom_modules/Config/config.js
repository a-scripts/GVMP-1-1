const electron  = require('electron');
const path      = require('path');
const fs        = require('fs');

class Config {
    constructor(p_Options) {
        const l_UserData = (electron.app || electron.remote.app).getPath('userData');
        this.path = path.join(l_UserData, p_Options.configName + '.json');
        this.data = parseDataFile(this.path, p_Options.default);
    }

    get(p_Key) {
        return this.data[p_Key];
    }

    set(p_Key, p_Value) {
        this.data[p_Key] = p_Value;
        fs.writeFileSync(this.path, JSON.stringify(this.data));
    }

    has (p_Key) {
        if (this.data === undefined) {
            return false;
        }

        return (this.data[p_Key] === null || this.data[p_Key] === undefined);
    }
}

function parseDataFile(p_Path, p_Defaults) {
    try {
        const l_Data = JSON.parse(fs.readFileSync(p_Path));
        return l_Data;
    } catch (e) {
        return p_Defaults;
    }
}

module.exports = Config;