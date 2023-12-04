<template>
  <el-row class="row_logo">
    <el-col :span="7">
      <el-image :src="logoImg" fit="fill" alt="your-image-alt-text" style="height: 50px; width: 50px"></el-image>
    </el-col>
    <el-col :span="17" style="display: flex; align-items: flex-end; padding: 0 0 8px 8px; font-family: fantasy"> Letter App </el-col>
  </el-row>
  <el-row style="padding: 8px">
    <el-col :span="28">
      <div class="Check-div">
        <el-button
          :class="{ checked: mapStore.state.selectedTags.includes(item) }"
          class="tagsButton"
          v-for="(item, index) in tags"
          :key="item.Id"
          :label="item.Title"
          :disabled="item.disabled"
          size="small"
          round
          @click="toggleSelected(index, item)"
          >{{ item.Title }}</el-button
        >
      </div>
    </el-col>
  </el-row>
</template>
<script setup>
  //定义图标
  import logoImg from "../../assets/logo.png";
  import layoutAPI from "../../api/Layout.js";
  
  //导入store并定义为mapStore
  import { useStore } from "vuex";
  const mapStore = useStore();
  //声明变量和vue方法
  import { ref, onMounted, watch } from "vue";
  const tags = ref([]);
  onMounted(() => {
    layoutAPI.GetLetterTags().then((res) => {
      tags.value = res.map((item) => {
        item.disabled = false;
        return item;
      });
    });
  });
  watch(mapStore.state.selectedTags, (val) => {
    layoutAPI.GetFilterTags(val).then((res) => {
      tags.value
        .filter((tag) => {
          return !res.some((res) => res.Id === tag.Id);
        })
        .forEach((tag) => {
          if (!res.some((res) => res.Id == tag.Id) && val.length > 0) {
            tag.disabled = true;
          } else {
            tag.disabled = false;
          }
        });
    });
  });
  const toggleSelected = (index, item) => {
    if (mapStore.state.selectedTags.includes(item)) {
      mapStore.state.selectedTags.splice(mapStore.state.selectedTags.indexOf(item), 1);
    } else {
      mapStore.state.selectedTags.push(item);
      // layoutAPI.GetFilterTags(item.Id).then((res) => {
      //   // 根据res.id将不存在于res中的button设置为禁用
      //   tags.value
      //     .filter((tag) => {
      //       return !res.some((res) => res.Id === tag.Id);
      //     })
      //     .forEach((tag) => {
      //       tag.disabled = true;
      //     });
      // });
    }
    // 在按钮被点击时，移除按钮的焦点
    document.activeElement.blur();
  };
</script>
<style>
  .row_logo {
    min-height: 60px;
    box-shadow: #0000000d 0px 0px 0.25rem;
    padding-left: 10px;
  }
  .Check-div {
    margin-top: 24px;
  }

  .tagsButton {
    margin: 2px !important;
    zoom: 0.9;
    font-weight: 700 !important;
  }
 
  .tagsButton.checked {
    background-color: var(--el-color-primary-light-8) !important;
    color: var(--el-color-primary) !important;
  }
</style>
