import { __decorate } from "tslib";
import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators';
import axios from "axios";
const instance = axios.create({
    baseURL: "http://localhost:44300",
    headers: {
        "Content-Type": "application/json",
    }
});
let LogEntries = class LogEntries extends VuexModule {
    logEntries = Array();
    getLogEntriesSuccess(newLogEntries) {
        const logEntries = this.logEntries;
        logEntries.length = 0;
        newLogEntries.forEach(function (logEntry) {
            logEntries.push(logEntry);
        });
    }
    getLogEntries() {
        return instance.get("logEntries").then(response => {
            this.context.commit('getLogEntriesSuccess', response.data);
        });
    }
};
__decorate([
    Mutation
], LogEntries.prototype, "getLogEntriesSuccess", null);
__decorate([
    Action({ rawError: true })
], LogEntries.prototype, "getLogEntries", null);
LogEntries = __decorate([
    Module({ namespaced: true })
], LogEntries);
export default LogEntries;
//# sourceMappingURL=logentries.module.js.map