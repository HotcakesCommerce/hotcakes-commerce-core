<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="FileDownloads.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Catalog.FileDownloads" %>

<%@ Register Src="../Controls/ProductEditMenu.ascx" TagName="ProductEditMenu" TagPrefix="uc5" %>
<%@ Register Src="../Controls/ProductEditingDisplay.ascx" TagName="ProductEditing" TagPrefix="uc5" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<%@ Register Src="../Controls/FilePicker.ascx" TagName="FilePicker" TagPrefix="uc2" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<uc5:ProductEditMenu ID="ProductNavigator" runat="server" SelectedMenuItem="Files" />
	<uc5:ProductEditing ID="ProductEditing1" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
	<h1>File Downloads</h1>
	<uc1:MessageBox ID="MessageBox1" runat="server" />

	<uc2:FilePicker ID="FilePicker1" runat="server" />

	<ul class="hcActions">
		<li>
			<asp:ImageButton ID="AddFileButton" runat="server"
				 CssClass="hcPrimaryAction" AlternateText="Add File" OnClick="AddFileButton_Click" />
		</li>
	</ul>


	<div class="clearfix"></div>
<div class="hcColumnLeft" style="width: 50%">
	<div class="hcForm">
		<h2>Files Associated With this Product</h2>
		<div class="hcFormItem">
			<asp:GridView ID="FileGrid" runat="server" AutoGenerateColumns="False"
				ShowHeader="true" GridLines="none" BorderWidth="0px" DataKeyNames="Bvin"
				OnRowDeleting="FileGrid_RowDeleting"
				OnRowEditing="FileGrid_RowEditing" CssClass="hcGrid">
				<HeaderStyle CssClass="hcGridHeader" />
				<RowStyle CssClass="hcGridRow" />
				<Columns>
					<asp:BoundField DataField="ShortDescription"  HeaderText="Uploaded File"/>
					<asp:ButtonField ButtonType="Image" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Download.png"
						Text="Button" CommandName="Edit" />
					<asp:ButtonField ButtonType="Image" Text="Button" CommandName="Delete" ControlStyle-CssClass="hcIconDelete" />
				</Columns>
			</asp:GridView>
		</div>
	</div>
</div>
</asp:Content>
