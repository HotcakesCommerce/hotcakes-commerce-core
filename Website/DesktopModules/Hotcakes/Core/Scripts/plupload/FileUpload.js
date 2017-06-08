jQuery(function ($) {
	function InitilizeUploader(){
		var allowedExtensions = $('input[name=uploadfiletypes]').val();
		var productBvin = jQuery('#productbvin').attr('value');
		
		$('[id^="container"]').each(function() {
			var el = $(this);
			var optUniqueId = el.attr("id").split('_')[1];
			var browse_button = el.find('a.browseFiles');
			var uploadfiles_button = el.find('a.uploadFiles');
			var allowMultiple_input = el.find('input.allowmultiplefiles');

			var uploader = new plupload.Uploader({
				runtimes : 'html5,flash,silverlight',
				browse_button: browse_button.attr("id"),
				container: el.attr("id"),
				max_file_size : '2gb',
				chunk_size: '1mb',
				multi_selection: false,
				unique_names : true,
				url : hcc.getResourceUrl("FileUpload.ashx"),
				silverlight_xap_url : hcc.getResourceUrl("Scripts/plupload/plupload.silverlight.xap"),
				flash_swf_url : hcc.getResourceUrl("Scripts/plupload/plupload.flash.swf"),
				filters : [
					{title : "Custom files", extensions : "*.*"}
				]
			});
				
			var allowMultipleFile = allowMultiple_input.val();
			var allowMutltipleUpload = false;
			if(allowMultipleFile === 'false' || allowMultipleFile === 'False') {
				allowMutltipleUpload = false;
			} else {
				allowMutltipleUpload = true;
			}
		
			uploader.settings.filters[0].extensions = allowedExtensions;
			uploader.optUniqueId = optUniqueId;
			uploader.productBvin = productBvin;
			uploader.allowMutltipleUpload = allowMutltipleUpload;
				
			uploader.bind('Init', initUploader);
			uploader.bind('BeforeUpload', beforeUpload);
			uploader.bind('FilesAdded', fileAdded);
			uploader.bind('FilesRemoved', filesRemoved);
			uploader.bind('UploadProgress', uploadProgress);
			uploader.bind('Error', uploadError);
			uploader.bind('ChunkUploaded', chunkUploaded);
			uploader.bind('FileUploaded', filesUploaded);
				
			uploadfiles_button.click(function (e) {
				uploader.start();
				e.preventDefault();
			});
				
			uploader.init();
			//console.log(uploader);
		});
	}

	var initUploader = function(up, params) {
		//console.log('plUpload Init');
		//console.log('plUpload - current runtime: ' + params.runtime);
		$('#addtocartbutton').attr('disabled','disabled');
		$('#savelaterbutton').attr('disabled','disabled');
	};

	var beforeUpload = function(up, file) {
		//console.log('plUpload BeforeUpload');
		up.settings.multipart_params = {
			filename: file.name,
			bvin: up.productBvin,
			optUniqueId: up.optUniqueId
		};
	};

	var fileAdded = function(up, files) {
		//console.log('plUpload FilesAdded');
		var deleteHandle = function(uploaderObject, fileObject) {
			return function(event) {
				event.preventDefault();
				if(fileObject.status ==  plupload.UPLOADING){
					uploaderObject.stop();
				}
				uploaderObject.removeFile(fileObject);
				$(this).closest("li#" + 'li_' + fileObject.id).remove();
				
			};
		};

		var maxCountError = false;
		var filelist = $('#filelist_' + up.optUniqueId);
		
		$.each(files, function(i, file) {
			//console.log(up.allowMutltipleUpload);
			if(up.allowMutltipleUpload == false && up.files.length >= 1){
				maxCountError = true;
				setTimeout(function() { up.removeFile(file); } ,50);
			}
			else {
			    filelist.find('ul').append(
					'<li class="upload_file_item" id="li_' + file.id + '" >' +
						'<div id="' + file.id + '">' +
							'<div class="upload_file_Name left" >' + file.name + '</div>' +
							'<div class="upload_file_size right" >' + ' (' + bytesToSize(file.size) + ') ' + '</div>' +
							'<div class="upload_file_status right" >' + '<b> </b>' + '</div>' +
							'<div class="upload_file_delete right" >' + '<a href="#" id="deleteFile' + file.id + '">[x]</a>' + '</div>' +
							'<div style="clear:both" >' +
						'</div>' +
					'</li>'
					);
				 $('#deleteFile' + file.id).click(deleteHandle(up, files[i]));
			}
		});
		up.refresh(); // Reposition Flash/Silverlight
	};

	var filesRemoved = function(up, files) {
		//console.log('plUpload FilesRemoved');
		if(files.length === 0) {
			$('#addtocartbutton').attr('disabled','disabled');
			$('#savelaterbutton').attr('disabled','disabled');
		}
	};
	
	var uploadProgress = function(up, file) {
		//console.log('plUpload UploadProgress');
		$('#li_' + file.id + ' .upload_file_status').html(file.percent + '%');
	};

	var uploadError = function(up, err) {
		//console.log('plUpload Error');
	    var filelist = $('#filelist_' + up.optUniqueId);
		//TODO: Add file id and base on that remove error element when file is removed from queue
	    filelist.find('#' + err.file.id + ' .upload_file_status').append("<div id='" + err.file.id + "'> Error: " + err.code +
			", Message: " + err.message +
			(err.file ? ", File: " + err.file.name : "") +
			"</div>"
		);
		up.refresh(); // Reposition Flash/Silverlight
	};

	var chunkUploaded = function(up, file, response) {
		//console.log('plUpload ChunkUploaded');
		//console.log(response.response);
		var data = $.parseJSON(response.response);
		//console.log(data);
		//TODO: When chunk is uploaded
		// if(data["StatusCode"] == 500 ){
		// $('#' + file.id + " b").html("Unable to write file");
		// up.trigger('Error',file);
		// }
	};

	var filesUploaded = function(up, file, response) {
		//console.log('plUpload FileUploaded');
		var data = $.parseJSON(response.response);
		//console.log(data);
		var downloadUrl = data['Message'].replace(/["]/g, "");
		//console.log(downloadUrl);
		var downloadLink = '<a id="downloadFile' + file.id + '" href="' + downloadUrl + '" target="_blank">' + file.name + '</a>' ;
		$('#li_' + file.id + ' .upload_file_Name').html(downloadLink);
		$('#li_' + file.id + ' .upload_file_status').html('<b>100%</b>');
		$('#deleteFile' + file.id).html('');

		//Add file to list (used to store back on server as html)
		var uploadedFileStack = $('input[name="opt' + up.optUniqueId + '"]').val();
		$('input[name="opt' + up.optUniqueId + '"]').val(uploadedFileStack + '|' + downloadUrl + '<' + bytesToSize(file.size));
		
		var isAddToCartDisabled = $('#addtocartbutton').attr('disabled');
		if(typeof isAddToCartDisabled !== 'undefined' && isAddToCartDisabled !== false){
			$('#addtocartbutton').removeAttr('disabled');
			$('#savelaterbutton').removeAttr('disabled');
		}
	};

	function bytesToSize(bytes) {
		//console.log(bytes);
		var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
		if (bytes == 0) return '0 Bytes';

		var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
		//var size= Math.round(bytes / Math.pow(1024, i), 3) + ' ' + sizes[i];
		var size = bytes / Math.pow(1024, i);
		if( size.toString().indexOf('.') === -1) {
			return size.toString() + ' ' + sizes[i];
		} else {
			var pre = size.toString().indexOf('.');
			var rounded = size.toString().substring(0,pre) + '.' + size.toString().substring(pre+1,pre+3);
			return rounded.toString() + ' ' + sizes[i];
		}
	};

	InitilizeUploader();
});
