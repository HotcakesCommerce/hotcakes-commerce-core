<%@ control language="C#" autoeventwireup="true" codebehind="FeeScheduleControl.ascx.cs"
    inherits="DNNSpecialists.Modules.Reservations.FeeScheduleControl" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<asp:validationsummary runat="server" id="validationSummary" visible="false" cssclass="Normal" style="text-align: left; color: red; display: block; margin: 0 0 10px 0" enableclientscript="false" showmessagebox="false" showsummary="true" displaymode="BulletList"/>
<asp:customvalidator runat="server" controltovalidate="seasonalEndDayTextBox" onservervalidate="ValidateDateRange" display="Dynamic" style="color: red;" resourcekey="InvalidDateRange" validationgroup="seasonalValidationGroup" />
<asp:customvalidator runat="server" controltovalidate="seasonalEndDayTextBox" onservervalidate="ValidateSeasonalFeeList" display="Dynamic" style="color: red;" resourcekey="InvalidSeasonalFeeList" />
<table cellpadding="0" cellspacing="3" border="0" width="100%">
    <tr>
        <td width="50px" nowrap="nowrap">
            <dnn:label runat="server" resourcekey="scheduleLabel" />
        </td>
        <td>
            <table cellpadding="0" cellspacing="3" border="0">
                <tr>
                    <td style="padding-right: 10px;" nowrap="nowrap">
                        <asp:radiobutton runat="server" id="freeScheduleRadioButton" checked="true" groupname="scheduleTypeRadioButtons" resourcekey="Free"
                            cssclass="Normal" autopostback="true" oncheckedchanged="ScheduleTypeChanged" />
                        <br />
                        <br />
                        <asp:radiobutton runat="server" id="flatScheduleRadioButton" checked="false" groupname="scheduleTypeRadioButtons" resourcekey="Flat"
                            cssclass="Normal" autopostback="true" oncheckedchanged="ScheduleTypeChanged" />
                        <br />
                        <br />
                        <asp:radiobutton runat="server" id="seasonalScheduleRadioButton" groupname="scheduleTypeRadioButtons" resourcekey="Seasonal"
                            checked="false" cssclass="Normal" autopostback="true" oncheckedchanged="ScheduleTypeChanged" />
                    </td>
                    <td rowspan="4" style="padding-left:10px; border-left: solid 1px #5c6f93">
                        <asp:placeholder runat="server" id="freechedulePlaceHolder" visible="false"><asp:label runat="server" resourcekey="freeReservationsLabel" cssclass="Normal" /></asp:placeholder>
                        <asp:placeholder runat="server" id="flatSchedulePlaceHolder" visible="false">
                            <table cellpadding="0" cellspacing="3" border="0" width="300px">
                                <tr>
                                    <td width="250">
                                        <dnn:label id="depositFeeLabel" runat="server" controlname="depositFeeTextBox" />
                                    </td>
                                    <td valign="top" nowrap="nowrap">
                                        <asp:textbox runat="server" id="depositFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                            width="50" />&nbsp;
                                        <asp:label runat="server" resourcekey="depositFeeTypeLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                        <asp:rangevalidator runat="server" controltovalidate="depositFeeTextBox" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                            maximumvalue="65535" type="Currency" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="depositFeeTextBox"
                                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td width="250">
                                        <dnn:label id="schedulingFeeLabel" runat="server" controlname="schedulingFeeTextBox" />
                                    </td>
                                    <td valign="top" nowrap="nowrap">
                                        <asp:textbox runat="server" id="schedulingFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                            width="50" />&nbsp;
                                        <asp:label runat="server" resourcekey="schedulingFeeTypeLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />&nbsp;
                                        <asp:textbox runat="server" id="schedulingFeeIntervalTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                            width="50" />&nbsp;
                                        <asp:dropdownlist runat="server" id="schedulingFeeIntervalDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                        <asp:rangevalidator runat="server" controltovalidate="schedulingFeeTextBox" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                            maximumvalue="65535" type="Currency" />
                                        <asp:rangevalidator runat="server" controltovalidate="schedulingFeeIntervalTextBox" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="1"
                                            maximumvalue="65535" type="Integer" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="schedulingFeeTextBox"
                                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="schedulingFeeIntervalTextBox"
                                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td width="250">
                                        <dnn:label id="reschedulingFeeLabel" runat="server" controlname="reschedulingFeeTextBox" />
                                    </td>
                                    <td valign="top" nowrap="nowrap">
                                        <asp:textbox runat="server" id="reschedulingFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                            width="50" />&nbsp;
                                        <asp:label runat="server" resourcekey="reschedulingFeeTypeLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                        <asp:rangevalidator runat="server" controltovalidate="reschedulingFeeTextBox" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                            maximumvalue="65535" type="Currency" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="reschedulingFeeTextBox"
                                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                    </td>
                                </tr>
                                <tr>
                                    <td width="250">
                                        <dnn:label id="cancellationFeeLabel" runat="server" controlname="cancellationFeeTextBox" />
                                    </td>
                                    <td valign="top">
                                        <asp:textbox runat="server" id="cancellationFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                            width="50" />&nbsp;
                                        <asp:label runat="server" resourcekey="cancellationFeeTypeLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                        <asp:rangevalidator runat="server" controltovalidate="cancellationFeeTextBox" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                            maximumvalue="65535" type="Currency" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="cancellationFeeTextBox"
                                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                        <asp:customvalidator runat="server" controltovalidate="cancellationFeeTextBox" validateemptytext="true"
                                            setfocusonerror="true" onservervalidate="ValidateCancellationFee" resourcekey="starLabel"
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" />
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                        <asp:placeholder runat="server" id="seasonalSchedulePlaceHolder" visible="false">
                            <table cellpadding="0" cellspacing="0" border="0" width="300px">
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:datagrid runat="server" id="seasonalFeesDataGrid" showheader="False" showfooter="False"
                                            cellpadding="0" cellspacing="5" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                                            ondeletecommand="DeleteSeasonalFee">
                                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle2" />
                                            <columns>
                                                <asp:templatecolumn runat="server">
                                                    <itemtemplate>
                                                        <table width="100%" cellpadding="0" cellspacing="10" class="DNNSpecialists_Modules_Reservations_Settings_DataGrid_SeasonalFees">
                                                            <tr>
                                                                <td nowrap="nowrap">
                                                                    <asp:label runat="server" text='<%#GetSeasonalFee(Container.DataItem)%>'
                                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                                </td>
                                                                <td align="right">
                                                                    <asp:imagebutton runat="server" imageurl="~/images/delete.gif" resourcekey="Delete"
                                                                    commandname="Delete" causesvalidation="false" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </itemtemplate>
                                                </asp:templatecolumn>
                                            </columns>
                                        </asp:datagrid>
                                        <asp:label runat="server" id="noSeasonalFeesLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                                            resourcekey="noSeasonalFeesLabel" />
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2"><hr /></td>
                                </tr>
                                <tr runat="server" id="addTableRow1" visible="false">
                                    <td colspan="2" align="center">
                                        <table cellpadding="0" cellspacing="5" border="0">
                                            <tr>
                                                <td align="right"><asp:label runat="server" resourcekey="StartOn" cssclass="SubHead" /></td>
                                                <td>
                                                    <asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="seasonalStartMonthDropDownList" />&nbsp;<asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="seasonalStartDayTextBox" text="1" width="30px" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalStartDayTextBox"
                                                        enableclientscript="false" display="None" resourcekey="starLabel" validationgroup="seasonalValidationGroup" />
                                                </td>
                                                <td align="right"><asp:label runat="server" resourcekey="EndBy" cssclass="SubHead" /></td>
                                                <td>
                                                    <asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="seasonalEndMonthDropDownList" />&nbsp;<asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="seasonalEndDayTextBox" text="1" width="30px" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalEndDayTextBox"
                                                        enableclientscript="false" display="None" resourcekey="starLabel" validationgroup="seasonalValidationGroup" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr runat="server" id="addTableRow2" visible="false">
                                    <td colspan="2"><hr /></td>
                                </tr>
                                <tr runat="server" id="addTableRow3" visible="false">
                                    <td colspan="2">
                                        <table cellpadding="0" cellspacing="3" border="0" width="300px">
                                            <tr>
                                                <td width="250">
                                                    <dnn:label id="seasonalDepositFeeLabel" resourcekey="depositFeeLabel" runat="server" controlname="seasonalDepositFeeTextBox" />
                                                </td>
                                                <td valign="top" nowrap="nowrap">
                                                    <asp:textbox runat="server" id="seasonalDepositFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                                        width="50" />&nbsp;
                                                    <asp:label runat="server" resourcekey="depositFeeTypeLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                                    <asp:rangevalidator runat="server" controltovalidate="seasonalDepositFeeTextBox" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                                        maximumvalue="65535" type="Currency" validationgroup="seasonalValidationGroup" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalDepositFeeTextBox"
                                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dnn:label id="seasonalSchedulingFeeLabel" runat="server" controlname="schedulingFeeTextBox" />
                                                </td>
                                                <td valign="top" nowrap="nowrap">
                                                    <asp:textbox runat="server" id="seasonalSchedulingFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                                        width="50" />&nbsp;
                                                    <asp:label runat="server" resourcekey="schedulingFeeTypeLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />&nbsp;
                                                    <asp:textbox runat="server" id="seasonalSchedulingFeeIntervalTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                                        width="50" />&nbsp;
                                                    <asp:dropdownlist runat="server" id="seasonalSchedulingFeeIntervalDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                    <asp:rangevalidator runat="server" controltovalidate="seasonalSchedulingFeeTextBox" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                                        maximumvalue="65535" type="Currency" validationgroup="seasonalValidationGroup" />
                                                    <asp:rangevalidator runat="server" controltovalidate="seasonalSchedulingFeeIntervalTextBox" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="1"
                                                        maximumvalue="65535" type="Integer" validationgroup="seasonalValidationGroup" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalSchedulingFeeTextBox"
                                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalSchedulingFeeIntervalTextBox"
                                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dnn:label id="seasonalReschedulingFeeLabel" runat="server" controlname="reschedulingFeeTextBox" />
                                                </td>
                                                <td valign="top" nowrap="nowrap">
                                                    <asp:textbox runat="server" id="seasonalReschedulingFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                                        width="50" />&nbsp;
                                                    <asp:label runat="server" resourcekey="reschedulingFeeTypeLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                                    <asp:rangevalidator runat="server" controltovalidate="seasonalReschedulingFeeTextBox" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                                        maximumvalue="65535" type="Currency" validationgroup="seasonalValidationGroup" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalReschedulingFeeTextBox"
                                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dnn:label id="seasonalCancellationFeeLabel" runat="server" controlname="cancellationFeeTextBox" />
                                                </td>
                                                <td valign="top">
                                                    <asp:textbox runat="server" id="seasonalCancellationFeeTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                                        width="50" />&nbsp;
                                                    <asp:label runat="server" resourcekey="cancellationFeeTypeLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                                    <asp:rangevalidator runat="server" controltovalidate="seasonalCancellationFeeTextBox" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" minimumvalue="0"
                                                        maximumvalue="65535" type="Currency" validationgroup="seasonalValidationGroup" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="seasonalCancellationFeeTextBox"
                                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                    <asp:customvalidator runat="server" controltovalidate="seasonalCancellationFeeTextBox" validateemptytext="true"
                                                        setfocusonerror="true" onservervalidate="ValidateCancellationFee2" resourcekey="starLabel"
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="false" display="Dynamic" validationgroup="seasonalValidationGroup" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr runat="server" id="addTableRow4" visible="false">
                                    <td colspan="2"><hr /></td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="center">
                                        <asp:linkbutton runat="server" id="seasonalSaveCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" validationgroup="seasonalValidationGroup" visible="false"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="Save" /></asp:linkbutton>
                                        <asp:linkbutton runat="server" id="seasonalCancelCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" visible="false" causesvalidation="false" style="margin-left: 10px;"><asp:image runat="server" imageurl="~/images/delete.gif" /><asp:label runat="server" resourcekey="Cancel" /></asp:linkbutton>
                                        <asp:linkbutton runat="server" id="seasonalAddCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" causesvalidation="false"><asp:image runat="server" imageurl="~/images/add.gif" /><asp:label runat="server" resourcekey="Add" /></asp:linkbutton>
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
