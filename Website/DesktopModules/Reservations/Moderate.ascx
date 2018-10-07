<%@ control language="C#" autoeventwireup="true" codebehind="Moderate.ascx.cs" inherits="DNNSpecialists.Modules.Reservations.Moderate" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<script type="text/javascript">
    if (typeof jQuery != 'undefined') {
        (function ($) {
            $(function () {
                var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();

                if (pageRequestManager != null) {
                    pageRequestManager.add_endRequest(function () {
                        bindEventHandlers();
                    });
                }

                bindEventHandlers();
            });
        })(jQuery);

        function bindEventHandlers() {
            $('.filterTextBox').keypress(function (event) {
                if (event.keyCode == '13') {
                    event.preventDefault();
                    $('.filterImageButton').trigger("click");
                }
            });
        }
    }
</script>
<div runat="server" id="topPagingControlDiv" class="DNNSpecialists_Modules_Reservations_DataGrid_TopPagerStyle"
    visible="false">
    <dnn:pagingcontrol id="topPagingControl" runat="server" enableviewstate="true" />
</div>
<asp:datagrid id="dataGrid" runat="server" datakeyfield="PendingApprovalID" enableviewstate="true"
    autogeneratecolumns="False" gridlines="None" width="100%" onsortcommand="SortCommand"
    onitemcommand="ItemCommand" onitemcreated="ItemCreated" onitemdatabound="ItemDataBound"
    cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid">
    <headerstyle cssclass="DNNSpecialists_Modules_Reservations_SubHead DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle" />
    <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle" />
    <alternatingitemstyle cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_AlternatingItemStyle" />
    <pagerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_PagerStyle" />
    <columns>
        <asp:templatecolumn>
            <headerstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_HeaderStyle_Button" />
            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_DataGrid_ItemStyle_Button" />
            <headertemplate>
                <asp:imagebutton runat="server" commandname="Settings" imageurl="~/images/icon_hostsettings_16px.gif"
                    width="16" height="16" resourcekey="Settings" visible="<%#HasEditPermissions%>" />
            </headertemplate>
            <itemtemplate>
                <asp:label runat="server" id="pendingApprovalID" visible="false" text='<%#DataBinder.Eval( Container.DataItem, "PendingApprovalID" ) %>' />
                <asp:imagebutton runat="server" id="approveCommandButton" commandname="Approve" imageurl="grant.gif"
                    width="16" height="16" resourcekey="approveCommandButton" />
                <asp:imagebutton runat="server" id="declineCommandButton" commandname="Decline" imageurl="~/images/delete.gif"
                    width="16" height="16" resourcekey="declineCommandButton" />
                <asp:imagebutton runat="server" id="viewCommandButton" commandname="View" imageurl="~/images/view.gif"
                    width="16" height="16" resourcekey="viewCommandButton" />
            </itemtemplate>
        </asp:templatecolumn>
    </columns>
</asp:datagrid>
<asp:label runat="server" id="numberOfRecordsFoundLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_NumberOfRecordsFound" />
<div runat="server" id="bottomPagingControlDiv" class="DNNSpecialists_Modules_Reservations_DataGrid_BottomPagerStyle"
    visible="false">
    <dnn:pagingcontrol id="bottomPagingControl" runat="server" enableviewstate="true" />
</div>
<%--<div style="width: 100%; text-align: left; padding-top: 10px;">
    <dnn:commandbutton runat="server" id="cancelCommandButton" resourcekey="cancelCommandButton"
        imageurl="~/images/lt.gif" cssclass="CommandButton DNNSpecialists_Modules_Reservations_LinkCommandButton" />
</div>--%>
<div style="width: 100%; text-align: center; padding-top: 10px;">
    <asp:linkbutton runat="server" id="returnCommandButton" style="width: 80px;"
        cssclass="DNNSpecialists_Modules_Reservations_CommandButton" onclick="CancelCommandButtonClicked"
        causesvalidation="false"><asp:image runat="server" imageurl="~/images/lt.gif" width="16" height="16" resourcekey="cancelCommandButton" /><asp:label runat="server" resourcekey="cancelCommandButton" /></asp:linkbutton>
</div>
