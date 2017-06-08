<%@ Page Language="C#" MasterPageFile="../AdminPopup.master" AutoEventWireup="True" Inherits="Hotcakes.Modules.Core.Admin.Orders.PrintOrder" Title="Print Order" CodeBehind="PrintOrder.aspx.cs" %>

<%@ Register Src="OrderActions.ascx" TagName="OrderActions" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HccAdminPopupConent" runat="Server">

    <script type="text/javascript">
        function doPrint() {
            if (window.print) {
                window.print();
            } else {
                alert('Please choose the print button from your browser.  Usually in the menu dropdowns at File: Print');
            }
        }

        jQuery(function ($) {
            if (hcc.getUrlVar('autoprint') == "1") {
                doPrint();
            }
        });
    </script>

    <div style="background: #fff; padding: 10px;">
        <div class="hcNavContent printhidden">
            <uc1:OrderActions ID="OrderActions1" HideActions="true" runat="server" />

            <div class="hcBlockNoBorder">
                 <asp:LinkButton ID="lnkPrintNow" runat="server" Text="Print" CssClass="hcPrimaryAction"  OnClientClick="javascript:doPrint();" />
            </div>
        </div>
        <div class="hcMainContent printhidden">
            <h1>Print Order</h1>
            Template:
            <asp:DropDownList ID="TemplateField" runat="Server"></asp:DropDownList>
            <asp:ImageButton ID="btnGenerate" runat="server"
                ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Go.png" OnClick="btnGenerate_Click" />
        </div>
        <div class="printbackground">
            <asp:DataList BackColor="#FFFFFF" ID="DataList1" runat="server" Width="100%" OnItemDataBound="DataList1_ItemDataBound">
                <ItemTemplate>
                    <div class="printhidden">
                        <hr />
                    </div>
                    <asp:Literal ID="litTemplate" runat="server"></asp:Literal>
                </ItemTemplate>
                <SeparatorTemplate>
                    <div style="page-break-after: always;">&nbsp;</div>
                </SeparatorTemplate>
            </asp:DataList>
        </div>
        <div class="printhidden clear">
            <asp:ImageButton ID="btnContinue2" runat="server" ImageUrl="~/DesktopModules/Hotcakes/Core/Admin/Images/Buttons/Ok.png" OnClick="btnContinue2_Click" />
        </div>

        <div class="clear"></div>
    </div>
</asp:Content>
