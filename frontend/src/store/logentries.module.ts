import { VuexModule, Module, Mutation, Action } from 'vuex-module-decorators';
import LogEntry from '../models/LogEntry';
import axios from "axios";
const instance = axios.create({
  baseURL: "http://localhost:44300",
  headers: {
    "Content-Type": "application/json",
  }
})

@Module({ namespaced: true })
class LogEntries extends VuexModule {
  public logEntries: Array<LogEntry> = Array<LogEntry>();

  @Mutation
  public getLogEntriesSuccess(newLogEntries: Array<LogEntry>): void {
    const logEntries = this.logEntries;
    logEntries.length = 0;
    newLogEntries.forEach(function (logEntry) {
        logEntries.push(logEntry);
    })
    }

  @Action({ rawError: true })
  public getLogEntries(): unknown{
    return instance.get("logEntries").then(
      response => {
        this.context.commit('getLogEntriesSuccess', response.data);
      }
    )
    
  }

}
export default LogEntries;
