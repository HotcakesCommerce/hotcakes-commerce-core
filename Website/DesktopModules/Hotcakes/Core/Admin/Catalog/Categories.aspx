<%@ Page Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Catalog.Categories" Title="Categories" CodeBehind="Categories.aspx.cs" %>
<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="NavContent" runat="server">
	<hcc:NavMenu runat="server" ID="NavMenu" />

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
				<label class="hcLabel"><%=Localization.GetString("AddCategory") %></label>
				<asp:DropDownList runat="server" ID="lstType">
					<Items>
						<asp:ListItem Value="0" Text="Category Page" />
						<asp:ListItem Value="2" Text="Custom Link" />
					</Items>
				</asp:DropDownList>
			</div>
		</div>
	</div>

	<div class="hcBlock hcBlockLight hcPaddingBottom">
		<div class="hcForm">
			<div class="hcFormItem">
                <asp:DropDownList runat="server" ID="lstParents"/>
			</div>
		</div>
	</div>

	<div class="hcBlock">
		<div class="hcForm">
			<div class="hcFormItem">
				<asp:LinkButton ID="btnNew" resourcekey="btnNew" runat="server" CssClass="hcTertiaryAction" EnableViewState="False" OnClick="btnNew_Click" />
			</div>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
	<h1><%=PageTitle %></h1>
    <asp:Literal ID="litMain" runat="server" EnableViewState="false"/>
    <script type="text/javascript">
        var confirmText = "<%=Localization.GetJsEncodedString("Confirm")%>";
    </script>
    <script src="Categories.js" type="text/javascript"></script>
</asp:Content>
