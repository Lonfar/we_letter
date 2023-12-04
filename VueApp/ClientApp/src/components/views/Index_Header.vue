<template>
  <el-row :gutter="24" style="height: 100%; width: 100%; align-content: center">
    <el-col :span="23"
      >标签：
      <el-tag class="row-tag" v-for="tag in mapStore.state.selectedTags" :key="tag.Id" @close="handleClose(tag)" closable>
        {{ tag.Title }}
      </el-tag></el-col
    >
    <el-col :span="1">
      <el-button type="primary" @click="drawer.Open()" size="default">新增</el-button>
      <Letter_add ref="drawer" />
    </el-col>
  </el-row>
</template>
<script setup>
  //导入store并定义为mapStore
  import { useStore } from "vuex";
  const mapStore = useStore();
  //定义ref
  import { ref } from "vue";
  //导入自定义组件
  import Letter_add from "../Letter_Add.vue";
  //定义drawer并与Drawer_Add.vue中的drawer进行绑定
  const drawer = ref();
  const handleClose = (tag) => {
    mapStore.state.selectedTags.splice(mapStore.state.selectedTags.indexOf(tag), 1);
  };
</script>
<style>
  .row-tag {
    margin-right: 5px;
  }
</style>
