import axios from "axios";

//创建一个axios实例
const apiClient = axios.create({
  // // 在请求地址前面加上 baseURL
  // baseURL: "https://localhost:5001",

  // headers 是即将被发送的自定义请求头
  headers: {
    // "Content-Type": "application/x-www-form-urlencoded;charset=UTF-8",
    "Content-Type": "application/json;charset=UTF-8",
  },

  timeout: 5000, // request timeout

  // `transformRequest` 允许在向服务器发送前，修改请求数据
  // 只能用在 'PUT', 'POST' 和 'PATCH' 这几个请求方法
  // 后面数组中的函数必须返回一个字符串，或 ArrayBuffer，或 Stream
  transformRequest: [
    (data) => {
      // 对 data 进行任意转换处理
      console.log("请求数据格式：", JSON.stringify(data));
      return JSON.stringify(data);
    },
  ],
  // transformResponse 在传递给 then/catch 前，允许修改响应数据
  transformResponse: [
    (data) => {
      // 对 data 进行任意转换处理
      console.log("响应的数据格式：", JSON.parse(data));
      return JSON.parse(data);
    },
  ],
});

// 请求拦截器 (在请求被 then 或 catch 处理前拦截它们。)
apiClient.interceptors.request.use(
  (config) => {
    // 在请求发送之前做一些处理
    // if (config.method === "get") {
    // } else if (config.method === "post") {
    // }
    return config;
  },
  (error) => {
    // 当请求异常时做一些处理
    return Promise.reject(error);
  }
);

// 响应拦截器 (在响应被 then 或 catch 处理前拦截它们。)
apiClient.interceptors.response.use(
  (res) => {
    let data = res.data;
    // 在这里对返回的数据进行处理
    return data;
  },
  (error) => {
    // 对响应错误做点什么
    let message = "";
    if (error && error.response) {
      switch (error.response.status) {
        case 302:
          message = "接口重定向了！";
          break;
        case 400:
          message = "参数不正确！";
          break;
        case 401:
          message = "您未登录，或者登录已经超时，请先登录！";
          break;
        case 403:
          message = "您没有权限操作！";
          break;
        case 404:
          message = `请求地址出错: ${error.response.config.url}`;
          break;
        case 408:
          message = "请求超时！";
          break;
        case 409:
          message = "系统已存在相同数据！";
          break;
        case 500:
          message = "服务器内部错误！";
          break;
        case 501:
          message = "服务未实现！";
          break;
        case 502:
          message = "网关错误！";
          break;
        case 503:
          message = "服务不可用！";
          break;
        case 504:
          message = "服务暂时无法访问，请稍后再试！";
          break;
        case 505:
          message = "HTTP 版本不受支持！";
          break;
        default:
          message = "异常问题，请联系管理员！";
          break;
      }
      console.log(config);
    }
    return Promise.reject(message);
  }
);

export default apiClient;
