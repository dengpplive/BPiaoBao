var d=(document.domain||"").match(/ctrip(travel)?\.com$/);
if(d) window.__uidc_init=new Date*1;
var _=window,
__=document,
___=__.documentElement,
Ctrip={
    module: {}
},
$topWin=_,
$$={};
_.module={};
_.module.event={};(function () {
    try {
        for(;;) {
            var a=$topWin.parent;
            if(a&&a!=$topWin&&a.Ctrip) $topWin=a;
            else break
        }
    } catch(c) { }
})();
$$.browser=function (a) {
    var c=/opera/.test(a),
    h=/chrome/.test(a),
    b=/webkit/.test(a),
    m=!h&&/safari/.test(a),
    g=!c&&/msie/.test(a),
    e=g&&/msie 7/.test(a),
    f=g&&/msie 8/.test(a),
    i=g&&!e&&!f,
    k=!b&&/gecko/.test(a),
    n=k&&/rv:1\.8/.test(a);
    k&&/rv:1\.9/.test(a);
    return {
        IE: g,
        IE6: i,
        IE7: e,
        IE8: f,
        Moz: k,
        FF2: n,
        Opera: c,
        Safari: m,
        WebKit: b,
        Chrome: h
    }
} (navigator.userAgent.toLowerCase());
$extend(Date.prototype,{
    dateValue: function () {
        return new Date(this.getFullYear(),this.getMonth(),this.getDate())
    },
    addDate: function (a) {
        return new Date(this.getFullYear(),this.getMonth(),this.getDate()+a)
    },
    toStdString: function () {
        return this.getFullYear()+"-"+(this.getMonth()+1)+"-"+this.getDate()
    },
    toEngString: function () {
        return "Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec".split("|")[this.getMonth()]+"-"+this.getDate()+"-"+this.getFullYear()
    }
});
$$.module={
    iframe: [],
    list: {},
    tab: {},
    selectAll: {},
    address: {
        source: {}
    },
    calendar: {},
    init: []
};
$$.module.queue={};
function $extend(a) {
    for(var c=1;c<arguments.length;c++) {
        var h=arguments[c],
        b;
        for(b in h) h.hasOwnProperty(b)&&(a[b]=h[b])
    }
    return a
}
_.$s2t=function (a) {
    return a
};

var CtripJsLoader=function (a,c) {
    var h=function () {
        this.jsExecList=[];
        this.mainDom=c.getElementsByTagName("head")[0];
        this.jsInterval=""
    };
    h.prototype={
        isIEorOpera: function () {
            return a.opera&&a.opera.toString()=="[object Opera]"||a.ActiveXObject
        },
        createObject: function (b) {
            var a=b.key,
            g=b.file,
            e;
            this.isIEorOpera()?(e=new Image,e.src=g,e.onerror=function () {
                b.loaded=!0
            }):(e=c.createElement("object"),e.id="object_"+a,e.data=g,this.mainDom.appendChild(e),e.onload=function () {
                b.loaded=!0
            });
            return e
        },
        removeObject: function (b) {
            if(!this.isIEorOpera()&&(b=c.getElementById("object_"+b))) this.mainDom.removeChild(b),
            b.onload=null
        },
        createScript: function (b) {
            var obj=document.getElementById("script_"+b.key);
            if(obj!=null) {
                this.mainDom.removeChild(obj);
            }
            var a=this,
            g=c.createElement("script");
            g.type="text/javascript";
            g.charset=b.charset;
            g.src=b.loaded?b.file:b.bakfile;
            g.id="script_"+b.key;

            this.mainDom.appendChild(g);
            b.created=!0;
            g.readyState?g.onreadystatechange=function () {
                if(g.readyState=="loaded"||g.readyState=="complete") this.onreadystatechange=null,
                a.handleModule(),
                b.callback!=null&&eval(b.callback)(),
                b.exec=!0,
                a.handleScriptLoad(b.key)
            } :g.onload=function () {
                a.handleModule();
                b.callback!=null&&eval(b.callback)();
                b.exec=!0;
                a.handleScriptLoad(b.key)
            }
        },
        handleModule: function () {
            if($$&&$$.module&&$$.module.queue) {
                var b=[],
                a;
                for(a in $$.module.queue) typeof $$.module.queue[a]=="object"&&b.push(a);
                if(b.length!=0) {
                    a=0;
                    for(var c=b.length;a<c;a++) {
                        var e=b[a];
                        if(Ctrip.module[e]) {
                            for(var f=0,
                            h=$$.module.queue[e].length;f<h;f++) new Ctrip.module[e]($$.module.queue[e][f]);
                            delete $$.module.queue[e]
                        }
                    }
                }
            }
        },
        handleScriptLoad: function (a) {
            this.removeObject(a);
            this.jsExecList.shift();
            this.jsExecList.length==0&&clearInterval(this.jsInterval)
        },
        genData: function (a) {
            if(a!=null) {
                for(var c=document.getElementById("releaseno"),c=c==null?"":c.value,g=0,e=a.length;g<e;g++) {
                    var f=a[g];
                    this.jsExecList.push({
                        key: "k"+g,
                        file: f[0]+c,
                        bakfile: f[2]?f[0].replace("webresource.c-ctrip.com","webresource.ctrip.com")+c:f[0]+c,
                        loaded: !1,
                        exec: !1,
                        created: !1,
                        needLoadBak: !1,
                        charset: f[2]?f[1]:"",
                        callback: f[3]
                    })
                }
                return this.jsExecList.concat()
            }
        },
        scriptAll: function (a) {
            var c=this;
            this.orilist=this.genData(a);
            for(var a=0,
            g=this.orilist.length;a<g;a++) this.script(this.orilist[a]);
            this.jsInterval=setInterval(function () {
                c.createScriptAll()
            },
            100)
        },
        script: function (a) {
            this.createObject(a);
            setTimeout(function () {
                a&&!a.loaded&&(a.needLoadBak=!0)
            },
            1500)
        },
        createScriptAll: function () {
            var a=this.jsExecList[0];
            if(!a.created||a.exec) (a.loaded||a.needLoadBak)&&this.createScript(a)
        }
    };
    return h
} (window,document),
adManager=function () {
    this.adsScriptList=[];
    this.count=0
};
adManager.prototype={
    init: function (a) {
        this.isMulti=a.isMulti;
        this.adsList=a.adsList;
        this.g_adArr=[];
        this.genData()
    },
    getKey: function (a) {
        a=a.match(/\?user=([^&]+)/);
        if(a!=null) return a[1];
        return null
    },
    genData: function () {
        for(var a=0,
        c=this.adsList.length;a<c;a++) try {
            this.initScript(this.adsList[a],!1)
        } catch(h) { }
    },
    initScript: function (a,c) {
        var h=this.getKey(a),
        b=$c("script"),
        m=this;
        b.type="text/javascript";
        b.src=a;
        b.id="ads_"+h;
        this.count++;
        document.getElementsByTagName("head")[0].appendChild(b);
        if(c) b.readyState?b.onreadystatechange=function () {
            if(b.readyState=="loaded"||b.readyState=="complete") this.onreadystatechange=null,
            m.handleCallback(this.src)
        } :b.onload=function () {
            m.handleCallback(this.src)
        }
    },
    handleCallback: function (a) {
        if(this.g_adArr.length!=0) a=this.getKey(a),
        $(a).innerHTML=this.g_adArr.join(""),
        this.g_adArr=[],
        this.removeScript(a)
    },
    removeScript: function (a) {
        a=$("ads_"+a);
        document.getElementsByTagName("head")[0].removeChild(a);
        this.count--;
        if(this.count==0) document.write=this.DWFn,
        this.isMulti&&this.initMultiAds()
    },
    getCount: function () {
        return this.count
    },
    initMultiAds: function () {
        function a(a) {
            return function () {
                c(a)
            }
        }
        function c(a) {
            function b() {
                e[j].style.display="none";
                e[j].style.filter="";
                p=!1;
                q=setTimeout(function () {
                    c(null)
                },
                o===null?m:200)
            }
            if(a!==null) if(a==l) return;
            else o=a;
            clearTimeout(q);
            if(!p) if(p=!0,j=l,l=(l+1)%e.length,o!==null&&(l=o),o=null,k.parentNode.insertBefore(e[j],k),e[j].style.position="relative",e[l].style.position="absolute",e[l].style.display="",n[j].className="",n[l].className="pic_current",$$.browser.IE) h.filters[0].apply(),
            e[j].style.display="none",
            h.filters[0].play(),
            b();
            else var f=100,
            i=setInterval(function () {
                f=Math.max(f-g,0);
                e[j].style.opacity=f/100;
                e[j].style.filter="progid:DXImageTransform.Microsoft.Alpha(opacity="+f+")";
                if(!f) clearInterval(i),
                e[j].style.opacity=100,
                b()
            },
            20)
        }
        for(var h=$("adpic"),b=$("adpicBtn"),m=5E3,g=$$.browser.IE?25:5,e=[],f=h.$("div"),i=0;i<f.length;i++) f[i].style.display="none",
        e.push(f[i]);
        var k=document.createElement("a");
        k.style.display="none";
        h.appendChild(k);
        if(e.length) {
            if(b) {
                b.innerHTML="";
                for(var n=[],i=0;i<e.length;i++) {
                    f=$c("li");
                    if(!i) f.className="pic_current";
                    f.innerHTML=i+1;
                    n.push(f);
                    b.appendChild(f);
                    f.onclick=a(i)
                }
            }
            var q,p=!1,
            j=0,
            l=0,
            o=null;
            if($$.browser.IE) h.style.position="relative",
            h.style.filter="progid:DXImageTransform.Microsoft.Fade(duration=1)";
            e[j].style.display="";
            q=setTimeout(function () {
                c(null)
            },
            m)
        }
    }
};
function insertCS() {
    if(d) {
        var a=document.createElement("script");
        a.type="text/javascript";
        a.src="http://www."+(d[1]?"dev.sh."+d[0]:d[0])+"/rp/uiScript.asp";
        document.getElementsByTagName("head")[0].appendChild(a)
    }
}
function insertAds() {
    if(config_adm.length==0) return !1;
    var a=new adManager;
    a.DWFn=document.write;
    document.write=function (b) {
        if(arguments.callee.caller==null) {
            var c=a.getKey(b);
            c!=null&&/^\<a/.test(b)?($(c).innerHTML=b,a.removeScript(c)):a.g_adArr.push(b)
        } else a.DWFn.apply(this,arguments)
    };
    for(var c=0,
    h=config_adm.length;c<h;c++) a.init(config_adm[c])
};