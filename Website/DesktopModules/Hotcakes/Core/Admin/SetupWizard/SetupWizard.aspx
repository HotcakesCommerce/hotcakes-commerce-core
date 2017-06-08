<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="../AdminWizard.master" CodeBehind="SetupWizard.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.SetupWizard.SetupWizard" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<asp:Panel ID="pnlNavigation" runat="server">
		<ul class="hcWizardProgressNav hcBranding">
			<li class="<%=GetProgressItemClass(1) %>">
				<asp:HyperLink runat="server" ID="hlStep1"><i>1</i> <%=Localization.GetString("General") %></asp:HyperLink>
			</li>
			<li class="<%=GetProgressItemClass(2) %>">
				<asp:HyperLink runat="server" ID="hlStep2"><i>2</i> <%=Localization.GetString("Payment") %></asp:HyperLink>
			</li>
			<li class="<%=GetProgressItemClass(3) %>">
				<asp:HyperLink runat="server" ID="hlStep3"><i>3</i> <%=Localization.GetString("Shipping") %></asp:HyperLink>
			</li>
			<li class="<%=GetProgressItemClass(4) %> hcProgressFinish">
				<asp:HyperLink runat="server" ID="hlStep4"><i>4</i> <%=Localization.GetString("Taxes") %></asp:HyperLink>
			</li>
		</ul>
	</asp:Panel>
	<asp:MultiView ID="MultiView" ActiveViewIndex="0" runat="server" OnActiveViewChanged="MultiView_ActiveViewChanged">
		<asp:View ID="Step0" runat="server">
			<asp:PlaceHolder ID="phStep0" runat="server" />
		</asp:View>
		<asp:View ID="Step1" runat="server">
			<asp:PlaceHolder ID="phStep1" runat="server" />
		</asp:View>
		<asp:View ID="Step2" runat="server">
			<asp:PlaceHolder ID="phStep2" runat="server" />
		</asp:View>
		<asp:View ID="Step3" runat="server">
			<asp:PlaceHolder ID="phStep3" runat="server" />
		</asp:View>
		<asp:View ID="Step4" runat="server">
			<asp:PlaceHolder ID="phStep4" runat="server" />
		</asp:View>
	</asp:MultiView>

</asp:Content>
