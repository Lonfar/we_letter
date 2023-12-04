<template>
    <el-drawer v-model="Visible" title="新增信函" direction="rtl" :before-close="handleClose">
        <el-form :model="form" ref="ruleFormRef" :rules="rules" label-suffix="：" label-width="140px" :inline="false"
            size="normal">
            <el-form-item label="WAPEC 编号" prop="L_LetterMain.NO_AlWaha">
                <el-input v-model="form.L_LetterMain.NO_AlWaha">
                    <!-- <template #prefix>
                        <span>WAPEC-O-</span>
                    </template> -->
                </el-input>
            </el-form-item>
            <el-form-item label="MDOC 编号" prop="L_LetterMain.NO_MDOC">
                <el-input v-model="form.L_LetterMain.NO_MDOC">
                    <template #prefix>
                        <span>MDOC-</span>
                    </template>
                </el-input>
            </el-form-item>
            <el-form-item label="信函日期" prop="L_LetterMain.date">
                <el-date-picker v-model="form.L_LetterMain.date" type="date" placeholder="选择日期" style="width: 100%"
                    format="YYYY/MM/DD" value-format="YYYY/MM/DD" />
            </el-form-item>
            <el-form-item label="是否为回函">
                <el-col :span="3" style="padding-left: 0px">
                    <el-switch v-model="form.L_LetterMain.type" inline-prompt @change="changeSwitch" active-value="回函"
                        inactive-value="新信函" active-text="是" inactive-text="否" />
                </el-col>
                <el-col :span="21" style="padding-right: 0;">
                    <el-form-item prop="Rep">
                        <el-select style="width: 100%;" v-show="selectReplyVisible" v-model="form.Rep" size="small" multiple
                            fit-input-width filterable clearable collapse-tags :max-collapse-tags="1" allow-create
                            placeholder="请选被回函号或者输入新号">
                            <el-option v-for="(item, index) in selectReplyOption" :key="index"
                                :label="item.NO_AlWaha + item.NO_MDOC" :value="item.Id">
                                <span style="float: left">{{ item.NO_AlWaha }}</span>
                                <span style="float: right; color: var(--el-text-color-secondary); font-size: 13px">{{
                                    item.NO_MDOC }}</span>
                            </el-option>
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-form-item>
            <el-form-item label="引用函号" prop="Ref">
                <el-select style="width: 100%" v-model="form.Ref" multiple fit-input-width filterable clearable
                    collapse-tags :max-collapse-tags="1" allow-create placeholder="请选引用函号">
                    <el-option v-for="(item, index) in selectRefOptions" :key="index" :label="item.NO_AlWaha + item.NO_MDOC"
                        :value="item.Id">
                        <span style="float: left">{{ item.NO_AlWaha }}</span>
                        <span style="float: right; color: var(--el-text-color-secondary); font-size: 13px">{{ item.NO_MDOC
                        }}</span>
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="信函标题" prop="L_LetterMain.Title">
                <el-input v-model="form.L_LetterMain.Title" placeholder="信函的英文标题 (EN)" />
            </el-form-item>
            <el-form-item label="中文描述" prop="L_LetterMain.Description">
                <el-input v-model="form.L_LetterMain.Description" type="textarea" placeholder="信函的中文描述 (CN)" />
            </el-form-item>
            <el-form-item label="上传文件">
                <el-upload ref="uploadRef" action="home/Upload" accept=".zip, .rar, .7z" :file-list="show_fileList"
                    :auto-upload="false" :on-change="fileUploadChange" :on-success="fileUploadSuccess"
                    :on-remove="fileUploadRemove" :on-error="fileUploadError">
                    <template #trigger>
                        <el-button size="small" type="primary">选择文件</el-button>
                    </template>
                    <template #tip>
                        <div class="el-upload__tip">只允许上传1个文件！支持格式：.rar .zip .7z</div>
                    </template>
                </el-upload>
            </el-form-item>
            <el-form-item label="信函标签" class="es1">
                <el-row>
                    <el-col span="12">
                        <el-scrollbar max-height="100px" class="es2">
                            <el-tag v-for="(item, index) in form.L_Tags" v-model="form.L_Tags" :key="index" closable
                                :disable-transitions="false" @close="deleteTag(item)">
                                {{ item.Title }}
                            </el-tag>
                            <el-input v-if="inputTagVisible" ref="InputTagRef" class="w-20" v-model="inputTagValue"
                                size="small" @keyup.enter="$event.target.blur()" @blur="InputTag(inputTagValue)" />
                            <el-button v-else size="small" @click="showInputTag"> + New Tag </el-button>
                        </el-scrollbar>
                    </el-col>
                </el-row>
                <el-row>
                    <el-col span="12">
                        <el-scrollbar max-height="150px" style="margin-top: 10px" always="true">
                            <el-check-tag class="Add_CheckTag" v-for="(tag, index) in ButtonTags" :key="index"
                                :checked="form.L_Tags.some((item) => item.Title === tag.Title)"
                                @click="toggleSelected(tag)">
                                {{ tag.Title }}
                            </el-check-tag>
                        </el-scrollbar>
                    </el-col>
                </el-row>
            </el-form-item>
            <el-form-item>
                <el-button @click="handleClose" :loading="loading">取消</el-button>
                <el-button type="primary" :loading="loading" @click="SubmitForm()">{{ loading ? "保存中 ..." : "保存"
                }}</el-button>
            </el-form-item>
        </el-form>
    </el-drawer>
</template>
<script setup>
//定义抽屉、通知、对话框、消息提示
import { ElDrawer, ElNotification, ElMessageBox, ElMessage } from "element-plus";
//定义其他方法类型
import { ref, reactive, nextTick, onMounted } from "vue";
import Guid from "../utils/Guid.js";
import layoutAPI from "../api/Layout.js";
import letterAPI from "../api/Letter.js";
onMounted(() => {
    layoutAPI.GetTags().then((res) => {
        ButtonTags.value = res;
    });
    layoutAPI.GetLetterID_For_Rep_Ref().then((res) => {
        selectRefOptions.value = selectReplyOption.value = res;
    });
});

//定义ButtonTags变量
const ButtonTags = ref([]);
const inputTagValue = ref("");
const inputTagVisible = ref(false);
const InputTagRef = ref();
const selectReplyVisible = ref(false);
const selectReplyOption = ref([]);
const selectRefOptions = ref([]);

//#region 上传功能

//定义上传组件
const uploadRef = ref();
const show_fileList = ref([]);

//fileUploadChange文件状态改变时的钩子，添加文件、上传成功和上传失败时都会被调用
const fileUploadChange = (file, fileList) => {
    //判断上传的文件是否相同,如果相同则删除最后一个
    if (fileList.some((item) => item.name === file.name && item.uid !== file.uid)) {
        fileList.pop();
        ElNotification({
            title: "消息",
            message: `文件 ${file.name} 已存在！`,
            type: "error",
        });
        return false;
    }
    //判断上传的文件是否大于30MB
    if (fileList.some((item) => item.size > 30 * 1024 * 1024)) {
        fileList.pop();
        ElNotification({
            title: "消息",
            message: `文件 ${file.name} 大小超过30MB！`,
            type: "error",
        });
        return false;
    }
    //只允许上传一个文件
    if (fileList.length > 1) {
        fileList.pop();
        ElNotification({
            title: "消息",
            message: `只允许上传一个文件！`,
            type: "error",
        });
        return false;
    }
    form.L_Annexes = fileList.map((item) => {
        return { FileName: item.name.split(".").shift(), FileSuffix: file.name.split(".").pop(), FileSize: (item.size / (1024 * 1024)).toFixed(2) };
    });
};

//fileUploadSuccess上传文件成功时触发
const fileUploadSuccess = (res, file, fileList) => {
    ElNotification({
        title: "消息",
        message: `文件 ${file.name} 上传成功！`,
        type: "success",
    });
};

//fileUploadError文件上传失败时触发
const fileUploadError = (err, file, fileList) => {
    ElNotification({
        title: "消息",
        message: `文件 ${file.name} 上传失败！`,
        type: "error",
    });
};


//fileUploadRemove文件列表移除文件时的钩子
const fileUploadRemove = (file, fileList) => {
    ElNotification({
        title: "消息",
        message: `文件 ${file.name} 已被移除！`,
        type: "warning",
    });
    form.L_Annexes = fileList.map((item) => {
        return { FileName: item.name.split(".").shift(), FileSuffix: file.name.split(".").pop(), FileSize: (item.size / (1024 * 1024)).toFixed(2) };
    });
};



//#endregion

//#region 外观行为

//定义Visible变量，用于控制抽屉的显示与隐藏
const Visible = ref();
//定义loading变量，用于控制保存按钮的loading状态
const loading = ref(false);

//switch开关更改触发
const changeSwitch = (value) => {
    if (value === "回函") {
        selectReplyVisible.value = true;
        //动态更改L_LetterMain.reply
        rules.Rep[0].required = true;
    } else {
        selectReplyVisible.value = false;
        rules.Rep[0].required = false;
    }
    form.Rep = null;
};
//显示输入标签
const showInputTag = () => {
    inputTagVisible.value = true;
    nextTick(() => {
        InputTagRef.value.input.focus();
    });
};
//添加标签
const InputTag = (val) => {
    if (val.trim() === "") {
        ElMessage.warning("标签不能为空");
        inputTagVisible.value = false;
        inputTagValue.value = "";
        return;
    }
    const s = form.L_Tags.some((item) => item.Title.toLowerCase().trim() === val.toLowerCase().trim());
    const b = ButtonTags.value.some((item) => item.Title.toLowerCase().trim() === val.toLowerCase().trim());
    if (s || b) {
        ElMessage.warning("标签已存在");
    } else {
        form.L_Tags.push({ Title: val.trim(), L_Letter_Tags: [] });
        inputTagVisible.value = false;
        inputTagValue.value = "";
    }
};
//删除标签
const deleteTag = (tag) => {
    form.L_Tags.splice(form.L_Tags.indexOf(tag), 1);
};
//选择el-check-tag后添加至标签
const toggleSelected = (tag) => {
    if (form.L_Tags.includes(tag)) {
        deleteTag(tag);
    } else {
        form.L_Tags.push(tag);
    }
};

//#endregion

//#region 表单验证和表单数据的定义

//定义表单数据
const form = reactive({
    L_LetterMain: {
        Title: "",
        Description: "",
        date: Date,
        NO_AlWaha: "",
        NO_MDOC: "",
        type: "",
        Virtual: Boolean,
    },
    L_Tags: [],
    L_Annexes: [],
    Rep: [],
    Ref: [],
});
//重置表单数据和表单规则
const resetForm = () => {
    ruleFormRef.value.resetFields();
    selectReplyVisible.value = false;
    form.L_LetterMain.type = "";
    form.L_Tags.length = 0;
    uploadRef.value.clearFiles();
};

//这个ruleFormRef为获取表单的对象ref值,用于对表单规则是否通过进行校验,ruleFormRef绑定到form的ref标签上
const ruleFormRef = ref(null);
//定义表单规则
const rules = reactive({
    // "L_LetterMain.NO_MDOC": [{ required: true, message: "请输入MdOC编号", trigger: "change" }],
    "L_LetterMain.NO_AlWaha": [{ required: true, message: "请输入Al-Waha的编号", trigger: "change" }],
    "L_LetterMain.date": [{ required: true, type: "date", message: "请选择信函日期", trigger: "change" }],
    "L_LetterMain.Title": [{ required: true, message: "请输入标题", trigger: "change" }],
    "L_LetterMain.Description": [{ required: true, message: "请输入描述", trigger: "change" }],
    Rep: [{ required: true, message: "请选择回函No.", trigger: "change" }],
    Ref: [{ required: false, message: "请选择参考No.", trigger: "change" }],
    "L_Tags.Title": [
        {
            required: true,
            validator(rule, value, callback) {
                if (inputTagValue.value == "" && form.L_Tags == 0) {
                    callback(new Error(this.message));
                } else {
                    callback();
                }
            },
            message: "请定义标签",
            trigger: "change",
        },
    ],
});

//#endregion

//定义Open方法，用于打开抽屉（父窗口的按键需要调用该方法）
const Open = () => {
    Visible.value = true;
};

//vue3中规定，使用了 <script setup> 的组件是默认私有的：一个父组件无法访问到一个使用了 <script setup> 的子组件中的任何东西，除非子组件在其中通过 defineExpose 宏显式暴露
defineExpose({
    Open,
});

//关闭窗口
const handleClose = () => {
    ElMessageBox.confirm("是否关闭窗口？", "确认框", {
        confirmButtonText: "是",
        cancelButtonText: "不，谢谢",
        type: "warning",
    })
        .then(() => {
            loading.value = false;
            Visible.value = false;
            // resetForm();
        })
        .catch(() => {
            //取消或关闭
        });
};

//保存表单
const SubmitForm = () => {
    if (loading.value) {
        return;
    }
    //校验表单是否通过
    ruleFormRef.value.validate((valid) => {
        if (valid) {
            //通过则弹出确认框
            ElMessageBox.confirm("是否保存信函?", "确认框", {
                confirmButtonText: "是",
                cancelButtonText: "不，谢谢",
                type: "warning",
            })
                .then(() => {
                    loading.value = true;
                    // 动画关闭需要一定的时间
                    setTimeout(() => {
                        //向form中添加L_Lines，传输数据用
                        form.L_Lines = [];

                        //判断是否有回函
                        if (Array.isArray(form.Rep)) {
                            //映射数据至template
                            let template = form.Rep.map((item) => {
                                //判断是否为GUID
                                if (Guid.isGUID(item)) {
                                    return { To_ID: item, Text: "回函" };
                                }
                            }).filter(Boolean);
                            form.L_Lines.push(...template);
                            //映射数据至form.Rep并传递至后台作为新添加信函值
                            form.Rep = form.Rep.map((item) => {
                                if (!Guid.isGUID(item)) {
                                    return { NO_AlWaha: item, type: "新信函", Virtual: true };
                                }
                            }).filter(Boolean);
                        }

                        //判断是否有参考
                        if (Array.isArray(form.Ref)) {
                            //映射数据至template
                            let template = form.Ref.map((item) => {
                                //判断是否为GUID
                                if (Guid.isGUID(item)) {
                                    return { To_ID: item, Text: "引用" };
                                }
                            }).filter(Boolean);
                            form.L_Lines.push(...template);
                            //映射数据至form.Ref并传递至后台作为新添加信函值
                            form.Ref = form.Ref.map((item) => {
                                if (!Guid.isGUID(item)) {
                                    return { NO_AlWaha: `WAPEC-O-${item}`, type: "新信函", Virtual: true };
                                }
                            }).filter(Boolean);
                        }
                        form.L_LetterMain.NO_AlWaha = form.L_LetterMain.NO_AlWaha.trim();
                        form.L_LetterMain.NO_MDOC = form.L_LetterMain.NO_MDOC.trim();


                        //提交表单
                        letterAPI
                            .AddLetter(form)
                            .then((res) => {
                                if (res.ResultType == 0) {
                                    ElNotification({
                                        title: "消息",
                                        message: res.Message,
                                        type: "success",
                                    });
                                    //手动上传文件
                                    uploadRef.value.submit();
                                    resetForm();
                                } else if (res.ResultType == 1) {
                                    ElNotification({
                                        title: "消息",
                                        message: res.Message,
                                        type: "info",
                                    });
                                }
                            })
                            .catch((err) => {
                                ElNotification({
                                    title: "消息",
                                    message: err,
                                    type: "error",
                                });
                            });
                        loading.value = false;
                        Visible.value = false;
                        // resetForm();
                    }, 2000);
                })
                .catch(() => {
                    //取消或关闭
                });
        } else {
            //不通过则提示错误
            ElMessage.error({
                dangerouslyUseHTMLString: true,
                message: "<h3>错误：请输入正确信息！</h3>",
            });
            return false;
        }
    });
};
</script>

<style>
.el-form-item__content {
    line-height: 20px !important;
}

.el-upload__tip {
    margin: 0 !important;
}

.el-upload-list {
    zoom: 0.8;
}

.el-upload-list__item-file-name {
    width: 300px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    text-align: left;
}

.w-20 {
    width: 5rem !important;
}

.es1 .el-form-item__content {
    flex-direction: column;
    align-items: normal;
}

.es2 .el-scrollbar__view {
    display: flex;
    flex-wrap: wrap;
    gap: 0.25rem !important;
}

.Add_CheckTag {
    zoom: 0.8;
    margin: 2px;
    border-radius: 50px !important;
}
</style>
