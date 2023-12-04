import apiClient from "../utils/Request";
//获得已被信函引用标签
const GetLetterTags = () =>
  apiClient
    .get("/home/GetLetterTags")
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });

//根据选择的标签获得其他可选择的标签
const GetFilterTags = (val) =>
  apiClient
    .post("/home/GetFilterTags", { L_Tags: val })
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });
//获得所有标签
const GetTags = () =>
  apiClient
    .get("/home/GetTags")
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });

//查询回复和引用信函
const GetLetterID_For_Rep_Ref = () =>
  apiClient
    .get("/home/GetLetterID_For_Rep_Ref")
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });
//获得所有信函
const Get_All_Letter = () =>
  apiClient
    .get("/home/Get_All_Letter")
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });

//获得所有线路
const Get_All_Lines = () =>
  apiClient
    .get("/home/Get_All_Lines")
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });
//获得相关信函
const Get_Relevant_Letter = (res) =>
  apiClient
    .get("/home/Get_Relevant_Letter", { params: { id: res } })
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });
const Get_Relevant_Lines = (res) =>
  apiClient
    .get("/home/Get_Relevant_Lines", { params: { ids: res } })
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });
export default { GetTags, GetFilterTags, GetLetterID_For_Rep_Ref, GetLetterTags, Get_All_Letter, Get_All_Lines, Get_Relevant_Letter, Get_Relevant_Lines };
