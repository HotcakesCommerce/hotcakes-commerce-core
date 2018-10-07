<%@ Control Language="C#" Inherits="ZLDNN.Modules.DNNArticleTagCloud.DNNArticleTagCloud" AutoEventWireup="true" Explicit="True" Codebehind="DNNArticleTagCloud.ascx.cs" %>


<div id="divTagList<%=ModuleId.ToString() %>">
<asp:Repeater ID="lstContent" runat="server" OnItemDataBound="lstContent_ItemDataBound">
    <itemtemplate>
        <asp:Literal ID="lblContent" runat="server" />
    </itemtemplate>
</asp:Repeater>
</div>