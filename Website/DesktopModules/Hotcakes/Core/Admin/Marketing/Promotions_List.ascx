<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Promotions_List.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Marketing.Promotions_List" %>
<%@ Register Src="../Controls/Pager.ascx" TagPrefix="hcc" TagName="Pager" %>

<script type="text/javascript">
    hcAttachUpdatePanelLoader();

    <%if (gvPromotions.Columns[0].Visible)
      {%>
    jQuery(function ($) {
        hcUpdatePanelReady(function () {
            $("#<%=gvPromotions.ClientID%> tbody").sortable({
                items: "tr.hcGridRow",
                placeholder: "ui-state-highlight",
                axis: 'y',
                update: function (event, ui) {
                    var ids = $(this).sortable('toArray');
                    ids += '';
                    $.post('MarketingHandler.ashx',
                    {
                        "method": "ResortPromotions",
                        "itemIds": ids,
                        "offset": $('#<%=gvPromotions.ClientID%>').data("rowoffset")
                    });
                }
            }).disableSelection();
        });
    });
    <%}%>
</script>
<div class="hcPromotionBlock">
    <asp:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>

            <div class="hcInfoLabel"><%= Localization.GetFormattedString("PromotionsFound", RowCount)%></div>
            <h2><%=Localization.GetString("Heading_" + Mode.ToString()) %></h2>
            <asp:GridView ID="gvPromotions" AutoGenerateColumns="false" DataKeyNames="Id" CssClass="hcGrid" runat="server" OnRowDataBound="gvPromotions_RowDataBound">
                <RowStyle CssClass="hcGridRow" />
                <HeaderStyle CssClass="hcGridHeader" />
                <Columns>
                    <asp:TemplateField>
                        <ItemStyle Width="22px" CssClass="hcIconWrapper" />
                        <ItemTemplate>
                            <span class='hcIconMove'></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <%# GetStatus(Container) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="IsEnabled" ItemStyle-Width="40px" ItemStyle-CssClass="hcCenter" />
                    <asp:TemplateField>
                        <ItemStyle Width="80px" CssClass="hcIconWrapper" />
                        <ItemTemplate>
                            <asp:HyperLink Text="Edit" NavigateUrl="<%#GetEditUrl(Container) %>" CssClass="hcIconEdit" runat="server" />
                            <asp:LinkButton ID="lnkDelete" Text="Delete" CssClass="hcIconDelete" CommandArgument='<%#Container.DataItemIndex %>' CommandName="Delete"
                                runat="server" OnPreRender="lnkDelete_OnPreRender" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <hcc:Pager ID="ucPager" PageSize="10" runat="server" PostBackMode="true" PageSizeSet="10,50,0" />
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
