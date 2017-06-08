<%@ Page Language="C#" MasterPageFile="~/HCC/Admin/Admin_old.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Reports.Shopping_Carts.View" title="Reports - Shopping Carts" Codebehind="View.aspx.cs" %>
<%@ Register Src="../../../Admin/Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<h1>
        Current Shopping Carts</h1>
    <uc1:MessageBox ID="msg" runat="server" />    
    <div style="padding-right: 10px; padding-left: 10px; background: #ffffff; padding-bottom: 10px;
                    width: 700px; padding-top: 10px">
                    <asp:Label ID="lblResponse" Text="" runat="server" /><br />
                    &nbsp;<br />
                    <asp:DataGrid DataKeyField="bvin" CellPadding="3" BorderWidth="0px" CellSpacing="1"
                        ItemStyle-CssClass="row" ID="dgList" runat="server" AutoGenerateColumns="False"
                        Width="680px" ShowFooter="True" GridLines="none" 
                        oneditcommand="dgList_Edit" onitemdatabound="dgList_ItemDataBound">
                        <HeaderStyle CssClass="rowheader" />
                        <AlternatingItemStyle CssClass="alternaterow"></AlternatingItemStyle>
                        <ItemStyle CssClass="ItemStyle2"></ItemStyle>
                        <FooterStyle CssClass="HeaderStyle2"></FooterStyle>
                        <FooterStyle CssClass="HeaderStyle2"></FooterStyle>
                        <Columns>
                            <asp:BoundColumn DataField="timeoforderutc" HeaderText="Date" DataFormatString="{0:d}">
                            </asp:BoundColumn>
                            <asp:TemplateColumn HeaderText="ID #">                                
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.bvin") %>'></asp:Label><br />
                                    <asp:Label ID="Label2" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.BillingAddress.LastName") %>'></asp:Label>, <asp:Label runat="server" ID="lblFirstName" Text='<%# DataBinder.Eval(Container, "DataItem.BillingAddress.FirstName") %>'>M</asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="SubTotal">                                
                                <ItemTemplate>                                    
                                    <asp:Label ID="Label1" runat="server" Text='<%# String.Format("{0:c}",DataBinder.Eval(Container, "DataItem.TotalGrand")) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Top"></ItemStyle>
                                <ItemTemplate>
                                    <asp:ImageButton ID="EditButton" runat="server" CommandName="Edit" ImageUrl="~/HCC/Admin/Images/Buttons/View.png">
                                    </asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle CssClass="FormLabel"></PagerStyle>
                    </asp:DataGrid>
                </div>
</asp:Content>

