<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategorySelector.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.CategorySelector" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee"
    BorderStyle="Solid" BorderWidth="1px" Transparency="50" Width="256px" Height="64px"
    RestoreOriginalRenderDelegate="false">
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle; height: 64px">
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                    runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>

<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
    <AjaxSettings>
    
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>

<asp:Panel runat="server" ID="panel" Style="text-align: left; display:inline-block;margin-bottom:10px">
    <asp:Label runat="server" CssClass="Normal" ID="lbNoCategory" ></asp:Label>
    <asp:Panel Style="overflow: auto;border:1px solid #ccc;" ID="divCategory" runat="server" Height="150px" Width="425px">
        <asp:TreeView ID="tvCategory" runat="server" HoverNodeStyle-CssClass="ZLDNN_TreeNodeSelected"
            NodeStyle-CssClass="ZLDNN_TreeNode" SelectedNodeStyle-CssClass="ZLDNN_TreeNodeSelected" 
            OnTreeNodePopulate="tvCategory_TreeNodePopulate">
        </asp:TreeView>
    </asp:Panel>
    <div runat="server" id="divAddCategory">
        <asp:Label runat="server" CssClass="dnnSecondaryAction" ID="lbAddNewCategory" onclick="$('#divAddC').show()" style="cursor:pointer"></asp:Label>
        <div id="divAddC" style="display: none">
            <div class="dnnFormItem">
                <dnn:Label ID="lblNewCategoryTitle" runat="server" ControlName="txtTitle" Suffix=":" />
                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtTitle"
                    ValidationGroup="CategoryEditor" EnableClientScript="true" CssClass="NormalRed"
                    Display="Dynamic" ErrorMessage="*" resourcekey="Title.ErrorMessage" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:Label ID="lblParentCategory" runat="server" ControlName="cboCategory" Suffix=":" />
                <asp:DropDownList ID="cboCategory" runat="server" CssClass="Normal CategoryDropdown">
                </asp:DropDownList>
            </div>
            <div style="text-align: center;">
                <asp:LinkButton CssClass="UpdateLabel" ID="cmdUpdate"  runat="server"
                    CausesValidation="true" ValidationGroup="CategoryEditor" BorderStyle="none" OnClick="cmdUpdate_Click"></asp:LinkButton>
            </div>
        </div>
    </div>
</asp:Panel>
