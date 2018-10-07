<%@ control language="C#" autoeventwireup="true" codebehind="ViewList.ascx.cs" inherits="DNNSpecialists.Modules.Reservations.ViewList" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<script type="text/javascript">
    if (typeof (jQuery) == 'function') {
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

        var bindDocumentClick = true;

        function bindEventHandlers() {
            $('.filterTextBox').keypress(function (event) {
                if (event.keyCode == '13') {
                    event.preventDefault();
                    $('.filterImageButton').trigger("click");
                }
            });

            $(".DNNSpecialists_Modules_Reservations_DateFilter_AbsoluteTable").click(function (e) {
                hide = false;
            });

            $(".DNNSpecialists_Modules_Reservations_DateFilter_TextBox").click(function (e) {
                hide = false;
            });

            $(".DNNSpecialists_Modules_Reservations_DateFilter_TextBox").keyup(function (e) {
                $(".DNNSpecialists_Modules_Reservations_DateFilter_AbsoluteTable").hide();
            });

            $(".DNNSpecialists_Modules_Reservations_DateFilter_TableCell").mouseenter(function (e) {
                var date = parseInt($(this).attr("date"));
                var columnName = $(this).attr("columnName");
                var selectedDate = $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_AbsoluteTable").attr("date");

                if (selectedDate != null && selectedDate != "") {
                    selectedDate = parseInt(selectedDate);

                    var start = selectedDate > date ? date : selectedDate;
                    var end = selectedDate > date ? selectedDate : date;

                    var i = start + 1;

                    $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_TableCell a").css("background-color", "");

                    for (; i < end; i++) {
                        var _class = $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_TableCell_" + i).attr("class");

                        if (_class == null || _class.indexOf("DNNSpecialists_Modules_Reservations_SelectedDayStyle") == -1)
                        { $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_TableCell_" + i + " a").css("background-color", "#D8F0FF"); }
                    }
                }
            });

            $(".DNNSpecialists_Modules_Reservations_DateFilter_TableCell").mouseleave(function (e) {
                var columnName = $(this).attr("columnName");
                $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_TableCell a").css("background-color", "");
            });

            if (bindDocumentClick) {
                $(document).click(function (e) {
                    if (hide) {
                        $(".DNNSpecialists_Modules_Reservations_DateFilter_AbsoluteTable").hide();
                    }

                    hide = true;
                });

                bindDocumentClick = false;
            }
        }

        var hide = true;

        function showFilterCalendar(columnName) {
            if (typeof jQuery != 'undefined') {
                $(".DNNSpecialists_Modules_Reservations_DateFilter_AbsoluteTable").hide();
                $(".DNNSpecialists_Modules_Reservations_DateFilter_" + columnName + "_AbsoluteTable").show();
            }
        }
    }
</script>
<div runat="server" id="masterDiv">
    <div runat="server" id="topPagingControlDiv" class="DNNSpecialists_Modules_Reservations_DataGrid_TopPagerStyle"
        visible="false">
        <dnn:pagingcontrol id="topPagingControl" runat="server" enableviewstate="true" />
    </div>
    <asp:datagrid id="dataGrid" runat="server" datakeyfield="ReservationID" enableviewstate="true"
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
                    <asp:label runat="server" id="idLabel" visible="false" text='<%#DataBinder.Eval( Container.DataItem, IdPropertyName ) %>' />
                </itemtemplate>
            </asp:templatecolumn>
        </columns>
    </asp:datagrid>
    <asp:label runat="server" id="numberOfRecordsFoundLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal DNNSpecialists_Modules_Reservations_DataGrid_NumberOfRecordsFound" />
    <div runat="server" id="bottomPagingControlDiv" class="DNNSpecialists_Modules_Reservations_DataGrid_BottomPagerStyle"
        visible="false">
        <dnn:pagingcontrol id="bottomPagingControl" runat="server" enableviewstate="true" />
    </div>
    <table cellpadding="0" cellspacing="0" border="0" width="100%" runat="server" id="buttonsTable">
        <tr>
            <td align="left" style="padding-top: 10px">
                <asp:linkbutton runat="server" id="returnCommandButton" style="width: 80px;" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                onclick="CancelCommandButtonClicked" causesvalidation="false"><asp:image runat="server" imageurl="~/images/lt.gif" width="16" height="16" resourcekey="cancelCommandButton" /><asp:label runat="server" resourcekey="cancelCommandButton" /></asp:linkbutton>
            </td>
            <td align="right" style="padding-top: 10px">
                <asp:linkbutton runat="server" id="exportLinkButton" style="width: 140px"
                onclick="ExportCSVCommandButtonClicked" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                causesvalidation="false"><asp:image runat="server" imageurl="Images/csv.png" width="16" height="16" resourcekey="exportLinkButton" /><asp:label runat="server" resourcekey="exportLinkButton" /></asp:linkbutton>
                <asp:linkbutton runat="server" id="printCommandButton" style="width: 140px;" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                causesvalidation="false"><asp:image runat="server" imageurl="~/images/print.gif" width="16" height="16" resourcekey="printCommandButton" /><asp:label runat="server" resourcekey="printCommandButton" /></asp:linkbutton>
            </td>
        </tr>
    </table>
</div>