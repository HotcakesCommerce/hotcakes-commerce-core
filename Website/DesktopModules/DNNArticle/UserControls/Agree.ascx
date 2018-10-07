<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Agree.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.Agree" %>

<%--<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>


<telerik:RadAjaxManagerProxy ID="AjaxManagerProxy1" runat="server" >
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="cmdAgree" >
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="panelAgree" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="cmdDisAgree" >
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="panelAgree" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManagerProxy>--%>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true" UpdateMode="Always" RenderMode="Block"> 
    <ContentTemplate>
        <asp:Panel ID="panelAgree" runat="server" CssClass="panelAgree">
            <asp:LinkButton ID="cmdAgree" runat="server" CssClass="agreelink" CommandName="1" ClientIDMode="AutoID" 
                            onclick="cmdAgree_Click"></asp:LinkButton><asp:Label ID="lbTotalAgree" runat="server" CssClass="lbTotalAgree"></asp:Label>
            <asp:LinkButton ID="cmdDisAgree" runat="server" CssClass="disagreelink" CommandName="0" ClientIDMode="AutoID" 
                            onclick="cmdAgree_Click"></asp:LinkButton>
    
            <asp:Label ID="lbAgree" runat="server" CssClass="lbAgree"></asp:Label>
            <asp:Label ID="lbDisAgree" runat="server" CssClass="lbDisAgree"></asp:Label>
    
        </asp:Panel> 
    </ContentTemplate> 
    
</asp:UpdatePanel>


