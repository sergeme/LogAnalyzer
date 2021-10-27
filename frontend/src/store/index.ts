import Vue from "vue";
import Vuex from "vuex";

import LogEntries from "./logentries.module";

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    LogEntries
  }
});
