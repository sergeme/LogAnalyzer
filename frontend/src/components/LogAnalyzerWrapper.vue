<template>
  <v-container>
    <v-row class="text-center font-weight-bold">
      <v-col cols="4">
        IP
      </v-col>
      <v-col cols="4">
        FQDN
      </v-col>
      <v-col cols="4">
        #
      </v-col>
    </v-row>
    <v-row v-for="logEntry in logEntries" :key="logEntry.id" class="text-center">
      <v-col cols="4">
        {{logEntry.clientIP}}
      </v-col>
      <v-col cols="4">
        {{logEntry.clientFQDN}}
      </v-col>
      <v-col cols="4">
        {{logEntry.visits}}
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
  import Vue from 'vue'
  import LogEntry from '../models/LogEntry';

  export default Vue.extend({
    name: 'LogAnalyzer',
    data: () => ({
      isFetchingData: true,
      logEntries: Array<LogEntry>(),

    }),
    async mounted() {
      await this.fetchData();
    },
    methods: {
      async fetchData() {
        await this.$store.dispatch("LogEntries/getLogEntries");
        this.logEntries = this.$store.state.LogEntries.logEntries;
      }
    }
  })
</script>
