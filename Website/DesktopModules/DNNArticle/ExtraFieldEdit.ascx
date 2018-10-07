<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ExtraFieldEdit.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.ExtraFieldEdit" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div id="divBasicInfo" class="dnnForm">
<div class="dnnFormItem">
   <dnn:Label ID="plType" runat="server" Suffix=":" ControlName="cboCatalog"></dnn:Label>
   <asp:DropDownList ID="cboType" runat="server" Width="192px"  AutoPostBack="True"
        CausesValidation="false" OnSelectedIndexChanged="cboType_SelectedIndexChanged">
        <asp:ListItem Value="Integer">Integer</asp:ListItem>
        <asp:ListItem Value="Decimal">Decimal</asp:ListItem>
        <asp:ListItem Value="DropDownList">Selection List</asp:ListItem>
        <asp:ListItem Value="TextBox">TextBox</asp:ListItem>
        <asp:ListItem Value="HTML">HTML</asp:ListItem>
        <asp:ListItem Value="Date">Date</asp:ListItem>
        <asp:ListItem Value="HyperLink">HyperLink</asp:ListItem>
        <asp:ListItem Value="DNNArticle">DNNArticle</asp:ListItem>
    </asp:DropDownList>
</div>
<div class="dnnFormItem dnnClear">
    <asp:PlaceHolder ID="pl" runat="server"></asp:PlaceHolder>
</div>
<div class="dnnFormItem dnnClear">
    <p>
        <asp:Label ID="lbError" runat="server" ForeColor="Red"></asp:Label></p>
    <p>
        <asp:LinkButton ID="cmdUpdate" resourcekey="cmdUpdate" BorderStyle="None" runat="server"
            CssClass="UpdateLabel" OnClick="cmdUpdate_Click">Update</asp:LinkButton>&nbsp;<asp:LinkButton
                ID="cmdCancel" resourcekey="cmdCancel" BorderStyle="None" runat="server" CssClass="CancelLabel"
                CausesValidation="False" OnClick="cmdCancel_Click">Cancel</asp:LinkButton>&nbsp;<asp:LinkButton
                    ID="cmdDelete" resourcekey="cmdDelete" BorderStyle="None" runat="server" CssClass="DeleteLabel"
                    OnClick="cmdDelete_Click" CausesValidation="False" Visible="False">Delete</asp:LinkButton>
    </p>
</div>
</div>