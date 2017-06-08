<%@ Page Title="" Language="C#" MasterPageFile="../AdminNav.master" AutoEventWireup="true" CodeBehind="Api.aspx.cs" Inherits="Hotcakes.Modules.Core.Admin.Configuration.Api" %>

<%@ Register Src="../Controls/MessageBox.ascx" TagName="MessageBox" TagPrefix="hcc" %>
<%@ Register Src="../Controls/NavMenu.ascx" TagName="NavMenu" TagPrefix="hcc" %>

<asp:Content ID="nav" ContentPlaceHolderID="NavContent" runat="server">
    <hcc:NavMenu runat="server" ID="NavMenu" BaseUrl="configuration-admin/" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    
    <h1><%=Localization.GetString("APISettings") %></h1>
    <hcc:MessageBox ID="MessageBox1" runat="server" />
    
    <div class="hcFormMessage hcFormInfo">
        <p><%=Localization.GetString("APIMessage1") %></p>
        <p><%=Localization.GetString("APIMessage2") %></p>
        <p><%=Localization.GetString("APIMessage3") %></p>
    </div>

    <h3><%=Localization.GetString("CurrentAPIKeys") %></h3>
    
    <div class="hcForm">
        <div class="hcFormItem">
            <asp:Literal ID="litApiKeys" runat="server" EnableViewState="false"/>
        </div>
        <div class="hcFormItem">
            <asp:LinkButton ID="lnkCreateApiKey" resourcekey="lnkCreateApiKey" runat="server" CausesValidation="False" CssClass="hcPrimaryAction" OnClick="lnkCreateApiKey_Click"/>
        </div>
    </div>
    
    <div class="hcFormMessage hcFormWarning">
        <div class="hcForm">
            <div class="hcFormItem">
                <strong><%=Localization.GetString("ClearOperationsWarning") %></strong>
            </div>
            <div class="hcFormItem">
                <%=Localization.GetString("ClearOperations") %> 
                <asp:Literal ID="litTimeLimit" runat="server"/>.
            </div>
            <div class="hcFormItem">
                <asp:LinkButton ID="btnResetClearTime" resourcekey="btnResetClearTime" CausesValidation="False" runat="server" CssClass="hcSecondaryAction" OnClick="btnResetClearTime_Click"/>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">

        function RemoveApiKey(lnk) {
            var id = lnk.attr('id');
            var idr = id.replace('remove', '');
            $.post('ApiRemoveKey.aspx',
                { "id": idr },
                function () {
                    lnk.parent().parent().slideUp('slow', function () { lnk.parent().remove(); });
                }
                );
        }

        // Jquery Setup
        $(document).ready(function () {
            $('.removeapikey').click(function () {
                RemoveApiKey($(this));
                return false;
            });
        });                      // End Doc Ready
    </script>

</asp:Content>