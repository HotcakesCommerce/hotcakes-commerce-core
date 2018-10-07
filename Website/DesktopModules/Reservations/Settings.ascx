<%@ control language="C#" autoeventwireup="true" codebehind="Settings.ascx.cs" inherits="DNNSpecialists.Modules.Reservations.Settings" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<%@ register tagprefix="dnn" tagname="SectionHead" src="~/controls/SectionHeadControl.ascx" %>
<%@ register tagprefix="dnn" assembly="DotNetNuke" namespace="DotNetNuke.UI.WebControls" %>
<%@ register tagprefix="dnns" tagname="recurrencepatterncontrol" src="RecurrencePatternControl.ascx" %>
<%@ register tagprefix="dnns" tagname="feeschedulecontrol" src="FeeScheduleControl.ascx" %>
<table cellspacing="0" cellpadding="5" border="0" summary="Reservations Settings Design Table"
    width="100%">
    <tr runat="server" id="generalSettingsSectionTableRow">
        <td>
            <dnn:sectionhead id="generalSettingsSectionHead" cssclass="Head" section="generalSettingsTable"
                includerule="true" resourcekey="GeneralSettings" isexpanded="true" runat="server" />
            <table runat="server" id="generalSettingsTable" cellspacing="0" cellpadding="5" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="250">
                        <dnn:label id="timeZoneLabel" runat="server" controlname="timeZoneDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="timeZoneDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="themeLabel" runat="server" controlname="themeDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="themeDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="skipContactInfoLabel" runat="server" controlname="skipContactInfoCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="skipContactInfoCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="contactInfoFirstLabel" runat="server" controlname="contactInfoFirstCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="contactInfoFirstCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="categorySelectionModeTableRow">
                    <td width="250">
                        <dnn:label id="categorySelectionModeLabel" runat="server" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_Normal">
                        <asp:radiobutton runat="server" id="categorySelectionModeList" groupname="categorySelectionMode"
                            resourcekey="List" />
                        <asp:radiobutton runat="server" id="categorySelectionModeDropDownList" groupname="categorySelectionMode"
                            resourcekey="DropDownList" />
                        <asp:radiobutton runat="server" id="categorySelectionModeListBox" groupname="categorySelectionMode"
                            resourcekey="ListBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="displayCalendar" runat="server" controlname="displayListRadioButton" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_Normal">
                        <asp:radiobutton runat="server" id="displayListRadioButton" groupname="displayCalendar"
                            resourcekey="List" />
                        <asp:radiobutton runat="server" id="displayCalendarRadioButton" groupname="displayCalendar"
                            resourcekey="displayCalendarRadioButton" />
                    </td>
                </tr>
                <tr runat="server" id="timeOfDaySelectionModeTableRow">
                    <td width="250">
                        <dnn:label id="timeOfDaySelectionModeLabel" runat="server" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_Normal">
                        <asp:radiobutton runat="server" id="timeOfDaySelectionModeList" groupname="timeOfDaySelectionMode"
                            resourcekey="List" />
                        <asp:radiobutton runat="server" id="timeOfDaySelectionModeDropDownList" groupname="timeOfDaySelectionMode"
                            resourcekey="DropDownList" />
                        <asp:radiobutton runat="server" id="timeOfDaySelectionModeListBox" groupname="timeOfDaySelectionMode"
                            resourcekey="ListBox" />
                    </td>
                </tr>
                <tr runat="server" id="timeSelectionModeTableRow">
                    <td width="250">
                        <dnn:label id="timeSelectionModeLabel" runat="server" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_Normal">
                        <asp:radiobutton runat="server" id="timeSelectionModeList" groupname="timeSelectionMode"
                            resourcekey="List" />
                        <asp:radiobutton runat="server" id="timeSelectionModeDropDownList" groupname="timeSelectionMode"
                            resourcekey="DropDownList" />
                        <asp:radiobutton runat="server" id="timeSelectionModeListBox" groupname="timeSelectionMode"
                            resourcekey="ListBox" />
                    </td>
                </tr>
                <tr runat="server" id="durationSelectionModeTableRow">
                    <td width="250">
                        <dnn:label id="durationSelectionModeLabel" runat="server" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_Normal">
                        <asp:radiobutton runat="server" id="durationSelectionModeList" groupname="durationSelectionMode"
                            resourcekey="List" />
                        <asp:radiobutton runat="server" id="durationSelectionModeDropDownList" groupname="durationSelectionMode"
                            resourcekey="DropDownList" />
                        <asp:radiobutton runat="server" id="durationSelectionModeListBox" groupname="durationSelectionMode"
                            resourcekey="ListBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="displayRemainingReservationsLabel" runat="server" controlname="displayRemainingReservationsCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="displayRemainingReservationsCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="displayEndTimeLabel" runat="server" controlname="displayEndTimeCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="displayEndTimeCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="requireEmailLabel" runat="server" controlname="requireEmailCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="requireEmailCheckBox" autopostback="true" oncheckedchanged="RequireEmailCheckBoxCheckChanged" />
                    </td>
                </tr>
                <tr runat="server" id="requireVerificationCodeTableRow">
                    <td width="250">
                        <dnn:label id="requireVerificationCodeLabel" runat="server" controlname="requireVerificationCodeCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="requireVerificationCodeCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="requirePhoneLabel" runat="server" controlname="requirePhoneCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="requirePhoneCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="allowLookupByPhoneLabel" runat="server" controlname="allowLookupByPhoneCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowLookupByPhoneCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="allowDescription" runat="server" controlname="allowDescriptionCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowDescriptionCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="allowSchedulingAnotherReservationLabel" runat="server" controlname="allowSchedulingAnotherReservationCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowSchedulingAnotherReservationCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="redirectUrlLabel" runat="server" controlname="redirectUrlTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="redirectUrlTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="categoriesSectionTableRow">
        <td>
            <dnn:sectionhead id="categoriesSectionHead" cssclass="Head" section="categoriesTable"
                includerule="true" resourcekey="CategoriesSettings" isexpanded="false" runat="server" />
            <table runat="server" id="categoriesTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="250">
                        <dnn:label id="allowCategorySelectionLabel" runat="server" controlname="allowCategorySelectionCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowCategorySelectionCheckBox" oncheckedchanged="AllowCategorySelectionCheckBoxCheckedChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="selectCategoryLastTableRow">
                    <td width="250">
                        <dnn:label id="selectCategoryLastLabel" runat="server" controlname="selectCategoryLastCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="selectCategoryLastCheckBox" oncheckedchanged="SelectCategoryLastChanged" autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="categoryListTableRow">
                    <td width="250">
                        <dnn:label id="categoryListLabel" runat="server" controlname="categoryNameTextBox" />
                    </td>
                    <td valign="top">
                        <asp:requiredfieldvalidator runat="server" controltovalidate="categoryNameTextBox"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Static" validationgroup="categoryNameValidationGroup" />
                        <asp:textbox runat="server" id="categoryNameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                    <td width="16">
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddCategoryCommandButtonClicked" causesvalidation="true" validationgroup="categoryNameValidationGroup" />
                    </td>
                </tr>
                <tr runat="server" id="categoryListTableRow2">
                    <td width="250">
                    </td>
                    <td valign="top">
                        <asp:datagrid runat="server" id="categoryListDataGrid" showheader="False" showfooter="False"
                            cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                            ondeletecommand="DeleteCategory">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="categoryID" visible="false" text='<%#DataBinder.Eval( Container.DataItem, "CategoryID")%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#DataBinder.Eval( Container.DataItem, "Name")%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false"  onclientclick='<%#"return confirm(\"" + Localization.GetString( "ConfirmDelete", LocalResourceFile ) + "\")"%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noCategoriesLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noCategoriesLabel" />
                    </td>
                </tr>
                <tr runat="server" id="allowCrossCategoryConflictsTableRow">
                    <td width="250">
                        <dnn:label id="allowCrossCategoryConflictsLabel" runat="server" controlname="preventCrossCategoryConflictsCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="preventCrossCategoryConflictsCheckBox" oncheckedchanged="PreventCrossCategoryConflictsChanged" autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="bindUponSelectionTableRow">
                    <td width="250">
                        <dnn:label id="bindUponSelectionLabel" runat="server" controlname="bindUponSelectionCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="bindUponSelectionCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="displayUnavailableCategoriesTableRow">
                    <td width="250">
                        <dnn:label id="displayUnavailableCategoriesLabel" runat="server" controlname="displayUnavailableCategoriesCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="displayUnavailableCategoriesCheckBox" />
                    </td>
                </tr>

                <tr runat="server" id="categoryPermissionsTableRow">
                    <td valign="top" style="padding-top: 16px;" width="200">
                        <dnn:label id="categoryPermissionsLabel" runat="server" controlname="categoryPermissionsDropDownList" />
                    </td>
                    <td valign="top" style="padding-top: 8px;" colspan="2">
                        <table cellpadding="5" cellspacing="0" width="100%" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                            <tr>
                                <td class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                                    <asp:dropdownlist runat="server" id="categoryPermissionsDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="100%" onselectedindexchanged="CategoryPermissionsDropDownListSelectedIndexChanged"
                                        autopostback="true" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:checkboxlist runat="server" id="categoryPermissionsCheckboxList" cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:linkbutton runat="server" id="categoryPermissionsUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                            </td>
                                            <td style="padding-left: 5px">
                                                <asp:linkbutton runat="server" id="categoryPermissionsResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="reservationSettingsSectionTableRow">
        <td>
            <dnn:sectionhead id="reservationSettingsSectionHead" cssclass="Head" section="reservationSettingsTable"
                includerule="true" resourcekey="ReservationSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="reservationSettingsTable" cellpadding="5" cellspacing="0"
                border="0" width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="reservationSettingsCategoryTableRow" visible="false">
                    <td valign="top" colspan="2" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="reservationSettingsCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="ReservationSettingsCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="allowCancellationsLabel" runat="server" controlname="allowCancellationsCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowCancellationsCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="allowReschedulingLabel" runat="server" controlname="allowReschedulingCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowReschedulingCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="minTimeAheadLabel" runat="server" controlname="minTimeAheadTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="minTimeAheadTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="minTimeAheadDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:rangevalidator runat="server" controltovalidate="minTimeAheadTextBox" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" minimumvalue="-9999"
                            maximumvalue="9999" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="minTimeAheadTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="daysAheadLabel" runat="server" controlname="daysAheadTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="daysAheadTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:label runat="server" resourcekey="daysLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                        <asp:rangevalidator runat="server" controltovalidate="daysAheadTextBox" resourcekey="daysAheadTextBoxRangeValidator"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" minimumvalue="0"
                            maximumvalue="365" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="daysAheadTextBox" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="reservationIntervalLabel" runat="server" controlname="reservationIntervalTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="reservationIntervalTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="reservationIntervalDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:customvalidator runat="server" controltovalidate="reservationIntervalTextBox"
                            validateemptytext="false" setfocusonerror="true" onservervalidate="ValidateReservationInterval"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="reservationIntervalTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="reservationDurationLabel" runat="server" controlname="reservationDurationTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="reservationDurationTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="reservationDurationDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:customvalidator runat="server" controltovalidate="reservationDurationTextBox"
                            validateemptytext="false" setfocusonerror="true" onservervalidate="ValidateReservationDuration"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="reservationDurationTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="reservationDurationIntervalLabel" runat="server" controlname="reservationDurationIntervalTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="reservationDurationIntervalTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="reservationDurationIntervalDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:customvalidator runat="server" controltovalidate="reservationDurationIntervalTextBox"
                            validateemptytext="false" setfocusonerror="true" onservervalidate="ValidateReservationDurationInterval"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="reservationDurationIntervalTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="reservationDurationMaxLabel" runat="server" controlname="reservationDurationMaxTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="reservationDurationMaxTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="reservationDurationMaxDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:customvalidator runat="server" controltovalidate="reservationDurationMaxTextBox"
                            validateemptytext="false" setfocusonerror="true" onservervalidate="ValidateReservationDurationMax"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="reservationDurationMaxTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="maxConflictingReservationsLabel" runat="server" controlname="maxConflictingReservationsTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="maxConflictingReservationsTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:rangevalidator runat="server" controltovalidate="maxConflictingReservationsTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic"
                            minimumvalue="0" maximumvalue="9999" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="maxConflictingReservationsTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="maxReservationsPerUserLabel" runat="server" controlname="maxReservationsPerUserTextBox"></dnn:label>
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="maxReservationsPerUserTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input" width="50" />
                        <asp:rangevalidator runat="server" controltovalidate="maxReservationsPerUserTextBox" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" minimumvalue="1"
                            maximumvalue="9999" type="Integer" />
                    </td>
                </tr>
                <tr runat="server" id="reservationSettingsUpdateResetTableRow">
                    <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="reservationSettingsUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="reservationSettingsResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="feesSectionTableRow">
        <td>
            <dnn:sectionhead id="feesSectionHead" cssclass="Head" section="feesTable" includerule="true"
                resourcekey="FeesSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="feesTable" cellpadding="5" cellspacing="0" border="0" width="500"
                align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="reservationFeesCategoryTableRow" visible="false">
                    <td valign="top" colspan="2" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="reservationFeesCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="ReservationFeesCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="paymentMethodTableRow">
                    <td width="250">
                        <dnn:label id="paymentMethodLabel" runat="server" controlname="paymentMethodDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="paymentMethodDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" onselectedindexchanged="PaymentMethodChanged" autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="payPalAccountTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="payPalAccountLabel" runat="server" controlname="payPalAccountTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:textbox runat="server" id="payPalAccountTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:customvalidator runat="server" controltovalidate="payPalAccountTextBox" validateemptytext="true"
                            setfocusonerror="true" onservervalidate="ValidatePayPalAccount" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="payPalUrlTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="payPalUrlLabel" runat="server" controlname="payPalUrlTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:textbox runat="server" id="payPalUrlTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:customvalidator runat="server" controltovalidate="payPalUrlTextBox" validateemptytext="true"
                            setfocusonerror="true" onservervalidate="ValidatePayPalUrl" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="authorizeNetApiLoginTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="authorizeNetApiLoginLabel" runat="server" controlname="authorizeNetApiLoginTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:textbox runat="server" id="authorizeNetApiLoginTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:customvalidator runat="server" controltovalidate="authorizeNetApiLoginTextBox" validateemptytext="true"
                            setfocusonerror="true" onservervalidate="ValidateAuthorizeNetApiLogin" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="authorizeNetTransactionKeyTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="authorizeNetTransactionKeyLabel" runat="server" controlname="authorizeNetTransactionKeyTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:textbox runat="server" id="authorizeNetTransactionKeyTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:customvalidator runat="server" controltovalidate="authorizeNetTransactionKeyTextBox" validateemptytext="true"
                            setfocusonerror="true" onservervalidate="ValidateAuthorizeNetTransactionKey" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="authorizeNetMerchantHashTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="authorizeNetMerchantHashLabel" runat="server" controlname="authorizeNetMerchantHashTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:textbox runat="server" id="authorizeNetMerchantHashTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:customvalidator runat="server" controltovalidate="authorizeNetMerchantHashTextBox" validateemptytext="true"
                            setfocusonerror="true" onservervalidate="ValidateAuthorizeNetMerchantHash" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="authorizeNetTestModeTableRow">
                    <td width="250" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <dnn:label id="authorizeNetTestModeLabel" runat="server" controlname="authorizeNetTestModeTextBox" />
                    </td>
                    <td valign="top" class="DNNSpecialists_Modules_Reservations_PaymentMethodTableCell">
                        <asp:checkbox runat="server" id="authorizeNetTestModeCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="itemDescriptionTableRow">
                    <td width="250">
                        <dnn:label id="itemDescriptionLabel" runat="server" controlname="payPalItemDescriptionTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="payPalItemDescriptionTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="payPalItemDescriptionTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="pendingPaymentExpirationTableRow">
                    <td width="250">
                        <dnn:label id="pendingPaymentExpirationLabel" runat="server" controlname="pendingPaymentExpirationTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="pendingPaymentExpirationTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:label runat="server" resourcekey="minutesLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                        <asp:rangevalidator runat="server" controltovalidate="pendingPaymentExpirationTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic"
                            minimumvalue="1" maximumvalue="65535" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="pendingPaymentExpirationTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="currencyTableRow">
                    <td width="250">
                        <dnn:label id="currencyLabel" runat="server" controlname="currencyDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="currencyDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr runat="server" id="allowPayLaterTableRow">
                    <td width="250">
                        <dnn:label id="allowPayLaterLabel" runat="server" controlname="allowPayLaterCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="allowPayLaterCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="padding: 20px; border-top: solid 1px #ccc">
                        <dnns:feeschedulecontrol runat="server" id="feeschedulecontrol" visible="true" />
                    </td>
                </tr>
                <tr runat="server" id="reservationFeesUpdateResetTableRow">
                    <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="reservationFeesUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="reservationFeesResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" causesvalidation="false"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="cashierListSectionTableRow">
        <td>
            <dnn:sectionhead id="cashierListSectionHead" cssclass="Head" section="cashierListTable"
                includerule="true" resourcekey="CashierList" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="cashierListTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="cashierListCategoryTableRow" visible="false">
                    <td valign="top" colspan="3" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="cashierListCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="CashierListCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="cashierListLabel" runat="server" controlname="cashierListUsersDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="cashierListUsersDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:textbox runat="server" width="100%" id="cashierListUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            visible="false" validationgroup="cashierListUsernameValidationGroup" />
                        <asp:requiredfieldvalidator runat="server" id="cashierListUsernameRequiredFieldValidator"
                            controltovalidate="cashierListUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true"
                            display="Static" resourcekey="starLabel" validationgroup="cashierListUsernameValidationGroup"
                            visible="false" />
                    </td>
                    <td>
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddCashierCommandButtonClicked" causesvalidation="true" validationgroup="cashierListUsernameValidationGroup" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <asp:datagrid runat="server" id="cashierListDataGrid" showheader="False" showfooter="False"
                            cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                            ondeletecommand="DeleteCashier">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="userId" visible="false" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).UserID%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).DisplayName%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noCashiersLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noCashiersLabel" />
                    </td>
                </tr>
                <tr runat="server" id="cashierUpdateResetTableRow">
                    <td colspan="3" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="cashierListUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="cashierListResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="workingHoursSectionTableRow">
        <td>
            <dnn:sectionhead id="workingHoursSectionHead" cssclass="Head" section="workingHoursTable"
                includerule="true" resourcekey="WorkingHoursSettings" isexpanded="false" runat="server">
            </dnn:sectionhead>
            <table runat="server" id="workingHoursTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="workingHoursCategoryTableRow" visible="false">
                    <td valign="top" colspan="2" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="workingHoursCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="WorkingHoursCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="padding: 0px" align="center">
                        <table cellpadding="10" cellspacing="0" border="0">
                            <%--<tr>
                                <td valign="top" nowrap="nowrap" rowspan="2">
                                    <asp:dropdownlist runat="server" id="weekDaysDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                </td>
                                <td valign="top" align="left" nowrap="nowrap">
                                    <asp:label runat="server" resourcekey="fromLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                    <asp:dropdownlist runat="server" id="startHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="startMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="startAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:label runat="server" cssclass="DNNSpecialists_Modules_Reservations_SubHead" resourcekey="toLabel" />
                                    <asp:dropdownlist runat="server" id="endHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="endMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="endAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:customvalidator runat="server" validationgroup="workingHoursValidationGroup"
                                        controltovalidate="endHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Normal" onservervalidate="ValidateWorkingHours" />
                                </td>
                                <td valign="middle" nowrap="nowrap">
                                    <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                                        onclick="AddWorkingHoursCommandButtonClicked" causesvalidation="true" validationgroup="workingHoursValidationGroup" />
                                </td>
                            </tr>--%>
                            <tr>
                                <td align="center">
                                    <asp:datagrid runat="server" id="workingHoursDataGrid" showheader="False" showfooter="False"
                                        cellpadding="0" cellspacing="5" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                                        ondeletecommand="DeleteWorkingHours">
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" horizontalalign="Left" />
                                        <columns>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:imagebutton runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                                        commandname="Delete" causesvalidation="false" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:label runat="server" text='<%#GetWorkingHours( Container.DataItem )%>'
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <asp:label runat="server" id="noWorkingHoursLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                                        resourcekey="noWorkingHoursLabel" style="display: block; margin: 10px 0 0 0" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" style="padding: 20px; border-top: solid 1px #ccc">
                                    <asp:linkbutton runat="server" id="addWorkingHoursCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                                        causesvalidation="false" onclick="AddWorkingHours"><asp:image runat="server" imageurl="~/images/add.gif" /><asp:label runat="server" resourcekey="addWorkingHoursCommandButton" /></asp:linkbutton>
                                    <dnns:recurrencepatterncontrol runat="server" id="recurrencepatterncontrol" visible="false" submitimageurl="~/images/add.gif" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" id="workingHoursUpdateResetTableRow">
                    <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="workingHoursUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="workingHoursResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="workingHoursExceptionsSectionTableRow">
        <td>
            <dnn:sectionhead id="workingHoursExceptionsSectionHead" cssclass="Head" section="workingHoursExceptionsTable"
                includerule="true" resourcekey="ExceptionsSettings" isexpanded="false" runat="server">
            </dnn:sectionhead>
            <table runat="server" id="workingHoursExceptionsTable" cellpadding="5" cellspacing="0">
                <tr>
                    <td>
                        <asp:label runat="server" resourcekey="ExceptionsSettings.Help" cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table cellpadding="5" cellspacing="0" border="0" width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable"
                            style="margin-top: 5px">
                            <tr runat="server" id="workingHoursExceptionsCategoryTableRow" visible="false">
                                <td valign="top" colspan="2" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                                    <asp:dropdownlist runat="server" id="workingHoursExceptionsCategoryDropDownList"
                                        cssclass="DNNSpecialists_Modules_Reservations_Input" width="100%" onselectedindexchanged="WorkingHoursExceptionsCategoryDropDownListSelectedIndexChanged"
                                        autopostback="true" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" style="padding: 0px" align="center">
                                    <table cellpadding="5" cellspacing="0" border="0">
                                        <tr>
                                            <td valign="middle" align="right" nowrap="nowrap">
                                                <table cellpadding="0" cellspacing="0" border="0">
                                                    <tr>
                                                        <td valign="middle" align="right" nowrap="nowrap">
                                                            <asp:textbox runat="server" id="workingHoursExceptionDateTextBox" width="100" style="text-align: center" />
                                                        </td>
                                                        <td style="padding-left: 3px">
                                                            <asp:image runat="server" id="workingHoursExceptionDateImage" imageurl="~/images/calendar.png"
                                                                style="cursor: pointer" />
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:customvalidator runat="server" validateemptytext="true" validationgroup="workingHoursExceptionValidationGroup"
                                                    controltovalidate="workingHoursExceptionDateTextBox" cssclass="DNNSpecialists_Modules_Reservations_Normal" onservervalidate="ValidateWorkingHoursExceptionDate"
                                                    display="Dynamic" />
                                            </td>
                                            <td align="left" nowrap="nowrap">
                                                <asp:radiobutton runat="server" id="workingHoursExceptionFromRadioButton" groupname="workingHoursExceptionRadioButtonGroup"
                                                    resourcekey="fromLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" checked="true" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionStartHourDropDownList"
                                                    cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionStartMinuteDropDownList"
                                                    cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionStartAMPMDropDownList"
                                                    cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:label runat="server" cssclass="DNNSpecialists_Modules_Reservations_SubHead" resourcekey="toLabel" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionEndHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionEndMinuteDropDownList"
                                                    cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:dropdownlist runat="server" id="workingHoursExceptionEndAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                                <asp:customvalidator runat="server" validationgroup="workingHoursExceptionValidationGroup"
                                                    controltovalidate="workingHoursExceptionEndHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Normal"
                                                    onservervalidate="ValidateWorkingHoursException" />
                                            </td>
                                            <td valign="middle" nowrap="nowrap">
                                                <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                                                    onclick="AddWorkingHoursExceptionCommandButtonClicked" causesvalidation="true"
                                                    validationgroup="workingHoursExceptionValidationGroup" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td valign="top" align="left">
                                                <asp:radiobutton runat="server" id="workingHoursExceptionNoWorkingHoursRadioButton"
                                                    groupname="workingHoursExceptionRadioButtonGroup" resourcekey="workingHoursExceptionNoWorkingHoursRadioButton"
                                                    cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3" align="center" style="padding-top: 10px">
                                                <asp:datagrid runat="server" id="workingHoursExceptionsWorkingHoursDataGrid" showheader="False"
                                                    showfooter="False" cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False"
                                                    gridlines="None" ondeletecommand="DeleteWorkingHoursException">
                                                    <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" horizontalalign="Left" />
                                                    <columns>
                                                        <asp:templatecolumn runat="server" visible="false">
                                                            <itemtemplate>
                                                                <asp:label runat="server" id="dateLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursExceptionInfo )Container.DataItem ).Date.ToString( )%>' />
                                                                <asp:label runat="server" id="startTimeLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursExceptionInfo )Container.DataItem ).StartTime.ToString( )%>' />
                                                                <asp:label runat="server" id="endTimeLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursExceptionInfo )Container.DataItem ).EndTime.ToString( )%>' />
                                                                <asp:label runat="server" id="allDayLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursExceptionInfo )Container.DataItem ).AllDay.ToString( )%>' />
                                                            </itemtemplate>
                                                        </asp:templatecolumn>
                                                        <asp:templatecolumn runat="server">
                                                            <itemtemplate>
                                                                <asp:label runat="server" text='<%#GetWorkingHoursException( ( DNNSpecialists.Modules.Reservations.WorkingHoursExceptionInfo )Container.DataItem )%>'
                                                                    cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                            </itemtemplate>
                                                        </asp:templatecolumn>
                                                        <asp:templatecolumn runat="server">
                                                            <itemtemplate>
                                                                <asp:imagebutton
                                                                    runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                                                    commandname="Delete" causesvalidation="false" />
                                                            </itemtemplate>
                                                        </asp:templatecolumn>
                                                    </columns>
                                                </asp:datagrid>
                                                <asp:label runat="server" id="workingHoursExceptionsNoWorkingHoursLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal"
                                                    font-italic="true" resourcekey="workingHoursExceptionsNoWorkingHoursLabel" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr runat="server" id="workingHoursExceptionsUpdateResetTableRow">
                                <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:linkbutton runat="server" id="workingHoursExceptionsUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                            </td>
                                            <td style="padding-left: 5px">
                                                <asp:linkbutton runat="server" id="workingHoursExceptionsResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="timeOfDaySectionTableRow">
        <td>
            <dnn:sectionhead id="timeOfDaySectionHead" cssclass="Head" section="timeOfDayTable"
                includerule="true" resourcekey="TimeOfDaySettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="timeOfDayTable" cellpadding="5" cellspacing="0" width="500"
                align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="250">
                        <dnn:label id="displayTimeOfDay" runat="server" controlname="displayTimeOfDayCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="displayTimeOfDayCheckBox" oncheckedchanged="ShowHideTimeOfDayTableRow"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="displayUnavailableTimeOfDayTableRow" visible="false">
                    <td width="250">
                        <dnn:label id="displayUnavailableTimeOfDay" runat="server" controlname="displayUnavailableTimeOfDayCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="displayUnavailableTimeOfDayCheckBox" />
                    </td>
                </tr>
                <tr runat="server" id="timeOfDayTableRow" visible="false">
                    <td colspan="2" style="padding: 0px;">
                        <table cellpadding="5" cellspacing="0">
                            <tr>
                                <td valign="top" nowrap="nowrap" align="right">
                                    <asp:textbox runat="server" id="timeOfDayNameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                                        width="100" />
                                </td>
                                <td valign="top" nowrap="nowrap">
                                    <asp:dropdownlist runat="server" id="timeOfDayStartHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="timeOfDayStartMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="timeOfDayStartAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:label runat="server" cssclass="DNNSpecialists_Modules_Reservations_SubHead" resourcekey="toLabel" />
                                    <asp:dropdownlist runat="server" id="timeOfDayEndHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="timeOfDayEndMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="timeOfDayEndAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:requiredfieldvalidator runat="server" validationgroup="timeOfDayValidationGroup"
                                        controltovalidate="timeOfDayNameTextBox" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal"
                                        display="Dynamic" />
                                    <asp:customvalidator runat="server" validationgroup="timeOfDayValidationGroup" controltovalidate="timeOfDayEndHourDropDownList"
                                        resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" onservervalidate="ValidateTimeOfDay"
                                        display="Dynamic" />
                                </td>
                                <td valign="middle" nowrap="nowrap">
                                    <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                                        onclick="AddTimeOfDayCommandButtonClicked" causesvalidation="true" validationgroup="timeOfDayValidationGroup" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center">
                                    <asp:customvalidator runat="server" controltovalidate="timeOfDayEndHourDropDownList"
                                        resourcekey="TimeOfDayListError" cssclass="DNNSpecialists_Modules_Reservations_Normal" onservervalidate="ValidateTimeOfDayList"
                                        display="Dynamic" />
                                    <asp:datagrid runat="server" id="timeOfDayDataGrid" showheader="False" showfooter="False"
                                        cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                                        ondeletecommand="DeleteTimeOfDay">
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" horizontalalign="Left" />
                                        <columns>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:label runat="server" id="timeOfDayNameLabel" text='<%#DataBinder.Eval( Container.DataItem, "Name" )%>'
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:label runat="server" text='<%#GetTimeOfDay( ( DNNSpecialists.Modules.Reservations.TimeOfDayInfo )Container.DataItem )%>'
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:imagebutton cssclass="CommandButton DNNSpecialists_Modules_Reservations_LinkCommandButton"
                                                        runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                                        commandname="Delete" causesvalidation="false" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <asp:label runat="server" id="noTimeOfDayLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                                        resourcekey="noTimeOfDayLabel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="notificationsSectionTableRow">
        <td>
            <dnn:sectionhead id="notificationsSectionHead" cssclass="Head" section="notificationsTable"
                includerule="true" resourcekey="NotificationsSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="notificationsTable" cellpadding="5" cellspacing="0" width="500"
                align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="bccListCategoryTableRow" visible="false">
                    <td valign="top" colspan="3" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="bccListCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="BCCListCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="bccListLabel" runat="server" controlname="usersDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="usersDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:textbox runat="server" width="100%" id="usernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            visible="false" validationgroup="usernameValidationGroup" />
                        <asp:requiredfieldvalidator runat="server" id="usernameRequiredFieldValidator" controltovalidate="usernameTextBox"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Static" resourcekey="starLabel"
                            validationgroup="usernameValidationGroup" visible="false" />
                    </td>
                    <td valign="mibble">
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddUserCommandButtonClicked" causesvalidation="true" validationgroup="usernameValidationGroup" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <asp:datagrid runat="server" id="bccListDataGrid" showheader="False" showfooter="False"
                            cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                            ondeletecommand="DeleteUser">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="userId" visible="false" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).UserID%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).DisplayName%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton
                                            runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noUsersLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noUsersLabel" />
                    </td>
                </tr>
                <tr runat="server" id="bccListUpdateResetTableRow">
                    <td colspan="3" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="bccListUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="bccListResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="moderationSectionTableRow">
        <td>
            <dnn:sectionhead id="moderationSectionHead" cssclass="Head" section="moderationTable"
                includerule="true" resourcekey="ModerationSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="moderationTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr runat="server" id="moderationCategoryTableRow" visible="false">
                    <td valign="top" colspan="3" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="moderationCategoryDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" onselectedindexchanged="ModerationCategoryDropDownListSelectedIndexChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="moderateLabel" runat="server" controlname="moderateCheckBox" />
                    </td>
                    <td valign="top" colspan="2">
                        <asp:checkbox runat="server" id="moderateCheckBox" oncheckedchanged="ModerateCheckBoxCheckChanged"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="globalModeratorsDropDownListTableRow">
                    <td width="250">
                        <dnn:label id="globalModeratorListLabel" runat="server" controlname="globalModeratorsDropDownList" />
                    </td>
                    <td>
                        <asp:dropdownlist width="100%" runat="server" style="vertical-align: middle" id="moderatorUsersDropDownList"
                            cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:textbox runat="server" width="100%" id="moderatorUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            visible="false" validationgroup="moderatorUsernameValidationGroup" />
                        <asp:requiredfieldvalidator runat="server" id="moderatorUsernameRequiredFieldValidator"
                            controltovalidate="moderatorUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true"
                            display="Static" resourcekey="starLabel" validationgroup="moderatorUsernameValidationGroup"
                            visible="false" />
                    </td>
                    <td width="16">
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddGlobalModeratorCommandButtonClicked" causesvalidation="true" validationgroup="moderatorUsernameValidationGroup" />
                    </td>
                </tr>
                <tr runat="server" id="globalModeratorsDataGridTableRow">
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <asp:datagrid runat="server" id="globalModeratorsDataGrid" showheader="False" showfooter="False"
                            cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                            ondeletecommand="DeleteGlobalModerator">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="userId" visible="false" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).UserID%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).DisplayName%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton
                                            runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noGlobalModeratorsLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noGlobalModeratorsLabel" />
                    </td>
                </tr>
                <tr runat="server" id="moderationHoursTableRow">
                    <td colspan="3" style="padding: 0px">
                        <table cellpadding="5" cellspacing="0" border="0">
                            <tr>
                                <td colspan="3">
                                    <asp:label runat="server" resourcekey="moderationHoursHelp" />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" nowrap="nowrap" rowspan="2">
                                    <asp:dropdownlist runat="server" id="moderationWeekDaysDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                </td>
                                <td valign="top" align="left" nowrap="nowrap">
                                    <asp:label runat="server" resourcekey="fromLabel" cssclass="DNNSpecialists_Modules_Reservations_SubHead" />
                                    <asp:dropdownlist runat="server" id="moderationStartHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="moderationStartMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="moderationStartAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:label runat="server" cssclass="DNNSpecialists_Modules_Reservations_SubHead" resourcekey="toLabel" />
                                    <asp:dropdownlist runat="server" id="moderationEndHourDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="moderationEndMinuteDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:dropdownlist runat="server" id="moderationEndAMPMDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                                    <asp:customvalidator runat="server" validationgroup="moderationHoursValidationGroup"
                                        controltovalidate="moderationEndHourDropDownList" resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal"
                                        onservervalidate="ValidateModerationHours" />
                                </td>
                                <td valign="middle" nowrap="nowrap">
                                    <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                                        onclick="AddModerationHoursCommandButtonClicked" causesvalidation="true" validationgroup="moderationHoursValidationGroup" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3" align="center" style="padding-top: 10px">
                                    <asp:datagrid runat="server" id="moderationHoursDataGrid" showheader="False" showfooter="False"
                                        cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                                        ondeletecommand="DeleteModerationHours">
                                        <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" horizontalalign="Left" />
                                        <columns>
                                            <asp:templatecolumn runat="server" visible="false">
                                                <itemtemplate>
                                                    <asp:label runat="server" id="dayOfWeekLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursInfo )Container.DataItem ).DayOfWeek.ToString( )%>' />
                                                    <asp:label runat="server" id="startTimeLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursInfo )Container.DataItem ).StartTime.ToString( )%>' />
                                                    <asp:label runat="server" id="endTimeLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursInfo )Container.DataItem ).EndTime.ToString( )%>' />
                                                    <asp:label runat="server" id="allDayLabel" visible="false" text='<%#( ( DNNSpecialists.Modules.Reservations.WorkingHoursInfo )Container.DataItem ).AllDay.ToString( )%>' />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:label runat="server" text='<%#GetWorkingHours( ( DNNSpecialists.Modules.Reservations.WorkingHoursInfo )Container.DataItem )%>'
                                                        cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                            <asp:templatecolumn runat="server">
                                                <itemtemplate>
                                                    <asp:imagebutton
                                                        runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                                        commandname="Delete" causesvalidation="false" />
                                                </itemtemplate>
                                            </asp:templatecolumn>
                                        </columns>
                                    </asp:datagrid>
                                    <asp:label runat="server" id="noModerationHoursLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                                        resourcekey="noModerationHoursLabel" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr runat="server" id="moderationUpdateResetTableRow">
                    <td colspan="3" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="moderationUpdateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="moderationResetCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="reportsSectionTableRow">
        <td nowrap="nowrap">
            <dnn:sectionhead id="reportsSectionHead" cssclass="Head" section="reportsTable" includerule="true"
                resourcekey="ReportsSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="reportsTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="250">
                        <dnn:label id="duplicateReservationsListLabel" runat="server" controlname="duplicateReservationsDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="duplicateReservationsUsersDropDownList"
                            cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:textbox runat="server" width="100%" id="duplicateReservationsUsernameTextBox"
                            cssclass="DNNSpecialists_Modules_Reservations_Input" visible="false" validationgroup="duplicateReservationsUsernameValidationGroup" />
                        <asp:requiredfieldvalidator runat="server" id="duplicateReservationsUsernameRequiredFieldValidator"
                            controltovalidate="duplicateReservationsUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true"
                            display="Static" resourcekey="starLabel" validationgroup="duplicateReservationsUsernameValidationGroup"
                            visible="false" />
                    </td>
                    <td valign="middle">
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddDuplicateReservationsCommandButtonClicked" causesvalidation="true"
                            validationgroup="duplicateReservationsUsernameValidationGroup" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <asp:datagrid runat="server" id="duplicateReservationsDataGrid" showheader="False"
                            showfooter="False" cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False"
                            gridlines="None" ondeletecommand="DeleteDuplicateReservations">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="userId" visible="false" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).UserID%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).DisplayName%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton
                                            runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noDuplicateReservationsLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noDuplicateReservationsLabel" />
                    </td>
                </tr>
                <tr>
                    <td width="250">
                        <dnn:label id="viewReservationsListLabel" runat="server" controlname="viewReservationsDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="viewReservationsUsersDropDownList"
                            cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:textbox runat="server" width="100%" id="viewReservationsUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            visible="false" validationgroup="viewReservationsUsernameValidationGroup" />
                        <asp:requiredfieldvalidator runat="server" id="viewReservationsUsernameRequiredFieldValidator"
                            controltovalidate="viewReservationsUsernameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true"
                            display="Static" resourcekey="starLabel" validationgroup="viewReservationsUsernameValidationGroup"
                            visible="false" />
                    </td>
                    <td valign="middle">
                        <asp:imagebutton runat="server" resourcekey="addCommandButton" imageurl="~/images/add.gif"
                            onclick="AddViewReservationsCommandButtonClicked" causesvalidation="true" validationgroup="viewReservationsUsernameValidationGroup" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp
                    </td>
                    <td>
                        <asp:datagrid runat="server" id="viewReservationsDataGrid" showheader="False" showfooter="False"
                            cellpadding="0" cellspacing="0" borderwidth="0" autogeneratecolumns="False" gridlines="None"
                            ondeletecommand="DeleteViewReservations">
                            <itemstyle cssclass="DNNSpecialists_Modules_Reservations_Settings_DataGrid_ItemStyle" />
                            <columns>
                                <asp:templatecolumn runat="server" visible="false">
                                    <itemtemplate>
                                        <asp:label runat="server" id="userId" visible="false" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).UserID%>' />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:label runat="server" text='<%#( ( DotNetNuke.Entities.Users.UserInfo )Container.DataItem ).DisplayName%>'
                                            cssclass="DNNSpecialists_Modules_Reservations_Normal" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                                <asp:templatecolumn runat="server">
                                    <itemtemplate>
                                        <asp:imagebutton
                                            runat="server" imageurl="~/images/delete.gif" resourcekey="deleteCommandButton"
                                            commandname="Delete" causesvalidation="false" />
                                    </itemtemplate>
                                </asp:templatecolumn>
                            </columns>
                        </asp:datagrid>
                        <asp:label runat="server" id="noViewReservationsLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" font-italic="true"
                            resourcekey="noViewReservationsLabel" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="remindersSectionTableRow">
        <td>
            <dnn:sectionhead id="remindersSectionHead" cssclass="Head" section="remindersTable"
                includerule="true" resourcekey="RemindersSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="remindersTable" cellpadding="5" cellspacing="0" border="0"
                width="500" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="250">
                        <dnn:label id="sendRemindersLabel" runat="server" controlname="sendRemindersCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="sendRemindersCheckBox" oncheckedchanged="ShowHideSendRemindersWhenTableRow"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="sendRemindersViaTableRow" visible="false">
                    <td width="250">
                        <dnn:label id="sendRemindersViaLabel" runat="server" controlname="sendRemindersViaDropDownList" />
                    </td>
                    <td valign="top">
                        <asp:dropdownlist width="100%" runat="server" id="sendRemindersViaDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                    </td>
                </tr>
                <tr runat="server" id="sendRemindersWhenTableRow" visible="false">
                    <td width="250">
                        <dnn:label id="sendRemindersWhenLabel" runat="server" controlname="sendRemindersWhenTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="sendRemindersWhenTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="sendRemindersWhenDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:rangevalidator runat="server" controltovalidate="sendRemindersWhenTextBox" resourcekey="starLabel"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" minimumvalue="1"
                            maximumvalue="9999" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="sendRemindersWhenTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="requireConfirmationTableRow" visible="false">
                    <td width="250">
                        <dnn:label id="requireConfirmationLabel" runat="server" controlname="requireConfirmationCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="requireConfirmationCheckBox" oncheckedchanged="ShowHideRequireConfirmationWhenTableRow"
                            autopostback="true" />
                    </td>
                </tr>
                <tr runat="server" id="requireConfirmationWhenTableRow" visible="false">
                    <td width="250">
                        <dnn:label id="requireConfirmationWhenLabel" runat="server" controlname="requireConfirmationWhenTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="requireConfirmationWhenTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="50" />
                        <asp:dropdownlist runat="server" id="requireConfirmationWhenDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input" />
                        <asp:rangevalidator runat="server" controltovalidate="requireConfirmationWhenTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic"
                            minimumvalue="1" maximumvalue="9999" type="Integer" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="requireConfirmationWhenTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr runat="server" id="requireConfirmationTableRow2" visible="false">
                    <td class="DNNSpecialists_Modules_Reservations_Normal" colspan="2">
                        <asp:label runat="server" resourcekey="requireConfirmationInstructionsLabel" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="mailTemplatesSectionTableRow">
        <td>
            <dnn:sectionhead id="mailTemplatesSectionHead" cssclass="Head" section="mailTemplatesTable"
                includerule="true" resourcekey="mailTemplatesSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="mailTemplatesTable" cellpadding="5" cellspacing="0" border="0"
                width="600" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="100">
                        <dnn:label id="mailFromLabel" runat="server" controlname="mailFromTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="mailFromTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dnn:label id="attachICalendarLabel" runat="server" controlname="attachICalendarCheckBox" />
                    </td>
                    <td valign="top">
                        <asp:checkbox runat="server" id="attachICalendarCheckBox" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dnn:label id="iCalendarAttachmentFileNameLabel" runat="server" controlname="iCalendarAttachmentFileNameTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="iCalendarAttachmentFileNameTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                        <asp:requiredfieldvalidator runat="server" controltovalidate="iCalendarAttachmentFileNameTextBox"
                            resourcekey="starLabel" cssclass="DNNSpecialists_Modules_Reservations_Normal" enableclientscript="true" display="Dynamic" />
                    </td>
                </tr>
                <tr>
                    <td class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <dnn:label id="mailTemplateLabel" runat="server" controlname="mailTemplateDropDownList" />
                    </td>
                    <td class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="mailTemplateDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" autopostback="true" onselectedindexchanged="MailTemplateDropDownListSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dnn:label id="mailTemplateSubjectLabel" runat="server" controlname="mailTemplateSubjectTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="mailTemplateSubjectTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" maxlength="2000" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <dnn:label id="mailTemplateBodyLabel" runat="server" controlname="mailTemplateBodyTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="mailTemplateBodyTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            textmode="MultiLine" rows="20" width="100%" maxlength="2000" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dnn:label id="bodyTypeLabel" runat="server" controlname="mailTemplateBodyTypeHtmlRadioButton" />
                    </td>
                    <td>
                        <asp:radiobutton runat="server" id="mailTemplateBodyTypeHtmlRadioButton" groupname="mailTemplateBodyTypeRadioButtonGroup"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" resourcekey="mailTemplateBodyTypeHtmlRadioButton" />
                        <asp:radiobutton runat="server" id="mailTemplateBodyTypeTextRadioButton" groupname="mailTemplateBodyTypeRadioButtonGroup"
                            cssclass="DNNSpecialists_Modules_Reservations_Normal" resourcekey="mailTemplateBodyTypeTextRadioButton" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="updateMailTemplateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="resetMailTemplateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="smsTemplatesSectionTableRow">
        <td>
            <dnn:sectionhead id="smsTemplatesSectionHead" cssclass="Head" section="smsTemplatesTable"
                includerule="true" resourcekey="smsTemplatesSettings" isexpanded="false" runat="server"></dnn:sectionhead>
            <table runat="server" id="smsTemplatesTable" cellpadding="5" cellspacing="0" border="0"
                width="600" align="center" class="DNNSpecialists_Modules_Reservations_SettingsTable">
                <tr>
                    <td width="150">
                        <dnn:label id="twilioAccountSIDLabel" runat="server" controlname="twilioAccountSIDTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="twilioAccountSIDTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                </tr>
                <tr>
                    <td width="150">
                        <dnn:label id="twilioAuthTokenLabel" runat="server" controlname="twilioAuthTokenTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="twilioAuthTokenTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                </tr>
                <tr>
                    <td width="150">
                        <dnn:label id="twilioFromLabel" runat="server" controlname="twilioFromTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="twilioFromTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" />
                    </td>
                </tr>
                <tr>
                    <td class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <dnn:label id="smsTemplateLabel" runat="server" controlname="smsTemplateDropDownList" />
                    </td>
                    <td class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <asp:dropdownlist runat="server" id="smsTemplateDropDownList" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            width="100%" autopostback="true" onselectedindexchanged="SMSTemplateDropDownListSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <dnn:label id="smsTemplateBodyLabel" runat="server" controlname="smsTemplateBodyTextBox" />
                    </td>
                    <td valign="top">
                        <asp:textbox runat="server" id="smsTemplateBodyTextBox" cssclass="DNNSpecialists_Modules_Reservations_Input"
                            textmode="MultiLine" rows="20" width="100%" maxlength="2000" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" class="DNNSpecialists_Modules_Reservations_CategoryTableCell">
                        <table cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <asp:linkbutton runat="server" id="updateSMSTemplateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" causesvalidation="false"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                                </td>
                                <td style="padding-left: 5px">
                                    <asp:linkbutton runat="server" id="resetSMSTemplateCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton" causesvalidation="false"><asp:image runat="server" imageurl="~/images/reset.gif" /><asp:label runat="server" resourcekey="resetCommandButton" /></asp:linkbutton>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr runat="server" id="updateCancelTableRow" visible="false">
        <td align="center">
            <table cellpadding="0" cellspacing="0">
                <tr>
                    <td>
                        <asp:linkbutton runat="server" id="updateSettingsCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/save.gif" /><asp:label runat="server" resourcekey="updateCommandButton" /></asp:linkbutton>
                    </td>
                    <td style="padding-left: 5px">
                        <asp:linkbutton runat="server" id="cancelSettingsCommandButton" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"><asp:image runat="server" imageurl="~/images/delete.gif" /><asp:label runat="server" resourcekey="cancelCommandButton" /></asp:linkbutton>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
