(function ($) {
    /***
    * A sample AJAX data store implementation.
    * Right now, it's hooked up to load all Apple-related Digg stories, but can
    * easily be extended to support and JSONP-compatible backend that accepts paging parameters.
    */
    function RemoteModelItem() {
        // private
        var PAGESIZE = 100;
        var data = { length: 0 };
        var searchstr = "";
        var appkey = "";
        var sortcol = null;
        var sortdir = 1;
        var h_request = null;
        var req = null; // ajax request

        // events
        var onDataLoading = new Slick.Event();
        var onDataLoaded = new Slick.Event();


        function init() {
        }


        function isDataLoaded(from, to) {
            for (var i = from; i <= to; i++) {
                if (data[i] == undefined || data[i] == null) {
                    return false;
                }
            }

            return true;
        }


        function clear() {
            for (var key in data) {
                delete data[key];
            }
            data.length = 0;
        }


        function ensureData(from, to) {
            //alert("from: " + from + " / to: " + to);

            if (req) {
                req.abort();
                for (var i = req.fromPage; i <= req.toPage; i++)
                    data[i * PAGESIZE] = undefined;
            }

            if (from < 0) {
                from = 0;
            }

            var fromPage = Math.floor(from / PAGESIZE);
            var toPage = Math.floor(to / PAGESIZE);

            while (data[fromPage * PAGESIZE] !== undefined && fromPage < toPage)
                fromPage++;

            while (data[toPage * PAGESIZE] !== undefined && fromPage < toPage)
                toPage--;

            if (fromPage > toPage || ((fromPage == toPage) && data[fromPage * PAGESIZE] !== undefined)) {
                // TODO:  look-ahead
                return;
            }

            var t = new Date().getTime();
            var url = "/Data/TestItem/?query=" + searchstr + "&offset=" + (fromPage * PAGESIZE) + "&to=" + PAGESIZE + "&t=" + t;
            //var url = "/Script/TestJsonpData.js";

            if (sortcol != null) {
                url += ("&sort=" + sortcol + "&dir=" + ((sortdir > 0) ? "asc" : "desc"));
            }

            if (h_request != null) {
                clearTimeout(h_request);
            }

            h_request = setTimeout(function () {
                for (var i = fromPage; i <= toPage; i++)
                    data[i * PAGESIZE] = null; // null indicates a 'requested but not available yet'

                onDataLoading.notify({ from: from, to: to });

                //alert(url);

                req = $.ajax({
                    url: url,
                    dataType: "json",
                    success: onSuccess,
                    error: function (xOptions, textStatus) {
                        onError(fromPage, toPage, textStatus)
                    }
                });
                req.fromPage = fromPage;
                req.toPage = toPage;
            }, 50);
        }


        function onError(fromPage, toPage, textStatus) {
            alert("error loading pages - " + textStatus + " : " + fromPage + " to " + toPage);
        }

        function onSuccess(resp, textStatus) {
            var from = data.length;
            var to = from + resp.data.length;
            data.length = to;

            //alert("Data length : " + resp.data.length + " / fromPage : " + req.fromPage + " / total data.length : " + data.length + " / SQL : " + resp.sql );

            for (var i = 0; i < resp.data.length; i++) {
                data[from + i] = resp.data[i];
                data[from + i].index = from + i + 1;
                data[from + i].NO = from + i + 1;
            }

            req = null;

            onDataLoaded.notify({ from: from, to: to });
        }


        function reloadData(from, to) {
            for (var i = from; i <= to; i++)
                delete data[i];

            ensureData(from, to);
        }


        function setSort(column, dir) {
            sortcol = column;
            sortdir = dir;
            clear();
        }

        function setSearch(str) {
            searchstr = str;
            clear();
        }


        init();

        return {
            // properties
            "data": data,

            // methods
            "clear": clear,
            "isDataLoaded": isDataLoaded,
            "ensureData": ensureData,
            "reloadData": reloadData,
            "setSort": setSort,
            "setSearch": setSearch,

            // events
            "onDataLoading": onDataLoading,
            "onDataLoaded": onDataLoaded
        };
    }

    // Slick.Data.RemoteModel
    $.extend(true, window, { Slick: { Data: { RemoteModelItem: RemoteModelItem}} });
})(jQuery);