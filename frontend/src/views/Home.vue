<template>
  <log-analyzer-wrapper />
</template>

<script lang="ts">
  import Vue from 'vue'
  import LogAnalyzerWrapper from '../components/LogAnalyzerWrapper.vue'

  export default Vue.extend({
    name: 'Home',

    components: {
      LogAnalyzerWrapper,
    },
  })
</script>
