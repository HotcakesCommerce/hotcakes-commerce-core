<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.FilePicker" Codebehind="FilePicker.ascx.cs" %>
<%@ Register Src="TimespanPicker.ascx" TagName="TimespanPicker" TagPrefix="uc1" %>
	<div class="hcForm">
		<div class="hcFormItem">
			<label class="hcLabel">Upload a new file to the site:</label>
			<asp:FileUpload ID="NewFileUpload" runat="server" Width="316px" />
		</div>
		<div class="hcFormItem">
			<label class="hcLabel">Use a File That Has Already Been Uploaded:</label>
			 <asp:DropDownList ID="FilesDropDownList" runat="server" AppendDataBoundItems="True" Width="304px">
                <asp:ListItem Text="File Below" Value="" />
            </asp:DropDownList>
			 <asp:TextBox ID="FileSelectedTextBox" runat="server" Visible="False"></asp:TextBox>
            <asp:CustomValidator ID="FileHasBeenSelectedCustomValidator" runat="server" 
                ErrorMessage="You must upload a file or select a file that has already been uploaded." 
                CssClass="errormessage" ForeColor=" " 
                onservervalidate="FileHasBeenSelectedCustomValidator_ServerValidate">*</asp:CustomValidator>
            <asp:CustomValidator ID="FileIsUniqueToProductCustomValidator" 
                runat="server" ErrorMessage="Physical file must be unique to product." 
                CssClass="errormessage" ForeColor=" " 
                onservervalidate="FileIsUniqueToProductCustomValidator_ServerValidate">*</asp:CustomValidator>
            <a id="browseButton" runat="server" href="javascript:popUpWindow('?returnScript=SetSmallImage&WebMode=1');">
            <asp:Image runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Browse.png" ID="imgSelect1" /></a><asp:HiddenField
                ID="FileIdHiddenField" runat="server" />
		</div>
		<div class="hcFormItem" id="ShortDescriptionRow" runat="server">
			<label class="hcLabel">Short Description:</label>
			<asp:TextBox ID="ShortDescriptionTextBox" runat="server" Width="304px"></asp:TextBox>
            <asp:CustomValidator ID="DescriptionIsUniqueToProductCustomValidator" 
                runat="server" 
                ErrorMessage="Short description must be unique for files with the same name." 
                ControlToValidate="ShortDescriptionTextBox" CssClass="errormessage" 
                ForeColor=" " 
                onservervalidate="DescriptionIsUniqueToProductCustomValidator_ServerValidate">*</asp:CustomValidator></td>
		</div>
		<div class="hcFormItem" id="AvailableMinutesRow" runat="server">
			<label class="hcLabel">Available for (leave blank for unlimited):</label>
			<uc1:TimespanPicker ID="AvailableForTimespanPicker" runat="server" />
		</div>
		<div class="hcFormItem" id="NumberDownloadsRow" runat="server">
			<label class="hcLabel">Number of times file can be downloaded:</label>
			<asp:TextBox ID="NumberOfDownloadsTextBox" runat="server" Width="32px"></asp:TextBox>
            (leave blank for unlimited)<asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                runat="server" ControlToValidate="NumberOfDownloadsTextBox" ErrorMessage="Number of times file can be downloaded must be numeric"
                ValidationExpression="\d{1,6}" CssClass="errormessage" ForeColor=" ">*</asp:RegularExpressionValidator>
		</div>
	</div>