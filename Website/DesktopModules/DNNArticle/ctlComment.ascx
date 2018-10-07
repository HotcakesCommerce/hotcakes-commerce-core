<%@ Control Language="C#" AutoEventWireup="true" Codebehind="ctlComment.ascx.cs"
            Inherits=" ZLDNN.Modules.DNNArticle.ctlComment" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" TagPrefix="cc1" %>

<%--<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server" BackColor="#eeeeee" BorderStyle="Solid" BorderWidth ="1px" Transparency="50" Width="256px" Height="64px" >
    <table width="100%" height="100%">
        <tr>
            <td style="vertical-align: middle; height: 64px">
                <asp:Image ID="Image1" ImageUrl="~/desktopmodules/dnnarticle/images/loading.gif"
                           runat="server" AlternateText="loading" />
            </td>
        </tr>
    </table>
</telerik:RadAjaxLoadingPanel>

<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="LoadingPanel1" 
 EnableAJAX="False"   >--%>
 


<table id="Table1" style="width: 100%" runat="server">
    <tr>
        <td>
            <dnn:SectionHead ID="dshComment" runat="server" IncludeRule="True" Section="tblComment"
                             CssClass="CommentsLabel" ResourceKey="dshComment"></dnn:SectionHead>
            <table id="tblComment" style="width: 100%" runat="server">
                <tr>
                    <td class="SubHead" >
                        <asp:Label ID="lbNoComment" runat="server" resourcekey="NoComments" CssClass="CommentsLabel"></asp:Label>
                        <asp:DataList ID="lstComments" runat="server" Width="100%" CellPadding="5" OnItemCommand="lstComments_ItemCommand">
                            <ItemTemplate>
                                <asp:Literal ID="lbComment" runat="server" Text='<%#GetCommentContent(Container.DataItem, "")%>'>		</asp:Literal>
                                <asp:LinkButton ID="Linkbutton1" resourcekey="cmdDelete" CausesValidation="false"
                                                runat="server" CssClass="CommandButton" Visible='<%#CanEdit%>' CommandName="Delete"
                                                CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CommentID")%>'>Delete</asp:LinkButton>
                                <asp:LinkButton ID="cmdApprove" CausesValidation="false" runat="server" Text='<%#GetApproveText(Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Hidden")))%>'
                                                CssClass='<%#GetApproveCSS(Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "Hidden")))%>'
                                                Visible='<%#CanEdit%>' CommandName="Approve" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CommentID").ToString()%>'> </asp:LinkButton>
                            </ItemTemplate>
                        </asp:DataList>
                        <cc1:PagingControl ID="ctlDNNPagingControl" Mode="PostBack" runat="server" OnPageChanged="ctlPagingControl_PageChanged"></cc1:PagingControl>
                        </td>
                </tr>
                <tr>
                    <td  style="width: 100%"><hr /></td>
                </tr>
                <tr runat="server" id="trComment">
                    
                    <td class="SubHead" >
                        <table id="tbComment" cellspacing="0" cellpadding="0" style="width: 100%" border="0" runat="server">
                            <tr id="trName" runat="server">
                                <td class="CommentLeftTD" valign="top" width="1%" >
                                    <asp:Label ID="plYourName" runat="server" CssClass="NormalBold"></asp:Label>
                                </td>
                                <td  width="99%" >
                                    <asp:TextBox ID="txtYourName" CssClass="NormalTextBox" runat="server" style="width: 100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtYourName"
                                                                ValidationGroup="ArticleComment" EnableClientScript="true" CssClass="NormalRed"
                                                                Display="Dynamic" ErrorMessage="*" runat="server" />
                                </td>
                            </tr>
                            <tr id="trEmail" runat="server">
                                <td class="CommentLeftTD" valign="top"  width="1%">
                                    <asp:Label ID="lbEmail" runat="server" CssClass="NormalBold"></asp:Label>
                                </td>
                                <td   width="99%"  >
                                    <asp:TextBox ID="txtEmail" CssClass="NormalTextBox" runat="server" style="width: 100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trWebsite" runat="server">
                                <td class="CommentLeftTD" valign="top"  width="1%">
                                    <asp:Label ID="lbWebSite" runat="server" CssClass="NormalBold"></asp:Label>
                                </td>
                                <td   width="99%"  >
                                    <asp:TextBox ID="txtWebSite" CssClass="NormalTextBox" runat="server" style="width: 100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="CommentLeftTD"  width="1%">
                                    <asp:Label ID="plCommentTitle" runat="server" CssClass="NormalBold"></asp:Label>
                                </td>
                                <td  width="99%" >
                                    <asp:TextBox ID="txtTitle" CssClass="NormalTextBox" runat="server" style="width: 100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtTitle"
                                                                ValidationGroup="ArticleComment" EnableClientScript="true" CssClass="NormalRed"
                                                                Display="Dynamic" ErrorMessage="*" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="CommentLeftTD" style="vertical-align: top"  width="1%">
                                    <asp:Label ID="plComment" runat="server" CssClass="NormalBold"></asp:Label>
                                </td>
                                <td  width="99%" >
                                    <asp:TextBox ID="txtComment" CssClass="NormalTextBox" runat="server" style="width: 100%" TextMode="MultiLine" Height="64px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtComment"
                                                                ValidationGroup="ArticleComment" EnableClientScript="true" CssClass="NormalRed"
                                                                Display="Dynamic" ErrorMessage="*" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead"  style="vertical-align: top; height: 20px"  width="1%">
                                </td>
                                <td class="SubHead"  style="vertical-align: top; height: 20px"  width="99%" >
                                    <cc1:CaptchaControl ID="CaptchaControl1" runat="server" CaptchaHeight="50px" CaptchaWidth="150px" />
                                    
                                </td>
                            </tr>
                            <tr>
                                <td class="SubHead"  style="vertical-align: top; height: 20px"  width="1%">
                                </td>
                                <td class="SubHead"  style="vertical-align: top; height: 20px"  width="99%" >
                                    <asp:Button ID="btnComment" runat="server" CssClass="StandardButton" resourcekey="btnComment"
                                                ValidationGroup="ArticleComment" Text="Submit Comment" OnClick="btnComment_Click"></asp:Button>
                                </td>
                            </tr>
                        </table>
                        
                        <asp:Label ID="lbMessage" runat="server" CssClass="Normal"></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
</table>

     

 </telerik:RadAjaxPanel>