/** 上传图片
 * @method imgUpload 
 * @param {string} upcon 上传控件
 * @param {string} namecon 文件名称显示控件
 * @param {string} showcon 图片显示控件
 * */
function imgUpload(upcon, namecon, showcon) {
    var form = new FormData();
    form.append("imgUpload", $(upcon)[0].files[0]);
    /*https://blog.csdn.net/qq_34709784/article/details/102806547
     https://blog.csdn.net/qq_34720759/article/details/78885657
     */
    $.ajax({
        type: "POST",
        url: "/Fixed_Assets/ImgUpdate",
        processData: false,
        contentType: false,
        data: form,
        dataType: "json",
        success: function (msg) {
            if (msg == '-1') {
                alert("图片保存失败");
            } else {
                $(namecon).val(msg);
                $(showcon).attr("src", URL.createObjectURL($(upcon)[0].files[0]));
            }
        }
    })
}

/**
 * 时间类型转换
 * @method GetTime
 * @param {string} value 要转换的时间
 * @return 转换后时间
 * */
function GetTime(value) {
    var date = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
    return date.toLocaleDateString();
}

/**
 * 时间选择控件
 * @method timemodel
 * @param {string} conname 控件名(input)
 * @param {string} begintime 开始时间
 * @param {string} arr 一周的周几不能选
 */
//https://blog.csdn.net/hizzyzzh/article/details/51212867#datetimepicker%E7%94%A8%E6%B3%95%E6%80%BB%E7%BB%93
function timemodel(conname, begintime, arr) {
    $(conname).datetimepicker('remove');
    $(conname).datetimepicker({
        format: 'yyyy-mm-dd', //显示格式可为yyyymm/yyyy-mm-dd/yyyy.mm.dd
        weekStart: 1, //0-周日,6-周六 。默认为0
        autoclose: true,//选完时间后是否自动关闭
        startView: 2, //打开时显示的视图。0-'hour' 1-'day' 2-'month' 3-'year' 4-'decade'
        minView: 2, //最小时间视图。默认0-'hour'
        maxView: 4, //最大时间视图。默认4-'decade'
        todayBtn: true, //true时"今天"按钮仅仅将视图转到当天的日期。如果是'linked'，当天日期将会被选中。
        todayHighlight: true, //默认值: false,如果为true, 高亮当前日期。
        initialDate: new Date(), //初始化日期.默认new Date()当前日期
        startDate: begintime,//开始时间
        daysOfWeekDisabled: arr,//一周的周几不能选默认值：”, []
        forceParse: false, //当输入非格式化日期时，强制格式化。默认true
        bootcssVer: 3, //显示向左向右的箭头
        language: 'zh-CN', //语言,
        clearBtn: true,//清除按钮
        pickerPosition: "bottom-left"
    });
}

/**
 * 可选择分钟的时间控件
 * @method timemodel_time
 * @param {any} conname 控件名字
 * @param {any} begintime 开始时间
 */
function timemodel_time(conname, begintime) {
    $(conname).datetimepicker('remove');
    $(conname).datetimepicker({
        format: 'yyyy-mm-dd hh:ii:ss',
        autoclose: true,
        /* minView: "month",  *///选择日期后，不会再跳转去选择时分秒 
        language: 'zh-CN',
        dateFormat: 'yyyy-mm-dd',//日期显示格式
        timeFormat: 'HH:mm:ss',//时间显示格式
        todayBtn: 1,
        autoclose: 1,
        minView: 0,  //0表示可以选择小时、分钟   1只可以选择小时
        minuteStep: 15,//分钟间隔1分钟
        startDate: begintime//开始时间
    });
}

//获取资产名称
function SetAssetName(AssetTypeID) {
    $('#AssetName').empty();
    $.get("/Fixed_Assets/SetAssetName", { AssetTypeID: AssetTypeID}, function (msg) {
        $.each(msg, function (index, item) {
            $('#AssetName').append('<option value="' + item.AssetId + '">' + item.AssetName + '</option>')
        })
    })
}

//员工
function SetEmpolyName() {
    $('#EmpolyName').empty();
    $.get("/Fixed_Assets/SetEmpolyName", function (msg) {
        $.each(msg, function (index, item) {
            $('#EmpolyName').append('<option value="' + item.EmpolyId + '">' + item.EmpolyName + '</option>')
        })
    })
}

//区域
function SetAreaName() {
    $('#AreaName').empty();
    $.get("/Fixed_Assets/SetAreaName", function (msg) {
        $.each(msg, function (index, item) {
            $('#AreaName').append('<option value="' + item.AreaId + '">' + item.AreaName + '</option>')
        })
    })
}

//刷新表格
function Refresh_table() {
    $("#tabStu").bootstrapTable('refresh');
}