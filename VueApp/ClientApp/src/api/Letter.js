import apiClient from "../utils/Request";

//添加信函
const AddLetter = (data) =>
  apiClient
    .post("/home/AddLetter", data)
    .then((response) => {
      return response;
    })
    .catch((err) => {
      console.log("错误信息", err.message);
    });

export default { AddLetter };
