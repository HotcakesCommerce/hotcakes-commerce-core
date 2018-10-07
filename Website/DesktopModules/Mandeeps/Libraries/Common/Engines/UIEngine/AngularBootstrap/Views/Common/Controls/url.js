app.controller('common_controls_url', function ($scope, $attrs, $compile, $element, $http, $location, CommonSvc, SweetAlert, FileUploader) {

    var common = CommonSvc.getData($scope);

    $scope.UploadFile = new FileUploader();
    $scope.Attachment = new FileUploader();

    $scope.onInit = function () {        
        setTimeout(function () {
            $scope.AttachmentClick_FileUpoad('browse');
            $('[identifier="common_controls_url"]').find('.ms-col-sm-12.esc').remove();
            $('[identifier="common_controls_url"]').find('.url-add .choosefile').parent().css({ 'position': 'relative', 'float': 'right', 'margin-bottom': '10px' });
            $('[identifier="common_controls_url"]').find('.url-add .choosefile').html('<span style="font-family:Arial, Helvetica, sans-serif;">[L:Upload]</span>');
            $('[identifier="common_controls_url"]').find('.url-add .choosefile').addClass('ms-glyphicon ms-glyphicon-plus');
        }, 10);
        if ($scope.ui.data.FilebrowserBrowseUrl.Value == "True" || $scope.ui.data.FilebrowserBrowseUrl.Value == "true")
            $scope.ui.data.FilebrowserBrowseUrl.Value = true;
        else
            $scope.ui.data.FilebrowserBrowseUrl.Value = false;
    };

    $scope.UploadFile.onBeforeUploadItem = function (item) {
        //$('[identifier="common_controls_url"]').find('[ng-show="UploadFile.queue.length"]').remove();
        item.formData[0].folder = $('[identifier="common_controls_url"]').find('.folders[style="font-weight: bold;"]').attr('id');
    };

    $scope.UploadFile.onCompleteAll = function () {
        if ($scope.UploadFile.progress == 100) {
            SweetAlert.swal('File uploaded successfully to ' + $('[identifier="common_controls_url"]').find('.folders[style="font-weight: bold;"]').text() + ' Folder');
            $scope.UploadFile.queue = [];
            $scope.Pipe_AttachmentPagging($scope.AttachmentTableState);
        }
    };

    $scope.UploadFile.onErrorItem = function (item, response, status, headers) {
        SweetAlert.swal(response.ExceptionMessage);
        $scope.UploadFile.progress = 0;
    };

    $scope.UpdateBrowser = function (sender) {
        if ($scope.Attachment.selectqueue != undefined && $scope.Attachment.selectqueue[0] != undefined && $scope.Attachment.selectqueue[0].fileurl != undefined && $scope.Attachment.selectqueue[0].fileurl != '') {
            var FuncNum = $scope.GetParameterByName('CKEditorFuncNum');
            var Opnr = window.top.opener;
            common.webApi.get('upload/getlink', 'fileurl=' + $scope.Attachment.selectqueue[0].fileurl + '&urltype=' + $scope.ui.data.Types.Value).success(function (data) {
                Opnr.CKEDITOR.tools.callFunction(FuncNum, data, '');
                self.close();
            });
        }
        else if ($scope.Attachment.queue != undefined && $scope.Attachment.queue[0] != undefined && $scope.Attachment.queue[0].formData[0] != undefined && $scope.Attachment.queue[0].formData[0].fileDirectory != undefined && $scope.Attachment.queue[0].file != undefined) {
            var FuncNum = $scope.GetParameterByName('CKEditorFuncNum');
            var Opnr = window.top.opener;
            common.webApi.get('upload/getlink', 'fileurl=' + $scope.Attachment.queue[0].formData[0].fileDirectory + $scope.Attachment.queue[0].file.name + '&urltype=' + $scope.ui.data.Types.Value).success(function (data) {
                Opnr.CKEDITOR.tools.callFunction(FuncNum, data, '');
                self.close();
            });
        }
        else
            window.close();
    };

    $scope.CancelBrowser = function () {
        window.close();
    };

    $scope.GetParameterByName = function (name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }
});