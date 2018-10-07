//var angularFormsApp = angular.module('angularFormsApp', ['ui.router']);
var routerApp = angular.module('angularFormsApp', ['ui.router']);

routerApp.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/home');

    $stateProvider.state('home', {
        url: '/home',
        templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/Home.html',
        controller: 'moslem'
    })
        .state('Rooms', {
            url: '/Rooms',
            views: {
                '': {
                    templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/RoomsLists.html',
                    controller: 'ctrRoomsLists'
                }

            }
        }).state('Cards', {
            url: '/Cards',
            views: {
                '': {
                    templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/Cards.html',
                    controller: 'ctrCards'
                }

            }
        }).state('Hotels', {
            url: '/Hotels',
            views: {
                '': {
                    templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/PackageLists.html',
                    controller: 'ctrPackageLists'
                }
            }
        }
        ).state('ShowPackage', {
            url: '/ShowPackage/:id',
            views: {
                '': {
                    templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/PackageDetails.html',
                    controller: 'ctrPackageDetails'
                }
            }
        }).state('ShowRoomsPackage', {
            url: '/ShowRoomsPackage/:id',
            views: {
                '': {
                    templateUrl: '/DesktopModules/MVC/GDSModuleMVC/content/Angular/RoomsPackageDetails.html',
                    controller: 'ctrRoomsPackageDetails'
                }
            }
        });


});
routerApp.controller('ctrCards', function ($scope, $http, $stateParams) {
    $.ajax({
        url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetCards",
        method: "Get",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        }
    }).done(function (data) {
        //data = JSON.parse(data);
        $scope.Packages = data;
        $scope.$apply();

    }).error(function () {
        alert(data);
        $scope.$apply();
    });
    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllPackages",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        data: { id: $stateParams.id },
        success: function (data2) {
            $scope.lstPackages = data2;
            $scope.$apply();
        }
    });
    $scope.AddCard = function () {

        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/AddCard",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { PassKey: $scope.PassKey, Description: $scope.Description, PackageID: $scope.Package.ID },
            success: function (data2) {
                $.ajax({
                    url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetCards",
                    method: "Get",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    }
                }).done(function (data) {
                        //data = JSON.parse(data);
                        $scope.Packages = data;
                        $scope.$apply();

                    }).error(function () {
                        $("#hotel").empty();
                        $scope.default = "خطا";
                        $scope.lstRoom = '';
                        $scope.$apply();
                    });
                $scope.$apply();

                return false;
            }
        });
        return false;
    };


});

routerApp.controller('ctrRoomsPackageDetails', function ($scope, $http, $stateParams) {
    $scope.RemoveRoominRoomsPackage = function (id) {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/RemoveRoominRoomsPackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { id: id },
            success: function (data2) {
                $.ajax({
                    url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetRoomsintoRoomsPackage?PackageID=" + $stateParams.id,
                    method: "Get",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    }
                })
                    .done(function (data) {
                        //data = JSON.parse(data);
                        $scope.Packages = data;
                        $scope.$apply();

                    })
                $scope.$apply();

            }
        });
    };
    $scope.AddRoominRoomPackage = function () {

        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/AddRoomsintoRoomsPackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { HotelID: $scope.mdlHotel.value, RoomID: $scope.mdlRoom.value, PackageID: $stateParams.id, Description: $scope.mdlCity.text + " " + $scope.mdlHotel.text + " " + $scope.mdlRoom.text  },
            success: function (data2) {
                $.ajax({
                    url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetRoomsintoRoomsPackage?PackageID=" + $stateParams.id,
                    method: "Get",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    }
                })
                    .done(function (data) {
                        //data = JSON.parse(data);
                        $scope.Packages = data;
                        $scope.$apply();

                    }).error(function () {
                        $("#hotel").empty();
                        $scope.default = "خطا";
                        $scope.lstRoom = '';
                        $scope.$apply();
                    });
                $scope.$apply();

                return false;
            }
        });
        return false;
    };
    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/GDS/getCity",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        success: function (data) {
            data = JSON.parse(data);
            $scope.lstCity = data.aaData;
            $scope.$apply();
        }
    });
    $.ajax({
        url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetRoomsintoRoomsPackage?PackageID=" + $stateParams.id,
        method: "Get",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        }
    })
        .done(function (data) {
            //data = JSON.parse(data);
            $scope.Packages = data;
            $scope.$apply();

        }).error(function () {
            $("#hotel").empty();
            $scope.default = "خطا";
            $scope.lstRoom = '';
            $scope.$apply();
        });


    $scope.CityChanged = function () {
        $("#hotel").prop("disabled", true);
        $("#hotel").empty()
        $('<option value="null">لطفا صبر کنید</option>').appendTo("#hotel");
        console.log('I am called');
        var rvtoken = $("input[name='__RequestVerificationToken']").val();


        $.ajax({
            url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/GDS/gethotel?cityid=" + $scope.mdlCity.value,
            method: "Get",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            }
        })
            .done(function (data) {
                data = JSON.parse(data);
                $scope.lstHotel = data.aaData;
                $scope.$apply();

            }).error(function () {
                $("#hotel").empty();
                $scope.default = "خطا";
                $('<option value="null">شهر دیگری را انتخاب نمایید</option>').appendTo("#hotel");
                $scope.lstHotel = '';
                $scope.$apply();
            });
    };
    $scope.HotelChanged = function () {
        var rvtoken = $("input[name='__RequestVerificationToken']").val();


        $.ajax({
            url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/GDS/GetRoomsInhote?HotelID=" + $scope.mdlHotel.value + "&CityID=" + $scope.mdlCity.value,
            method: "Get",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            }
        })
            .done(function (data) {
                data = JSON.parse(data);
                $scope.lstRoom = data.aaData;
                $scope.$apply();

            }).error(function () {
                $("#hotel").empty();
                $scope.default = "خطا";
                $scope.lstRoom = '';
                $scope.$apply();
            });
    };
});
routerApp.controller('ctrPackageDetails', function ($scope, $http, $stateParams) {
    $scope.AddRoomPackageintoPackage = function () {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/AddRoomPackageIntoPackages",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { RoomPackageID: $scope.mdlRoomsPackage, PackageID: $stateParams.id },
            success: function (data2) {
                $.ajax({
                    url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetRoomPackageinPackage?PackageID=" + $stateParams.id,
                    method: "Get",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    }
                })
                    .done(function (data) {
                        //data = JSON.parse(data);
                        $scope.Packages = data;
                        $scope.$apply();

                    }).error(function () {
                        $("#hotel").empty();
                        $scope.default = "خطا";
                        $scope.lstRoom = '';
                        $scope.$apply();
                    });
                $scope.$apply();

                return false;
            }
        });
        return false;
    };
    $.ajax({
        url: siteRoot + "DesktopModules/MVC/GDSModuleMVC/Package/GetRoomPackageinPackage?PackageID=" + $stateParams.id,
        method: "Get",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        }
    })
        .done(function (data) {
            //data = JSON.parse(data);
            $scope.Packages = data;
            $scope.$apply();

        }).error(function () {
            $("#hotel").empty();
            $scope.default = "خطا";
            $scope.lstRoom = '';
            $scope.$apply();
        });
    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetPackageDetails",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        data: { id: $stateParams.id },
        success: function (data2) {
            //alert('با موفقیت افزوده شد');
            $scope.$apply();

        }
    });
    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllRoomsPackages",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        data: { id: $stateParams.id },
        success: function (data2) {
            $scope.lstRoomsPackage = data2;
            $scope.$apply();
        }
    });


});
routerApp.controller('ctrPackageLists', function ($scope, $http) {
    $scope.RemovePackage = function (id) {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/RemovePackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { id: id },
            success: function (data2) {
                //alert('با موفقیت افزوده شد');
                $scope.$apply();
                $.ajax({
                    url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllPackages",
                    type: "GET",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    },
                    //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
                    success: function (data2) {
                        $scope.Packages = data2;
                        $scope.$apply();
                        return false;
                    }
                });
                return false;
            }
        });
        return false;
    }
    $scope.AddPackage = function () {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/AddPackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
            success: function (data2) {
                //alert('با موفقیت افزوده شد');
                $scope.$apply();
                $.ajax({
                    url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllPackages",
                    type: "GET",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    },
                    //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
                    success: function (data2) {
                        $scope.Packages = data2;
                        $scope.$apply();
                        return false;
                    }
                });
                return false;
            }
        });
        return false;
    };

    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllPackages",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
        success: function (data2) {
            $scope.Packages = data2;
            $scope.$apply();
            return false;
        }
    });




    $http.get("https://www.w3schools.com/angular/customers.php")
        .then(function (response) { /*$scope.Packages  = response.data.records;*/ });
});
routerApp.controller('ctrRoomsLists', function ($scope, $http) {
    $scope.RemoveRoomsPackage = function (id) {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/RemoveRoomsPackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { id: id },
            success: function (data2) {
                //alert('با موفقیت افزوده شد');
                $scope.$apply();
                $.ajax({
                    url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllRoomsPackages",
                    type: "GET",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    },
                    //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
                    success: function (data2) {
                        $scope.Packages = data2;
                        $scope.$apply();
                        return false;
                    }
                });
                return false;
            }
        });
        return false;
    }
    $scope.AddRoomsPackage = function () {
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/Package/AddRoomsPackage",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { Name: $scope.pkgName, Description: $scope.pkgDescription, Count: $scope.pkgCount },
            success: function (data2) {
                //alert('با موفقیت افزوده شد');
                $scope.$apply();
                $.ajax({
                    url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllRoomsPackages",
                    type: "GET",
                    headers: {
                        "ModuleId": ModuleId,
                        "TabId": TabId,
                        "RequestVerificationToken": rvtoken
                    },
                    //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
                    success: function (data2) {
                        $scope.Packages = data2;
                        $scope.$apply();
                        return false;
                    }
                });
                return false;
            }
        });
        return false;
    };

    $.ajax({
        url: "/DesktopModules/MVC/GDSModuleMVC/Package/GetAllRoomsPackages",
        type: "GET",
        headers: {
            "ModuleId": ModuleId,
            "TabId": TabId,
            "RequestVerificationToken": rvtoken
        },
        //data: { Name: $scope.pkgName, Description: $scope.pkgDescription },
        success: function (data2) {
            $scope.Packages = data2;
            $scope.$apply();
            return false;
        }
    });

});
routerApp.controller('moslem', function ($scope) {
    $scope.isValidForDiscount = false;
    $scope.usediscount = false;
    $scope.haspartner = false;
    $scope.AmountBeforeDiscount = $('#amount').val();
    $scope.calculateDiscount = function () {
        $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount;
        if ($scope.usediscount && $scope.haspartner && $scope.isValidForDiscount) {
            if ($scope.AmountBeforeDiscount > 6600000) {
                $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount - 6600000;
            }
            else
                $scope.AmountAfterDiscount = 0;
        }
        else if ($scope.usediscount && !($scope.haspartner) && $scope.isValidForDiscount) {
            if ($scope.AmountBeforeDiscount > 3300000) {
                $scope.AmountAfterDiscount = $scope.AmountBeforeDiscount - 3300000;
            }
            else
                $scope.AmountAfterDiscount = 0;
        }
        $('#AmountAfterDiscount').val($scope.AmountAfterDiscount);
        // alert($('#AmountAfterDiscount').val());
        //  alert($scope.AmountAfterDiscount);
    }
    $scope.$watch('AmountAfterDiscount', function (newValue, oldValue) {
        if (newValue === "")
            $('AmountAfterDiscount').val(newValue);
    });
    $scope.CheckText = "بررسی";
    $scope.switchBool = function (value) {
        $scope[value] = !$scope[value];
    };
    $scope.ApplyTakhfif = function () {
        if ($('#ContentPlaceHolder1_TKharidar_NationalID').val() == '') {
            show_error_panel('d_panel_kharidarinfo', 'ContentPlaceHolder1_TKharidar_NationalID', 'کد ملی را را وارد نمایید', 1000);
            return false;
        }
        if ($('#daftar').val() == '') {
            show_error_panel('d_panel_kharidarinfo', 'daftar', 'شماره دفتر را وارد نمایید', 1000);
            return false;
        }
        $scope.CheckText = "لطفا صبر کنید ...";
        $scope.isDisabled = true;
        $.ajax({
            url: "/DesktopModules/MVC/GDSModuleMVC/GDS/RetiredIsValid",
            type: "GET",
            headers: {
                "ModuleId": ModuleId,
                "TabId": TabId,
                "RequestVerificationToken": rvtoken
            },
            data: { NationalCode: $scope.NationalCode, DaftarNumber: $scope.daftarNumber },
            dataType: 'json',
            success: function (data) {
                $scope.calculateDiscount();
                $scope.CheckText = "بررسی";
                $scope.$apply();
                if (data == "102") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (Ip Is Invalid)";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "0") {
                    $scope.showSuccessAlert = true;
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "اطلاعات بازنشسته پیدا نشد";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "1") {

                    $.ajax({
                        url: "/DesktopModules/MVC/GDSModuleMVC/GDS/retiredhascredit",
                        type: "GET",
                        headers: {
                            "ModuleId": ModuleId,
                            "TabId": TabId,
                            "RequestVerificationToken": rvtoken
                        },
                        data: { NationalCode: $scope.NationalCode, DaftarNumber: $scope.daftarNumber },
                        success: function (data2) {
                            $scope.showSuccessAlert = false;
                            $scope.isDisabled = true;
                            $scope.switchBool('true');
                            $scope.CheckText = "شما مجاز به استفاده تور بازنشستگی هستید.";
                            $scope.isValidForDiscount = true;
                            $scope.calculateDiscount();
                            $('#ResultAfterCreditEnquery').html(data2);
                            $scope.$apply();
                        }
                    });
                }
                else if (data == "103") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = " در ارتباط با سیستم مرکزی مشکلی پیش آمده است. با پشتیبان فنی تماس بگیرید (User Name/Password Invalid) ";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                else if (data == "104") {
                    $scope.isDisabled = false;
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.successTextAlert = "کد ملی یا شماره دفتر کل نامعتبر است.";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
                return false;
            },
            statusCode: {
                500: function (data) {
                    $scope.isValidForDiscount = false;
                    $scope.calculateDiscount();
                    $scope.isDisabled = false;
                    $scope.calculateDiscount();
                    $scope.CheckText = "بررسی";
                    $scope.successTextAlert = "خطای نا مشخصی پیش آمده است.";
                    $scope.showSuccessAlert = true;
                    $scope.$apply();
                }
            }
        });
    };

});


