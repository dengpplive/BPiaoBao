ko.bindingHandlers.datagrid = {
    //update: function (element, valueAccessor) {
    //    var value = valueAccessor();
    //    ko.utils.extend(value, {
    //        onLoadError: function (e) {
    //            var obj = JSON.parse(e.responseText);
    //            window.parent.$.messager.alert('提示', obj.ErrorMsg, 'error');
    //        }
    //    });
    //    var $grid = $(element).datagrid(ko.toJS(value));
    //    value.datagrid = function () {
    //        $grid.datagrid.apply($grid, arguments);
    //    }
    //}
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        if (!value.onLoadError) {
            ko.utils.extend(value, {
                onLoadError: function (e) {
                    var obj = JSON.parse(e.responseText);
                    window.parent.$.messager.alert('提示', obj.ErrorMsg, 'error');
                }
            });
        }
        $(element).datagrid(ko.toJS(value));
        if (!value.datagrid) {
            ko.utils.extend(value, {
                datagrid: function (k, v) {
                    return $(element).datagrid(k, v);
                }
            });
        }
    }
}
ko.bindingHandlers.datebox = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        $(element).datebox({
            formatter: function (date) {
                return date.format("yyyy-MM-dd");
            },
            onSelect: function (record) {

                valueAccessor()(record.format("yyyy-MM-dd"));
            }
        })
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        if (value() != '') {
            var d = getDate(value());
            var sd = d.format("yyyy-MM-dd");
            $(element).datebox("setValue", sd);
            value(sd);
        }
    }
};
ko.bindingHandlers.winValue = {
    init: function (element, valueAccessor, allBindingsAccessor) {

        var value = valueAccessor(), allBindings = allBindingsAccessor();
        // var valueUnwrapped = ko.utils.unwrapObservable(value);
        var ev = $(element).val();
        if (ev != "")
            value(ev);
        //ko.bindingHandlers.value.init(element, valueAccessor, allBindingsAccessor);
        //valueAccessor.value($(element).val());
        $(element).blur(function () {
            value($(element).val());
        });

    }
};

ko.bindingHandlers.combobox = {
    update: function (element, valueAccessor, allBindingAccessor) {
        var value = valueAccessor(), allBindings = allBindingAccessor();
        ko.utils.extend(value, {
            onSelect: function () {
                allBindings.comValue($(element).combobox('getValue'));
            }
        })
        $(element).combobox(ko.toJS(value));
        if (allBindings.comValue() != '') {
            $(element).combobox('select', allBindings.comValue());
        }
    }
}
ko.bindingHandlers.my97value = {
    update: function (element, valueAccessor, allBindingAccessor) {
        var value = valueAccessor(), allBindings = allBindingAccessor();

        $(element).my97({
            changed: function (v) {
                value(v);
            }
        }).val(value());

    }
}


ko.bindingHandlers.imgSrc = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor(), allBindings = allBindingsAccessor();
        var v = value(); 
        $(element).attr("src", v);
    },
    update: function (element, valueAccessor, allBindingAccessor) {
        var value = valueAccessor(), allBindings = allBindingAccessor();
        var v = value(); 
        $(element).attr("src", v);

    }
}
