//The MIT License (MIT)<br /><br />
//Copyright (c) 2014 2sic Internet Solutions GmbH<br />
//<br />
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:<br />
//<br />
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.<br />
//<br />
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

(function () {
    if (window.___WebAPI)
        return;

    window.___WebAPI = {
        Angular: function (Module_Id, Module_API_Name, $http) { return window.___WebAPI.Common(Module_Id, Module_API_Name, "Angular", $http); },
        jQuery: function (Module_Id, Module_API_Name) { return window.___WebAPI.Common(Module_Id, Module_API_Name, "jQuery"); },
        Common: function (ModuleId, ModuleFolder, Framework, $http) {

            var controller;

            if (Framework == "jQuery")
                controller = ___WebAPI.jQueryRepo[ModuleId];
            else
                controller = ___WebAPI.AngularRepo[ModuleId];

            if (!controller) {
                controller = {
                    ModuleId: ModuleId,
                    ModuleFolder: ModuleFolder,
                    webApi: {
                        get: function (controllerAction, params, data, preventAutoFail, returlUrl) {
                            return controller.webApi._action(controllerAction, params, data, preventAutoFail, "GET", returlUrl);
                        },
                        post: function (controllerAction, params, data, preventAutoFail) {
                            return controller.webApi._action(controllerAction, params, data, preventAutoFail, "POST");
                        },
                        "delete": function (controllerAction, params, data, preventAutoFail) {
                            return controller.webApi._action(controllerAction, params, data, preventAutoFail, "DELETE");
                        },
                        put: function (controllerAction, params, data, preventAutoFail) {
                            return controller.webApi._action(controllerAction, params, data, preventAutoFail, "PUT");
                        },
                        _action: function (settings, params, data, preventAutoFail, method, returlUrl) {

                            // Url parameter: autoconvert a single value (instead of object of values) to an ModuleId=... parameter
                            if (typeof params != "object" && typeof params != "undefined")
                                params = params;

                            // If the first parameter is a string, resolve settings
                            if (typeof settings == 'string') {
                                var controllerAction = settings.split('/');
                                var controllerName = controllerAction[0];
                                var actionName = controllerAction[1];

                                if (controllerName == '' || actionName == '') {
                                    var errtext = "Error: controller or action not defined. Will continue with likely errors.";
                                        if (typeof swal !== 'undefined')
                                            swal(errtext);
                                        else
                                            alert(errtext);
                                }
                                settings = {
                                    controller: controllerName,
                                    action: actionName,
                                    params: params,
                                    data: data,
                                    preventAutoFail: preventAutoFail
                                };
                            }

                            var defaults = {
                                method: method == null ? 'POST' : method,
                                params: null,
                                preventAutoFail: false
                            };

                            settings = $.extend({}, defaults, settings);

                            var sf = $.ServicesFramework(ModuleId);


                            if (returlUrl)
                                return controller.webApi.getActionUrl(settings);

                            var promise

                            if (Framework == "jQuery") {
                                promise = $.ajax({
                                    type: settings.method,
                                    dataType: "json",
                                    async: true,
                                    data: JSON.stringify(settings.data),
                                    contentType: "application/json",
                                    url: controller.webApi.getActionUrl(settings),
                                    beforeSend: sf.setModuleHeaders
                                });
                            }
                            else {

                                var _dnnHeaders = {};
                                var tabID = sf.getTabId();
                                var afValue = sf.getAntiForgeryValue();

                                if (tabID > -1) {
                                    _dnnHeaders.ModuleId = ModuleId;
                                    _dnnHeaders.TabId = tabID;
                                }
                                if (afValue)
                                    _dnnHeaders.RequestVerificationToken = afValue;

                                var req = {
                                    method: settings.method,
                                    url: controller.webApi.getActionUrl(settings),
                                    headers: _dnnHeaders,
                                    data: JSON.stringify(settings.data),
                                }

                                promise = $http(req);



                            }

                            if (!settings.preventAutoFail)
                                if (Framework == "jQuery") {
                                    promise.fail(function (result) {
                                        if (window.console)
                                            console.log(result);
                                        var infoText = "Had an error talking to the server (status " + result.status + ").";
                                        infoText += "\n\nFor further debugging view the JS-console or use fiddler. ";
                                        if (typeof swal !== 'undefined')
                                            swal(infoText);
                                        else
                                            alert(infoText);
                                    });
                                }
                                else {
                                    promise.error(function (data, status, headers, config) {

                                        //Handle Licensing
                                        if (status == 403 && data.Message.length && data.Message.indexOf('#') == 0)
                                            location.hash = data.Message;
                                        else if (status != 0) {
                                            if (typeof swal !== 'undefined')
                                                swal(data.MessageDetail);
                                            else
                                                alert(data.MessageDetail);
                                        }
                                    });
                                }
                            return promise;
                        },
                        getActionUrl: function (settings) {
                            var sf = $.ServicesFramework(ModuleId);
                            var ActionUrl;

                            //Framework
                            if (settings.controller.substring(0, 1) == "~")
                                ActionUrl = sf.getServiceRoot("mCommonAngularBootstrap") + settings.controller.replace('~', '') + "/" + settings.action;
                            else
                                ActionUrl = sf.getServiceRoot(ModuleFolder) + settings.controller + "/" + settings.action;

                            if (settings.params != null) {
                                if (typeof settings.params == 'string')
                                    ActionUrl += "?" + settings.params;
                                else if (typeof settings.params == 'object')
                                    ActionUrl += "?" + $.param(settings.params);
                            }
                            return ActionUrl;
                        }
                    }
                };

                if (Framework == "jQuery")
                    ___WebAPI.jQueryRepo[ModuleId] = controller;
                else
                    ___WebAPI.AngularRepo[ModuleId] = controller;
            }

            return controller;
        },

        jQueryRepo: {},
        AngularRepo: {}
    };
})();