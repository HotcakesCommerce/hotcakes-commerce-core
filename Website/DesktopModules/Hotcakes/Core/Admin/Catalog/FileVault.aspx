<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.FileVault" Title="File Vault" CodeBehind="FileVault.aspx.cs" %>


<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc2" %>
<%@ Register Src="../Controls/FilePicker.ascx" TagName="FilePicker" TagPrefix="uc1" %>
<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu ID="ucNavMenu" runat="server" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
	<h1>File Vault</h1>
	<uc2:MessageBox ID="MessageBox1" runat="server" />
	<uc1:FilePicker ID="FilePicker" runat="server" />
	<ul class="hcActions">
		<li>
			<%--<asp:ImageButton ID="AddFileButton" runat="server"
				CssClass="hcPrimaryAction" AlternateText="Add File" OnClick="AddFileButton_Click" />--%>
			<asp:LinkButton ID="AddFileButton" Text="Add File" CssClass="hcPrimaryAction" runat="server" OnClick="AddFileButton_Click" />
		</li>
	</ul>

	<div class="clearfix"></div>

		<div class="hcForm">
			<div class="hcFormItem right hcFileVaultHelp" >
				<asp:LinkButton ID="ImportLinkButton" runat="server" CausesValidation="False"
					OnClick="ImportLinkButton_Click" class="">
					Import Files Already In "Files" Folder
				</asp:LinkButton>
				&nbsp;
				<i class="hcIconInfo">
					<span class="hcFormInfo" style="display: none; margin-top: 0px; left: 254px;">If you created a folder in the root of your website directory, called "Files," those files will be imported to this file vault upon clicking this link.</span>
				</i>
			</div>
			<div class="hcFormItem">
				<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always">
					<ContentTemplate>
						<table width="100%" class="hcGrid">
							<thead>
								<tr class="hcGridHeader">
									<td>File Name</td>
									<td>Description</td>
									<td>Product Count</td>
									<td style="width:5%;">&nbsp;</td>
									<td style="width:5%;">&nbsp;</td>
								</tr>
							</thead>
							<asp:Literal ID="litFiles" runat="server" EnableViewState="false"></asp:Literal>
						</table>
					</ContentTemplate>
				</asp:UpdatePanel>
			</div>
		</div>
</asp:Content>
