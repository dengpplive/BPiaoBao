var CC = {};
CC.Create = function (superclass, constructor, newmember) {

    var c = {};
    var clazz = function () {
        if (superclass != null)
            superclass.apply(this, arguments);
        if (constructor != null)
            constructor.apply(this, arguments);

    };
    if (superclass != null)
        $.extend(true, c, superclass.prototype);
    if (newmember != null)
        $.extend(true, c, newmember);
    $.extend(true, clazz.prototype, c);

    return clazz;
};

var BaseVM = CC.Create(null, function (metaModel) {
    $.extend(true, this, metaModel);
    this.metaModel = metaModel;
    this.searchForm = ko.mapping.fromJS(this.metaModel.searchForm);
    delete this.searchForm.__ko_mapping__;
    this.grid = {
        rownumbers: true,
        singleSelect: true,
        fit: true,
        url: this.metaModel.urls.search,
        customLoad: false,
        toolbar: '#toolbar',
        pagination: true
    };


}, {
    searchForm: null,
    grid: null,
    searchClick: function () {
        this.grid.datagrid('load', ko.toJS(this.searchForm));
    },
    ajax: function (obj, successCallback, failCallback) {
        var $this = this;
        $.ajax(obj).done(function (response, status) {
            if (successCallback) {
                successCallback.call($this, response, status);
            }
        }).fail(function (e) {
            if (failCallback) {
                failCallback.call($this, e);
            }
            var obj = JSON.parse(e.responseText);
            window.parent.$.messager.alert('提示', obj.ErrorMsg, 'error');
        })
    },
    exportClick: function (exportType) {
        var queryParams = this.grid.datagrid('options').queryParams;
        queryParams.exportType = exportType;
        var url = this.urls.exportexcel + "?" + $.param(queryParams);
        window.open(url, "download");
    },
    clearForm: function (obj) {
        var top = this;
        $.each(obj, function (i, n) {
            if (typeof (n) == "object") {
                top.clearForm(n);
            } else {
                n('');
            }
        });
    },
    alert: function (msg) {
        window.parent.$.messager.alert('提示', msg, 'info');
    },
    confirm: function (msg, callback) {
        window.parent.$.messager.confirm('确认', msg, function (r) {
            if (r) {
                callback()
            }
        });
    }
});


var ViewModel = CC.Create(BaseVM, function () {
    this.grid.fitColumns = true;
    this.grid.queryParams = ko.toJS(this.searchForm);
});