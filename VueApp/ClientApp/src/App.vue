<template>
  <el-container style="height: 100%">
    <el-aside> <Index_Aside /></el-aside>
    <el-container style="height: 100%">
      <el-header><Index_Header /></el-header>
      <el-main>
        <Index_Main />
        <!-- <test /> -->
      </el-main>
      <el-footer>2023 - Letter App by WE Dept.</el-footer>
    </el-container>
  </el-container>
</template>

<script setup>
  import Index_Header from "./components/views/Index_Header.vue";
  import Index_Main from "./components/views/Index_Main.vue";
  import Index_Aside from "./components/views/Index_Aside.vue";
  import test from "./components/views/test.vue";
</script>

<style>
  html,
  body,
  #app {
    height: 100%;
  }
  body {
    margin: 0px;
  }

  .el-header {
    box-shadow: 0 0.25rem 0.75rem #0000000d;
    width: 100%;
  }
  .el-main {
    padding: 0;
    margin: 0;
    width: 100%;
    height: 100%;
    overflow: auto;
  }
  .el-aside {
    width: 200px !important;
    background-color: #336d86;
    color: #ffffffff;
  }

  .el-footer {
    position: fixed;
    bottom: 0;
    width: 100%;
    white-space: nowrap;
    line-height: 60px;
    border-top: 1px solid #c7b8b8;
    color: #212529bf;
  }
</style>
