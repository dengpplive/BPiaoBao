(function ($) {

    $.fn.my97 = function (options, param) {
        
        if (typeof options == 'string') {
            var method = $.fn.my97.methods[options];
            if (method) {
                return method(this, param);
            }
        }
        
        options = options || {};
        return this.each(function () {

            var s = $.extend({}, $.fn.my97.defaults, $.parser.parseOptions(this), options)

            var state = $.data(this, 'my97');
            if (state) {
                $.extend(state.options, options);
            }
            else {
                $.data(this, 'my97', {
                    options: $.extend({}, $.fn.my97.defaults, $.parser.parseOptions(this), options)
                });
            }
            var state = $.data(this, 'my97');
            var opts = state.options;

            $(this).addClass("Wdate").click(function () {
                
                $.extend(opts, {
                    el: this,
                    onpicked: function () {
                        if (opts.changed)
                            opts.changed.call(this, $(this).val());
                    },
                    oncleared: function () {
                        if (opts.changed)
                            opts.changed.call(this, "");
                    }
                });

                WdatePicker(opts);
            });
        });

        
    }


    $.fn.my97.methods = {
        options: function (jq) {
            var state = $.data(jq, 'my97');
            var opts = state.options;
            return opts;
        },
        setValue: function (jq, value) {
            jq.val(value);
        },
        getValue: function (jq) {
            return jq.val();
        }
    };


    $.fn.my97.defaults = {

    };
})(jQuery);