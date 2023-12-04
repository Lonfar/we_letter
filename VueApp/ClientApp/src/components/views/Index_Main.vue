<template>
  <el-row :gutter="24">
    <el-col :span="8">
      <el-input v-model="input_Letter_NO" class="w-50 m-2" placeholder="信函编号" :prefix-icon="Search" clearable />
    </el-col>
    <el-col :span="8">
      <el-input v-model="input_Title_Desc" class="w-50 m-2" placeholder="标题 / 描述" :prefix-icon="Search" clearable />
    </el-col>
    <el-col :span="8">
      <el-input v-model="input_Letter_Date" class="w-50 m-2" placeholder="信函日期 (XXXX-XX-XX)" :prefix-icon="Calendar" clearable />
    </el-col>
  </el-row>
  <el-row :gutter="24">
    <el-col :span="24">
      <!--表格-->
      <el-table :data="pagedData" style="height: 201px" size="small">
        <el-table-column type="index" label="序号" width="60">
          <template #default="scope">{{ computeIndex(scope.$index) }}</template>
        </el-table-column>
        <el-table-column label="WAPEC编号" prop="NO_AlWaha" width="150" show-overflow-tooltip />
        <el-table-column label="MDOC编号" prop="NO_MDOC" width="150" show-overflow-tooltip />
        <el-table-column label="标题" prop="Title" show-overflow-tooltip />
        <el-table-column label="描述" prop="Description" show-overflow-tooltip />
        <el-table-column label="日期" prop="date" width="120" />
        <el-table-column label="类型" prop="type" width="70" />
        <el-table-column label="标签" prop="tags" show-overflow-tooltip>
          <template #default="scope">
            <el-tag v-for="tag in scope.row.tags" :key="tag.Id" size="small" style="margin-left: 5px">
              {{ tag.Title }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="选择" width="100">
          <template #default="scope">
            <el-button size="small" :icon="Search" circle @click="handleSelect(scope.$index, scope.row)" />
          </template>
        </el-table-column>
      </el-table>
      <!--分页-->
      <el-pagination
        small="true"
        v-model="currentPage"
        :page-size="pageSize"
        :total="total"
        layout="total, prev, pager, next"
        @current-change="handleCurrentChange"></el-pagination>
    </el-col>
  </el-row>
  <el-row :gutter="24">
    <el-col :span="24">
      <div ref="myPage" style="border: #efefef solid 1px; height: calc(100vh - 410px); width: 100%">
        <relation-graph ref="relationGraph$" :options="graphOptions" @click="isShowNodeMenuPanel = false">
          <template #node="{ node }">
            <div
              style="height: 80px; line-height: 100px; border-radius: 50%; cursor: pointer"
              @click="isShowNodeMenuPanel = false"
              @contextmenu.prevent.stop="showNodeMenus(node, $event)"
              @mouseover="showNodeTips(node, $event)"
              @mouseout="hideNodeTips()">
              <el-icon size="30px">
                <component :is="node.data.myicon" />
              </el-icon>
            </div>
            <div
              style="
                color: forestgreen;
                font-size: 16px;
                position: absolute;
                width: 160px;
                height: 25px;
                line-height: 25px;
                margin-top: 5px;
                margin-left: -48px;
                text-align: center;
                background-color: rgba(66, 187, 66, 0.2);
              ">
              {{ node.text }}
            </div>
          </template>
        </relation-graph>
        <div
          v-show="isShowNodeMenuPanel"
          :style="{ left: nodeMenuPanelPosition.x + 'px', top: nodeMenuPanelPosition.y + 'px' }"
          style="
            z-index: 999;
            padding: 10px;
            background-color: #ffffff;
            border: #eeeeee solid 1px;
            box-shadow: 0px 0px 8px #cccccc;
            position: absolute;
          ">
          <div style="line-height: 25px; padding-left: 10px; color: #888888; font-size: 12px">对这封信函进行操作：</div>
          <div class="c-node-menu-item">查看</div>
          <div class="c-node-menu-item">修改</div>
          <div class="c-node-menu-item">下载</div>
          <el-popconfirm
            width="180px"
            confirm-button-text="是"
            confirm-button-type="text"
            cancel-button-text="不, 谢谢"
            cancel-button-type="primary"
            @confirm="DeleteNode()"
            :icon="Delete"
            icon-color="red"
            title="确定删除这封信函?">
            <template #reference>
              <div class="c-node-menu-item" style="color: red">删除</div>
            </template>
          </el-popconfirm>
        </div>
        <div
          v-if="isShowNodeTipsPanel"
          :style="{ left: nodeTipsPanelPosition.x + 'px', top: nodeTipsPanelPosition.y + 'px' }"
          style="
            z-index: 999;
            padding: 10px;
            background-color: #ffffff;
            border: #eeeeee solid 1px;
            box-shadow: 0px 0px 8px #cccccc;
            position: absolute;
          ">
          <div style="line-height: 25px; padding-left: 10px; color: #888888; font-size: 12px">信函信息：</div>
          <div class="c-node-menu-item">WAPEC编号: {{ currentNodeTips.data.NO_AlWaha }}</div>
          <div class="c-node-menu-item">MdOC 编号: {{ currentNodeTips.data.NO_MDOC }}</div>
          <div class="c-node-menu-item">日期: {{ currentNodeTips.data.date }}</div>
        </div>
      </div>
    </el-col>
  </el-row>
</template>

<script setup>
  import { Delete, Search, Calendar } from "@element-plus/icons-vue";
  import { ElMessage } from "element-plus";
  import RelationGraph from "relation-graph/vue3";
  import { onMounted, watch, computed, ref } from "vue";
  import layoutAPI from "../../api/Layout.js";
  import PinyinMatch from "pinyin-match";
  //导入store并定义为mapStore
  import { useStore } from "vuex";
  const mapStore = useStore();

  const input_Letter_NO = ref(""); // 信函编号
  const input_Title_Desc = ref(""); // 信函标题
  const input_Letter_Date = ref(""); // 信函日期

  const tableData = ref([]); // 所有数据从数据库获取
  const currentPage = ref(1); // 当前页数
  const pageSize = ref(5); // 每页显示条数
  const total = ref(0); // 总条数

  const myPage = ref(null);
  const relationGraph$ = ref(RelationGraph);
  const currentNodeMenu = ref(); // 当前右键菜单节点
  const currentNodeTips = ref(); // 当前节点信息
  const isShowNodeMenuPanel = ref(false); // 是否显示节点菜单
  const isShowNodeTipsPanel = ref(false); // 是否显示节点信息
  const nodeMenuPanelPosition = ref({ x: 0, y: 0 }); // 节点菜单面板位置
  const nodeTipsPanelPosition = ref({ x: 0, y: 0 }); // 节点信息面板位置
  const nodesDate = ref([]); // 节点数据
  const linesDate = ref([]); // 连线数据
  const _graph_json_data = ref({}); // 图数据
  const graphOptions = {
    debug: false,
    moveToCenterWhenRefresh: false,
    zoomToFitWhenRefresh: false,
    defaultFocusRootNode: false,
    defaultLineShape: 1,
    defaultLineWidth: 2,
    layouts: [
      {
        layoutName: "force",
        centerOffset_x: 0,
        centerOffset_y: 0,
      },
    ],
  };
  onMounted(async () => {
    await axiosTableData(); // 获取初始数据
  });

  //获取表格数据
  const axiosTableData = async () => {
    await layoutAPI.Get_All_Letter().then((res) => {
      res.forEach((item) => {
        for (let key in item) {
          item.date = item.date.substring(0, 10);
          if (item[key] === null || item[key] === "0001-01-01") {
            item[key] = "-";
          }
        }
      });
      tableData.value = res;
      updateTotalAndPagedData();
    });
  };

  // 计算筛选后的数据
  const filteredData = computed(() => {
    return tableData.value.filter((item) => {
      const noMatch =
        item.NO_AlWaha.toLowerCase().includes(input_Letter_NO.value.toLowerCase()) ||
        item.NO_MDOC.toLowerCase().includes(input_Letter_NO.value.toLowerCase());
      const titleMatch = item.Title.includes(input_Title_Desc.value) || item.Description.includes(input_Title_Desc.value);
      const pinyinTitleMatch =
        PinyinMatch.match(item.Title, input_Title_Desc.value).length > 0 || PinyinMatch.match(item.Description, input_Title_Desc.value).length > 0;
      const dateMatch = item.date.toLowerCase().includes(input_Letter_Date.value.toLowerCase());
      const tagMatch =
        mapStore.state.selectedTags.length === 0 ||
        mapStore.state.selectedTags.every((selectedTags) => item.tags.some((tags) => tags.Title === selectedTags.Title));
      console.log("tagMatch", tagMatch);
      return noMatch && (titleMatch || pinyinTitleMatch) && dateMatch && tagMatch;
    });
  });

  // 更新总条数和分页数据
  const updateTotalAndPagedData = () => {
    total.value = filteredData.value.length;
    currentPage.value = 1; // 重置当前页数
  };

  // 获取分页数据
  const pagedData = computed(() => {
    const startIndex = (currentPage.value - 1) * pageSize.value;
    const endIndex = startIndex + pageSize.value;
    return filteredData.value.slice(startIndex, endIndex);
  });

  // 处理搜索框值变化
  watch([input_Letter_NO, input_Title_Desc, input_Letter_Date, mapStore.state.selectedTags], () => {
    updateTotalAndPagedData();
  });

  // 处理当前页数变化
  const handleCurrentChange = (newPage) => {
    currentPage.value = newPage;
  };
  // 计算固定的序号
  const computeIndex = (index) => {
    return (currentPage.value - 1) * pageSize.value + index + 1;
  };
  //获取选择的table数据和索引
  const handleSelect = async (index, row) => {
    await layoutAPI.Get_Relevant_Letter(row.Id).then((res) => {
      if (res != null) {
        nodesDate.value = res.map((item) => {
          let node = {};

          //替换date格式
          item.date = item.date.substring(0, 10);
          if (item.date === "0001-01-01") {
            item.date = "N/A";
          }
          //如果不是虚构的信函
          if (item.Virtual === false) {
            if (item.type === "新信函") {
              item.myicon = "document";
            } else if (item.type === "回函") {
              item.myicon = "DocumentChecked";
            }
            const { Id, Title, ...rest } = item; // 解构获取 ID 和 Title 属性，并将剩余的属性存储在 rest 变量中
            node = { id: Id, text: Title, data: { ...rest } }; // 将 ID 和 Title 属性替换为小写的 id 和 text，并与剩余属性合并为新对象
          }
          //如果是虚构的信函
          if (item.Virtual === true) {
            item.color = "#E8EB05";
            item.borderColor = "#D4D705";
            const { Id, Title, color, borderColor, ...rest } = item;
            node = {
              id: Id,
              text: Title,
              color: color,
              borderColor: borderColor,
              data: { ...rest },
            };
          }
          if (node.id === row.Id) {
            node.styleClass = "my-node-style";
          }
          return node;
        });
      }
    });
    const ids = nodesDate.value.map((item) => item.id);
    await layoutAPI.Get_Relevant_Lines(JSON.stringify(ids)).then((res) => {
      if (res != null) {
        linesDate.value = res.map((item) => {
          if (item.Text === "回函") {
            item.color = "rgba(213, 38, 38, 0.65)";
          }
          const { Id, From_ID, To_ID, Text, ...rest } = item; // 解构获取 Id, From_ID, To_ID, Text属性，并将剩余的属性存储在 rest 变量中
          return { id: Id, from: From_ID, to: To_ID, text: Text, ...rest }; // 将 Id, From_ID, To_ID, Text属性替换为小写的 id,from，to，text，并与剩余属性合并为新对象
        });
      }
    });
    _graph_json_data.value = { rootId: "1", nodes: nodesDate, lines: linesDate };
    relationGraph$.value.setJsonData(_graph_json_data.value);
    if (_graph_json_data.value.nodes.length > 1) {
      ElMessage({
        message: "选择成功",
        type: "success",
      });
    } else {
      ElMessage({
        message: "该信函没有相关联的信函",
        type: "info",
      });
    }
  };
  const showNodeTips = (nodeObject, $event) => {
    currentNodeTips.value = nodeObject;
    isShowNodeTipsPanel.value = true;
    const _base_position = myPage.value.getBoundingClientRect();
    nodeTipsPanelPosition.value.x = $event.clientX - _base_position.x - 150;
    nodeTipsPanelPosition.value.y = $event.clientY - _base_position.y + 30;
  };
  const hideNodeTips = () => {
    isShowNodeTipsPanel.value = false;
  };
  const showNodeMenus = (nodeObject, $event) => {
    currentNodeMenu.value = nodeObject;
    isShowNodeMenuPanel.value = true;
    const _base_position = myPage.value.getBoundingClientRect();
    nodeMenuPanelPosition.value.x = $event.clientX - _base_position.x + 20;
    nodeMenuPanelPosition.value.y = $event.clientY - _base_position.y + 20;
  };
  const DeleteNode = () => {
    // relationGraph$.value.deleteNode(currentNode.value.id);
    const isNote = true;
    if (isNote) {
      ElMessage({
        message: "已删除信函" + currentNodeMenu.value.text,
        type: "success",
      });
    } else {
      ElMessage({
        message: "删除信函失败" + currentNodeMenu.value.text,
        type: "error",
      });
    }
    isShowNodeMenuPanel.value = false;
  };
</script>

<style lang="scss">
  .c-node-menu-item {
    line-height: 30px;
    padding-left: 10px;
    cursor: pointer;
    color: #444444;
    font-size: 8px;
    border-top: #efefef solid 1px;
  }

  .c-node-menu-item:hover {
    background-color: rgba(66, 187, 66, 0.2);
  }

  @keyframes myFlash {
    from {
      opacity: 1;
    }

    50% {
      opacity: 0.4;
    }

    to {
      opacity: 1;
    }
  }

  @-webkit-keyframes myFlash {
    from {
      opacity: 1;
    }

    50% {
      opacity: 0.4;
    }

    to {
      opacity: 1;
    }
  }

  .my-node-style {
    animation: myFlash 600ms infinite;
    -webkit-animation: myFlash 600ms infinite;
  }
</style>
