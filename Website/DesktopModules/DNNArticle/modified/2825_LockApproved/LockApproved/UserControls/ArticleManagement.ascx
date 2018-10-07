<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ArticleManagement.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.ArticleManagement" %>
<%@ Register TagPrefix="dnnsc" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<asp:Label ID="lbNoContent" runat="server" CssClass="NormalRed" resourcekey="NoItems"></asp:Label>
<asp:DataGrid ID="dgArticleList" runat="server" AutoGenerateColumns="False" CssClass="dnnGrid" width="100%" CellPadding="2" GridLines="None" 
			  OnItemCommand="dgArticleList_ItemCommand" OnItemDataBound="dgArticleList_ItemDataBound"
              DataKeyField="ItemId">
    <ItemStyle CssClass="dnnGridItem"></ItemStyle>
	<AlternatingItemStyle CssClass="dnnGridAltItem"></AlternatingItemStyle>
    <HeaderStyle CssClass="dnnGridHeader" ></HeaderStyle>
               
    <Columns>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:CheckBox ID="ckbSelected" runat="server" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="Id">
            <ItemTemplate>
                <%#DataBinder.Eval(Container.DataItem, "ItemId")%>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText='Title'>
            <ItemTemplate>
                <asp:Label ID="ltName" CssClass="Normal" runat="server"></asp:Label>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText='Owner'>
            <ItemTemplate>
                <asp:Label ID="ltOwner" CssClass="Normal" runat="server"></asp:Label>
            </ItemTemplate>
                        
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText='PublishDate'>
            <ItemTemplate>
                <asp:Label ID="ltPublishDate" CssClass="Normal" runat="server"></asp:Label>
            </ItemTemplate>
                       
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText='ExpiredDate'>
            <ItemTemplate>
                <asp:Label ID="ltExpiredDate" CssClass="Normal" runat="server"></asp:Label>
            </ItemTemplate>
                       
        </asp:TemplateColumn>

        <asp:TemplateColumn HeaderText='Featured'>
            <ItemTemplate>
                <asp:ImageButton ID="ibtnFeatured" runat="server" CommandName="Featured" ImageUrl="~/desktopmodules/dnnarticle/images/Featured.gif"/>
                <asp:Image runat="server" ID="imgFeatured" ImageUrl="~/desktopmodules/dnnarticle/images/Featured.gif" AlternateText="featured" BorderWidth="0"/>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText='Approved'>
            <ItemTemplate>
                <asp:ImageButton ID="ibtnApprove" runat="server" CommandName="Approve" ImageUrl="~/desktopmodules/dnnarticle/images/Publish.png"/>
                <asp:Image runat="server" ID="imgApprove" ImageUrl="~/desktopmodules/dnnarticle/images/Publish.gif" AlternateText="Publish" BorderWidth="0"/>
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                <asp:HyperLink ID="lnkView" runat="server">
            
            <asp:Image runat="server" ID="imgView" ImageUrl="~/images/view.gif" AlternateText="view" BorderWidth="0"/></asp:HyperLink>
                          </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn>
            <ItemTemplate>
                            
                <asp:ImageButton ID="ibtnEdit" runat="server" CommandName="Edit" ImageUrl="~/images/edit.gif"
                                 AlternateText="Edit" />
            </ItemTemplate>
        </asp:TemplateColumn>
        <asp:TemplateColumn >
            <ItemTemplate>
                <asp:ImageButton ID="ibtnDelete" runat="server" CommandName="Delete" ImageUrl="~/images/delete.gif"
                                 AlternateText="Edit Content"/>
            </ItemTemplate>
        </asp:TemplateColumn>
    </Columns>
</asp:DataGrid>
<dnnsc:PagingControl ID="ctlDNNPagingControl" runat="server"></dnnsc:PagingControl>
<asp:LinkButton ID="cmdFeatured" runat="server" resourcekey="cmdFeatured" onclick="cmdFeatured_Click" CssClass="FeaturedLabel"></asp:LinkButton>
<asp:LinkButton ID="cmdApprove" runat="server" resourcekey="cmdApprove" onclick="cmdApprove_Click" CssClass="ApproveLabel"></asp:LinkButton>
<asp:LinkButton ID="cmdDelete" runat="server" resourcekey="cmdDelete" onclick="cmdDelete_Click" CssClass="DeleteLabel"></asp:LinkButton>
<asp:LinkButton ID="cmdCancel" runat="server" resourcekey="cmdCancel" onclick="cmdCancel_Click" CssClass="CancelLabel"></asp:LinkButton>