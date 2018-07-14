dojo.require("dojox.grid.DataGrid");
dojo.require("dojo.data.ItemFileReadStore");
dojo.require("dijit.form.TimeTextBox");
dojo.require("dijit.form.DateTextBox");
dojo.require("dojo.data.ItemFileReadStore");
dojo.require("dojox.charting.themes.Tom");
dojo.require("dijit.form.Select");

$(function () {
    $.datepicker.setDefaults($.datepicker.regional['en-gb']);
            var dates = $("#from, #to").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                //regional:"en-gb",
                numberOfMonths: 3,
                onSelect: function (selectedDate) {
                    var option = this.id == "from" ? "minDate" : "maxDate",
					instance = $(this).data("datepicker"),
					date = $.datepicker.parseDate(
						instance.settings.dateFormat ||
						$.datepicker._defaults.dateFormat,
						selectedDate, instance.settings);
                    dates.not(this).datepicker("option", option, date);
                }
            });
        });

        $(function () {
            $("#from").datepicker($.datepicker.regional["en-gb"]);
            $("#to").datepicker($.datepicker.regional["en-gb"]);
        });

        function plotVehiclePath(deviceData) {
            var p = eval("(" + deviceData + ")");
            var coordinates = new Array();
            for (var i = 0; i < p.length; i++) {
                coordinates.push(new google.maps.LatLng(p[i].LT, p[i].LG));
            }

            var myLatLng = new google.maps.LatLng(51.355725, -0.185963333333333);
            var myOptions = { zoom: 16, center: myLatLng, mapTypeId: google.maps.MapTypeId.ROADMAP };
            var map = new google.maps.Map(document.getElementById("leftcol"), myOptions);

            //vehiclePath.setMap(map);

            //var vehiclePath = new google.maps.Polyline({ path: coordinates, strokeColor: "#FF0000", strokeOpacity: 1.0, strokeWeight: 2 });

            var image = "images/zamtrack_over_speed.png";
            for (var i = 0; i < coordinates.length; i++) {
                if (i == 0) {
                    image = "images/zamtrack_journey_start.png";
                }
                else if (i==coordinates.length-1) {
                    image = "images/zamtrack_journey_end.png";
                }
                else {
                    image = "images/5378-car-icon-library.gif";
                }
                var marker = new google.maps.Marker({ position: coordinates[i], map: map, title: "Zamtrack",icon:image });

            }
        }

        function getSelectedDevice() {
            return document.getElementById("devices").value;
        }

        function getStartDate() {
            return document.getElementById("from").value;
        }

        function getEndDate() {
            return document.getElementById("to").value;
        }

        function addOption(selectbox, text, value) {
            var optn = document.createElement("OPTION");
            optn.text = text;
            optn.value = value;
            selectbox.options.add(optn);
        }

        function bindDeviceList(deviceList) {
            var devices = new Array();
            var p = eval("(" + deviceList + ")");

            for (var i = 0; i < p.length; i++) {
                addOption(document.getElementById("devices"), p[i].DeviceId, p[i].DeviceId);
                addOption(document.getElementById("devicesForSearch"), p[i].DeviceId, p[i].DeviceId);
            }
        }

        function getDevices() {
            var req = new XMLHttpRequest();
            req.open('GET', 'ZamTrackJsonHandler.ashx?&cmd=DISTINCT_DEVICES' + '&timeStamp=' + Number(new Date()), true);
            req.onreadystatechange = function (aEvt) {
                if (req.readyState == 4) {
                    if (req.status == 200) {
                        bindDeviceList(req.responseText);
                    }
                    else
                        alert(req.responseText);
                }
            };
            req.send(null);
        }

        function displayVehiclePathForDateRange() {
            var req = new XMLHttpRequest();
            var timestamp = Number(new Date())
            //get selected deviceId
            req.open('GET', 'ZamTrackJsonHandler.ashx?from=' + getStartDate() + '&to=' + getEndDate() + '&cmd=VEHICLE_PATH_FOR_DATE_RANGE&deviceId=' + getSelectedDevice() + '&timeStamp=' + Number(new Date()), true);
            req.onreadystatechange = function (aEvt) {
                if (req.readyState == 4) {
                    if (req.status == 200) {
                        plotVehiclePath(req.responseText);
                    }
                    else
                        alert("Error loading page...\n");
                }
            };
            req.send(null);
        }

        function initialise() {
            displayVehiclePathForDateRange();
            getDevices();
        }


        $(function () {
            $("#tabs").tabs({
                cookie: {
                    // store cookie for a day, without, it would be a session cookie
                    expires: 1
                }
            });
        });

        $(function () {
            $("#tabs").bind("tabsselect", function (e, tab) {
                if (tab.index == 0||tab.index==2) {
                    loadData();                    
                }
            });
        });

        function formatDegrees(value) {
            return value + '&deg;';
        }

        function formatDate(value) {
            var date = new Date(value);
            return date.toUTCString();
        }

        function formatSpeed(value) {
            if (value <= 70) {
                return '<div style="background-color:#7ACD34;width:100%">' + value + '</div>';
            }
            else if (value >= 80) {
                return '<div style="background-color:#EF2533;width:100%"><b>' + value + '</b></div>';
            }
            if (value > 70 && value < 80) {
                return '<div style="background-color:#FFBF00;width:100%"><b>' + value + '</b></div>';
            }
            return value;
        }

        function onRowClickHandler(evt) {
            var grid = dijit.byId("allDevices");
            var lat = grid.getItem(evt.rowIndex).LT;
            var long = grid.getItem(evt.rowIndex).LG;

            var driver = grid.getItem(evt.rowIndex).DR;
            var date = grid.getItem(evt.rowIndex).DT;
            var speed = grid.getItem(evt.rowIndex).SP;
            var make = grid.getItem(evt.rowIndex).MK;
            var model = grid.getItem(evt.rowIndex).MD;
            var reg = grid.getItem(evt.rowIndex).RG;

            document.getElementById("speedPoint").src = getStaticMapUrl(lat, long, speed) ;

            if (speed > 0) {
                document.getElementById("miniReport").innerHTML = "The driver " + driver + " with " + make + " " + model + " recorded " + speed + " km/hr  on " + date.toString();
            }
            else {
                document.getElementById("miniReport").innerHTML = "The driver " + driver + " with " + make + " " + model + " was stationary  @ " + date.toString();
            }
        }

        function getStaticMapUrl(lat,long,speed) {
            var imgUrl;
            if (speed >= 80) {
                imgUrl = "http://maps.google.com/maps/api/staticmap?maptype=roadmap&center=" + lat + "," + long + "&zoom=16&size=640x300&markers=color:0xEF2533|label:R|" + lat + "," + long + "&sensor=false";
            } else if ((speed > 70) && (speed < 80)) {
                imgUrl = "http://maps.google.com/maps/api/staticmap?maptype=roadmap&center=" + lat + "," + long + "&zoom=16&size=640x300&markers=color:0xFFBF00|label:A|" + lat + "," + long + "&sensor=false";
            } else {
                imgUrl = "http://maps.google.com/maps/api/staticmap?maptype=roadmap&center=" + lat + "," + long + "&zoom=16&size=640x300&markers=color:0x7ACD34|label:G|" + lat + "," + long + "&sensor=false";
            }
            return imgUrl;
        }

        function loadData() {
            var dataStore = new dojo.data.ItemFileReadStore({ url: "ZamTrackJsonHandler.ashx?cmd=ALL_DEVICES", urlPreventCache: true });
            var grid = dijit.byId("allDevices");
            grid.rowsPerPage = 20;

            grid.setStore(dataStore);
            dojo.connect(grid, "onRowClick", onRowClickHandler);

            var fleetEventsGrid = dijit.byId("fleetEvents");
            fleetEventsGrid.rowsPerPage = 20;


            fleetEventsGrid.setStore(dataStore);
            fleetEventsGrid.filter({
                EV: "F*"
            });

            var geofenceGrid = dijit.byId("geofence");
            var geofenceStore = dataStore;
            geofenceGrid.rowsPerPage = 20;
            geofenceGrid.setStore(geofenceStore);

            geofenceGrid.filter({
                EV: "Door*"
            });

        }

        dojo.ready(function () {
            loadData();
        });


        $(function () {
            $.datepicker.setDefaults($.datepicker.regional['en-gb']);
            var dates = $("#from1, #to1").datepicker({
                defaultDate: "+1w",
                changeMonth: true,
                numberOfMonths: 2,
                onSelect: function (selectedDate) {
                    var option = this.id == "from1" ? "minDate" : "maxDate",
					instance = $(this).data("datepicker"),
					date = $.datepicker.parseDate(
						instance.settings.dateFormat ||
						$.datepicker._defaults.dateFormat,
						selectedDate, instance.settings);
                    dates.not(this).datepicker("option", option, date);
                }
            });
        });

        function onSearchResultsRowClickHandler(evt) {
            var grid = dijit.byId("journeySearchResultsGrid");

            var lat = grid.getItem(evt.rowIndex).LT;
            var long = grid.getItem(evt.rowIndex).LG;
            var speed = grid.getItem(evt.rowIndex).SP;

            document.getElementById("searchResultsImg").src = getStaticMapUrl(lat, long, speed);
        }

        function averageVehicleSpeed(data) {
            var totalVehicleSpeed = 0;
            for (var i = 0; i < data.length; i++) {
                totalVehicleSpeed = data[i] + totalVehicleSpeed;
            }
            return totalVehicleSpeed / data.length;
        }

        function plotVehicleSpeed() {
            document.getElementById("simplechart").innerHTML = "";
            var from = (document.getElementById('from1').value);
            var to = (document.getElementById('to1').value);

            var startTime = document.getElementById("time1").value;
            var endTime = document.getElementById("time2").value;

            if (from == "" || to == "") {
                return;
            }

            var deviceId = document.getElementById("devicesForSearch").value;

            var xhrArgs = {
                url: "ZamTrackJsonHandler.ashx?from=" + from + "&to=" + to + "&cmd=DEVICE_DATA&deviceId=" + deviceId + "&startTime=" + startTime + "&endTime=" + endTime,
                handleAs: "json",
                preventCache: true,
                load: function (data) {
                    dojo.require("dojox.charting.Chart2D");
                    makeCharts = function () {
                        document.getElementById("averageSpeedLbl").innerHTML = "Generating graph...";

                        var chart1 = new dojox.charting.Chart2D("simplechart");
                        var chartData = new Array();

                        for (var i = 0; i < data.items.length; i++) {
                            chartData.push(data.items[i].SP);
                        }

                        var dataStore = new dojo.data.ItemFileReadStore({ url: xhrArgs.url });
                        var searchResultsGrid = dijit.byId("journeySearchResultsGrid");
                        searchResultsGrid.setStore(dataStore);
                        dojo.connect(searchResultsGrid, "onRowClick", onSearchResultsRowClickHandler);

                        if (data.items.length > 0) {

                            chart1
                        .addPlot("default", { type: "Lines", tension: "X", lines: true, areas: false, markers: false })
                        .addAxis("x")
                        .addAxis("y", { vertical: true })
                        .addSeries("Series 1", chartData, {
                            stroke: {
                                color: "red",
                                width: 0.5
                            },
                            fill: "#123456"
                        }).render();
                        }
                        document.getElementById("averageSpeedLbl").innerHTML = data.items.length + " results found" + "<br/><h3>Average speed " + averageVehicleSpeed(chartData) + " km/hr </h3>";

                    };

                    dojo.addOnLoad(makeCharts);
                },
                error: function (error) {
                    alert("An unexpected error occurred: " + error);
                }
            }
            var deferred = dojo.xhrGet(xhrArgs);
        }