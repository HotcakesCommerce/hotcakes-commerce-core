<%@ Control Language="C#" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Controls.FilePicker" Codebehind="FilePicker.ascx.cs" %>
<%@ Register Src="TimespanPicker.ascx" TagName="TimespanPicker" TagPrefix="hcc" %>
	<div class="hcForm">
		<div class="hcFormItem">
			<label class="hcLabel"><%=Localization.GetString("lblUploadFile") %></label>
			<asp:FileUpload ID="NewFileUpload" runat="server" Width="316px" />
		</div>
		<div class="hcFormItem">
			<label class="hcLabel"><%=Localization.GetString("lblUseExistingFile") %></label>
			 <asp:DropDownList ID="FilesDropDownList" runat="server" AppendDataBoundItems="True" Width="304px">
                <asp:ListItem Text="File Below" Value="" />
            </asp:DropDownList>
            <asp:TextBox ID="FileSelectedTextBox" runat="server" Visible="False"/>
            <asp:CustomValidator ID="cvFileHasBeenSelected" resourcekey="cvFileHasBeenSelected" runat="server"
                                 CssClass="hcFormError" onservervalidate="FileHasBeenSelectedCustomValidator_ServerValidate" Display="Dynamic"/>
            <asp:CustomValidator ID="cvFileIsUniqueToProduct" resourcekey="cvFileIsUniqueToProduct" runat="server" 
                                 CssClass="hcFormError" onservervalidate="FileIsUniqueToProductCustomValidator_ServerValidate" Display="Dynamic"/>
            <a id="browseButton" runat="server" href="javascript:popUpWindow('?returnScript=SetSmallImage&WebMode=1');">
            <asp:Image runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Browse.png" ID="imgSelect1" /></a>
            <asp:HiddenField ID="FileIdHiddenField" runat="server" />
		</div>
		<div class="hcFormItem" id="ShortDescriptionRow" runat="server">
			<label class="hcLabel">Short Description:</label>
            <asp:TextBox ID="ShortDescriptionTextBox" runat="server" Width="304px"/>
            <asp:CustomValidator ID="cvDescriptionIsUniqueToProduct" resourcekey="cvDescriptionIsUniqueToProduct" runat="server"
                                 ControlToValidate="ShortDescriptionTextBox" CssClass="hcFormError" 
                                 onservervalidate="DescriptionIsUniqueToProductCustomValidator_ServerValidate" Display="Dynamic"/>
		</div>
		<div class="hcFormItem" id="AvailableMinutesRow" runat="server">
			<label class="hcLabel"><%=Localization.GetString("lblAvailableMinutes") %></label>
			<hcc:TimespanPicker ID="AvailableForTimespanPicker" runat="server" />
		</div>
		<div class="hcFormItem" id="NumberDownloadsRow" runat="server">
			<label class="hcLabel"><%=Localization.GetString("lblDownloadNumber") %></label>
            <asp:TextBox ID="NumberOfDownloadsTextBox" runat="server" Width="32px"/>
            <asp:RegularExpressionValidator ID="revDownloadNumber" resourcekey="revDownloadNumber" runat="server" 
                                            ControlToValidate="NumberOfDownloadsTextBox" ValidationExpression="\d{1,6}" CssClass="hcFormError" Display="Dynamic"/>
		</div>
	</div>