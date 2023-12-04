// 从vuex中导入store的构造方法
import { createStore } from "vuex";

//声明store的三个最基础的组成部分
const state = () => ({
  selectedTags: [],
});
const mutations = {};
const actions = {};

//创建一个新的store实例，然后挂载上面的三个对象，并导出实例
export default createStore({
  state,
  mutations,
  actions,
});
