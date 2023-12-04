<template>
  <el-input v-model="input_Letter_NO" class="w-50 m-2" placeholder="信函编号" :prefix-icon="Search" clearable />
  <!--表格-->
  <el-table :data="pagedData" style="height: 201px" size="small">
    <el-table-column type="index" label="序号" width="60">
      <template #default="scope">{{ computeIndex(scope.$index) }}</template>
    </el-table-column>
    <el-table-column label="WAPEC编号" prop="NO_AlWaha" width="150" />
    <el-table-column label="MDOC编号" prop="NO_MDOC" width="150" />
  </el-table>
  <!--分页-->
  <el-pagination small="true" v-model="currentPage" :page-size="pageSize" :total="total" layout="total, prev, pager, next"
    @size-change="handleSizeChange" @current-change="handleCurrentChange"></el-pagination>
</template>

<script setup>
import { onMounted, watch, ref, computed } from "vue";
import layoutAPI from "../../api/Layout.js";
const input_Letter_NO = ref(""); // 信函编号
const tableData = ref([]); // 所有数据从数据库获取
const currentPage = ref(1); // 当前页数
const pageSize = ref(5); // 每页显示条数
const total = ref(0); // 总条数

onMounted(async () => {
  await axiosTableData(); // 获取初始数据
});

//获取表格数据
const axiosTableData = async () => {
  await layoutAPI.Get_All_Letter().then((res) => {
    tableData.value = res;
    updateTotalAndPagedData();
  });
};

// 计算筛选后的数据
const filteredData = computed(() => {
  return tableData.value.filter((item) => {
    return item.NO_AlWaha.includes(input_Letter_NO.value);
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
watch(input_Letter_NO, () => {
  updateTotalAndPagedData();
});

// 处理每页条数变化
const handleSizeChange = (newSize) => {
  pageSize.value = newSize;
  currentPage.value = 1; // 重置当前页数
};

// 处理当前页数变化
const handleCurrentChange = (newPage) => {
  currentPage.value = newPage;
};

// 计算固定的序号
const computeIndex = (index) => {
  return (currentPage.value - 1) * pageSize.value + index + 1;
};
</script>
