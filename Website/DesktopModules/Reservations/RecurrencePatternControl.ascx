<%@ control language="C#" autoeventwireup="true" codebehind="RecurrencePatternControl.ascx.cs"
    inherits="DNNSpecialists.Modules.Reservations.RecurrencePatternControl" %>
<%@ register tagprefix="dnn" tagname="Label" src="~/controls/LabelControl.ascx" %>
<asp:validationsummary runat="server" id="validationSummary" cssclass="Normal" style="text-align: left; color: red; display: block; margin: 0 0 10px 0" enableclientscript="false" showmessagebox="false" showsummary="true" displaymode="BulletList"/>
<table cellpadding="0" cellspacing="3" border="0">
    <tr>
        <td rowspan="3">
            <asp:label runat="server" cssclass="SubHead" resourcekey="timeLabel" />
        </td>
        <td>
            <table cellpadding="0" cellspacing="3" border="0">
                <tr>
                    <td>
                        <asp:radiobutton runat="server" enabled="false" style="visibility: hidden" />
                    </td>
                    <td width="60">
                        <asp:label runat="server" cssclass="Normal" id="recurrenceStartTimeLabel" resourcekey="recurrenceStartTimeLabel" />
                    </td>
                    <td>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceStartTimeHour"
                            style="text-align: center;" maxlength="2"  /><span class="Normal"> :
                        </span>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceStartTimeMinutes"
                            style="text-align: center;" maxlength="2" />
                        <asp:radiobutton runat="server" id="recurrenceStartTimeAM" resourcekey="AM" groupname="startTime"
                            cssclass="Normal" checked="True" />
                        <asp:radiobutton runat="server" id="recurrenceStartTimePM" resourcekey="PM" groupname="startTime"
                            cssclass="Normal" />
                        <asp:placeholder runat="server" id="recurrenceStartTimeValidatorsPlaceHolder">
                            <asp:requiredfieldvalidator runat="server" controltovalidate="recurrenceStartTimeHour"
                                enableclientscript="false" display="None" resourcekey="recurrenceStartTimeHourValidator" />
                            <asp:requiredfieldvalidator runat="server" controltovalidate="recurrenceStartTimeMinutes"
                                enableclientscript="false" display="None" resourcekey="recurrenceStartTimeMinutesValidator" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceStartTimeHour" onservervalidate="ValidateHour"
                                display="None" resourcekey="recurrenceStartTimeHourValidator2" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceStartTimeMinutes"
                                onservervalidate="ValidateMinutes" display="None" resourcekey="recurrenceStartTimeMinutesValidator2" />
                        </asp:placeholder>
                    </td>
                    <td style="padding-left: 10px">
                        <asp:checkbox runat="server" id="recurrenceAllDayEvent" cssclass="Normal" resourcekey="recurrenceAllDayEvent"
                            oncheckedchanged="RecurrenceAllDayEventChanged" autopostback="true" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table cellpadding="0" cellspacing="3" border="0">
                <tr>
                    <td>
                        <asp:radiobutton runat="server" id="recurrenceEndTimeRadioButton" groupname="recurrenceStartAndEndTimeGroup"
                            checked="true" oncheckedchanged="RecurrenceStartAndEndTimeGroupCheckedChanged"
                            autopostback="true" />
                    </td>
                    <td width="60">
                        <label runat="server" id="recurrenceEndTimeLabel" for="<%#recurrenceEndTimeRadioButton.ClientID%>"
                            class="Normal"><asp:label runat="server" resourcekey="recurrenceEndTimeLabel" /></label>
                    </td>
                    <td>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceEndTimeHour"
                            style="text-align: center;" maxlength="2" /><span class="Normal"> :
                        </span>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceEndTimeMinutes"
                            style="text-align: center;" maxlength="2" />
                        <asp:radiobutton runat="server" id="recurrenceEndTimeAM" resourcekey="AM" groupname="endTime"
                            cssclass="Normal" checked="True" />
                        <asp:radiobutton runat="server" id="recurrenceEndTimePM" resourcekey="PM" groupname="endTime"
                            cssclass="Normal" />
                        <asp:placeholder runat="server" id="recurrenceEndTimeValidatorsPlaceHolder">
                            <asp:requiredfieldvalidator runat="server" controltovalidate="recurrenceEndTimeHour"
                                enableclientscript="false" display="None" resourcekey="recurrenceEndTimeHourValidator" />
                            <asp:requiredfieldvalidator runat="server" controltovalidate="recurrenceEndTimeMinutes"
                                enableclientscript="false" display="None" resourcekey="recurrenceEndTimeMinutesValidator" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceEndTimeHour" onservervalidate="ValidateHour"
                                display="None" resourcekey="recurrenceEndTimeHourValidator2" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceEndTimeMinutes"
                                onservervalidate="ValidateMinutes" display="None" resourcekey="recurrenceEndTimeMinutesValidator2" />
                        </asp:placeholder>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table cellpadding="0" cellspacing="3" border="0">
                <tr>
                    <td>
                        <asp:radiobutton runat="server" id="recurrenceDurationRadioButton" groupname="recurrenceStartAndEndTimeGroup"
                            oncheckedchanged="RecurrenceStartAndEndTimeGroupCheckedChanged" autopostback="true" />
                    </td>
                    <td width="60">
                        <label runat="server" id="recurrenceDurationLabel" for="<%#recurrenceDurationRadioButton.ClientID%>"
                            class="Normal"><asp:label runat="server" resourcekey="recurrenceDurationLabel" /></label>
                    </td>
                    <td>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceDurationDays"
                            style="text-align: center;" maxlength="2" /><span class="Normal"> :
                        </span>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceDurationHours"
                            style="text-align: center;" maxlength="2" /><span class="Normal"> :
                        </span>
                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" width="35" id="recurrenceDurationMinutes"
                            style="text-align: center;" maxlength="2" /><span class="Normal" runat="server" id="ddhhmmLabel"
                                style="font-style: italic"> <asp:label runat="server" resourcekey="ddhhmmLabel" /></span>
                        <asp:placeholder runat="server" id="recurrenceDurationValidatorsPlaceHolder">
                            <asp:customvalidator runat="server" controltovalidate="recurrenceDurationDays" onservervalidate="ValidateInt32"
                                display="None" resourcekey="recurrenceDurationDaysValidator" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceDurationHours" onservervalidate="ValidateInt32"
                                display="None" resourcekey="recurrenceDurationHoursValidator" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceDurationMinutes"
                                onservervalidate="ValidateInt32" display="None" resourcekey="recurrenceDurationMinutesValidator" />
                            <asp:customvalidator runat="server" controltovalidate="recurrenceDurationMinutes"
                                onservervalidate="ValidateRecurrenceDuration" validateemptytext="true" display="None"
                                resourcekey="recurrenceDurationMinutesValidator2" />
                        </asp:placeholder>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td></td>
        <td><hr /></td>
    </tr>
    <tr>
        <td style="padding-right: 10px;">
            <asp:label runat="server" cssclass="SubHead" resourcekey="patternLabel" />
        </td>
        <td>
            <table cellpadding="0" cellspacing="3" border="0">
                <tr>
                    <td style="padding-right: 10px;" nowrap="nowrap">
                        <asp:radiobutton runat="server" id="daily" groupname="patternRadioButtons" resourcekey="Daily"
                            cssclass="Normal" autopostback="true" oncheckedchanged="RecurrencePatternChanged" />
                    </td>
                    <td rowspan="4" style="padding-left: 10px; border-left: solid 1px #5c6f93" valign="top">
                        <asp:placeholder runat="server" id="dailyPatternPlaceHolder" visible="false">
                            <table cellpadding="0" cellspacing="0" border="0" width="400">
                                <tr>
                                    <td>
                                        <asp:radiobutton runat="server" id="dailyEveryDayRadioButton" groupname="dailyRatternRadioButtons" resourcekey="Every" cssclass="Normal" checked="true" />&nbsp;<asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="dailyEveryDayTextBox" text="1" width="30px" />&nbsp;<asp:label runat="server" cssclass="Normal" resourcekey="daysLabel" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="dailyEveryDayTextBox"
                                            enableclientscript="false" display="None" resourcekey="dailyEveryDayTextBoxValidator" />
                                        <asp:customvalidator runat="server" controltovalidate="dailyEveryDayTextBox" onservervalidate="ValidateInt32"
                                            display="None" resourcekey="dailyEveryDayTextBoxValidator2" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 7px">
                                        <asp:radiobutton runat="server" id="dailyEveryWeekDayRadioButton" groupname="dailyRatternRadioButtons"
                                            resourcekey="EveryWeekday" cssclass="Normal" />
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                        <asp:placeholder runat="server" id="weeklyPatternPlaceHolder" visible="true">
                            <table cellpadding="0" cellspacing="0" border="0" width="400">
                                <tr>
                                    <td>
                                        <asp:label runat="server" resourcekey="Every" cssclass="Normal" />&nbsp; <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="weeklyEveryWeekTextBox" text="1" width="30px" />&nbsp;<asp:label runat="server" cssclass="Normal" resourcekey="weeksLabel" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="weeklyEveryWeekTextBox"
                                            enableclientscript="false" display="None" resourcekey="weeklyEveryWeekTextBoxValidator" />
                                        <asp:customvalidator runat="server" controltovalidate="weeklyEveryWeekTextBox" onservervalidate="ValidateInt32"
                                            display="None" resourcekey="weeklyEveryWeekTextBoxValidator2" />
                                        <asp:customvalidator runat="server" controltovalidate="weeklyEveryWeekTextBox" onservervalidate="ValidateWeeklyDay"
                                            display="None" resourcekey="weeklyEveryWeekTextBoxValidator3" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 7px">
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td>
                                                    <asp:checkbox runat="server" id="weeklySundayCheckBox" cssclass="Normal" />
                                                </td>
                                                <td>
                                                    <asp:checkbox runat="server" id="weeklyMondayCheckBox" cssclass="Normal" />
                                                </td>
                                                <td>
                                                    <asp:checkbox runat="server" id="weeklyTuesdayCheckBox" cssclass="Normal" />
                                                </td>
                                                <td>
                                                    <asp:checkbox runat="server" id="weeklyWednesdayCheckBox" cssclass="Normal" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 7px">
                                                    <asp:checkbox runat="server" id="weeklyThursdayCheckBox" cssclass="Normal" />
                                                </td>
                                                <td style="padding-top: 7px">
                                                    <asp:checkbox runat="server" id="weeklyFridayCheckBox" cssclass="Normal" />
                                                </td>
                                                <td style="padding-top: 7px">
                                                    <asp:checkbox runat="server" id="weeklySaturdayCheckBox" cssclass="Normal" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                        <asp:placeholder runat="server" id="monthlyPatternPlaceHolder" visible="false">
                            <table cellpadding="0" cellspacing="0" border="0" width="400">
                                <tr>
                                    <td width="65" nowrap="nowrap">
                                        <asp:radiobutton runat="server" id="monthlyDayXofEveryYMonth" groupname="monthlyPatternRadioButtons"
                                            resourcekey="dayRadioButton" cssclass="Normal" checked="true" />
                                    </td>
                                    <td nowrap="nowrap">
                                        <span class="Normal">
                                            <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="monthlyDayTextBox" text="1" width="30px" />&nbsp;<asp:label runat="server" resourcekey="ofEvery" />&nbsp;<asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="monthlyMonthTextBox" text="1" width="30px" />&nbsp;<asp:label runat="server" resourcekey="months" />
                                        </span>
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="monthlyDayTextBox"
                                            enableclientscript="false" display="None" resourcekey="monthlyDayTextBoxValidator" />
                                        <asp:customvalidator runat="server" controltovalidate="monthlyDayTextBox" onservervalidate="ValidateInt32"
                                            display="None" resourcekey="monthlyDayTextBoxValidator2" />
                                        <asp:requiredfieldvalidator runat="server" controltovalidate="monthlyMonthTextBox"
                                            enableclientscript="false" display="None" resourcekey="monthlyMonthTextBoxValidator" />
                                        <asp:customvalidator runat="server" controltovalidate="monthlyMonthTextBox" onservervalidate="ValidateInt32"
                                            display="None" resourcekey="monthlyMonthTextBoxValidator2" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 7px" nowrap="nowrap">
                                        <asp:radiobutton runat="server" id="monthlyXYofEveryZ" groupname="monthlyPatternRadioButtons"
                                            resourcekey="The" cssclass="Normal" />
                                    </td>
                                    <td style="padding-top: 7px" nowrap="nowrap">
                                        <span class="Normal">
                                            <asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="monthlyDayPositionDropDownList" />&nbsp;<asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="monthlyDayTypeDropDownList" />&nbsp;<asp:label runat="server" resourcekey="ofEvery" />&nbsp;<asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="monthlyMonthTextBox2" text="1" width="30px" />&nbsp;<asp:label runat="server" resourcekey="months" />
                                            <asp:requiredfieldvalidator runat="server" controltovalidate="monthlyMonthTextBox2"
                                                enableclientscript="false" display="None" resourcekey="monthlyMonthTextBox2Validator" />
                                            <asp:customvalidator runat="server" controltovalidate="monthlyMonthTextBox2" onservervalidate="ValidateInt32"
                                                display="None" resourcekey="monthlyMonthTextBox2Validator2" />
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                        <asp:placeholder runat="server" id="yearlyPatternPlaceHolder" visible="false">
                            <table cellpadding="0" cellspacing="0" border="0" width="400">
                                <tr>
                                    <td width="65" nowrap="nowrap">
                                        <span class="Normal">
                                            <asp:radiobutton runat="server" id="yearlyMonthDay" groupname="yearlyPatternRadioButtons"
                                                resourcekey="Every" cssclass="Normal" checked="true" />
                                        </span>
                                    </td>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" border="0">
                                            <tr>
                                                <td>
                                                    <asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="yearlyMonthDropDownList" />
                                                </td>
                                                <td style="padding-left: 5px;">
                                                    <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="yearlyDayTextBox" text="1"
                                                        width="30px" />
                                                    <asp:requiredfieldvalidator runat="server" controltovalidate="yearlyDayTextBox" enableclientscript="false"
                                                        display="None" resourcekey="yearlyDayTextBoxValidator" />
                                                    <asp:customvalidator runat="server" controltovalidate="yearlyDayTextBox" onservervalidate="ValidateInt32"
                                                        display="None" resourcekey="yearlyDayTextBoxValidator2" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="padding-top: 7px;" nowrap="nowrap">
                                        <asp:radiobutton runat="server" id="yearlyXYofZ" groupname="yearlyPatternRadioButtons"
                                            resourcekey="The" cssclass="Normal" />
                                    </td>
                                    <td style="padding-top: 7px;" nowrap="nowrap">
                                        <span class="Normal">
                                            <asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="yearlyDayPositionDropDownList" />
                                            &nbsp;<asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="yearlyDayTypeDropDownList" />
                                            &nbsp;of&nbsp;<asp:dropdownlist cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="yearlyMonthDropDownList2" />
                                        </span>
                                    </td>
                                </tr>
                            </table>
                        </asp:placeholder>
                    </td>
                </tr>
                <tr>
                    <td style="padding-right: 10px; padding-top: 7px" nowrap="nowrap">
                        <asp:radiobutton runat="server" id="weekly" groupname="patternRadioButtons" resourcekey="Weekly"
                            checked="true" cssclass="Normal" autopostback="true" oncheckedchanged="RecurrencePatternChanged" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-right: 10px; padding-top: 7px" nowrap="nowrap">
                        <asp:radiobutton runat="server" id="monthly" groupname="patternRadioButtons" resourcekey="Monthly"
                            cssclass="Normal" autopostback="true" oncheckedchanged="RecurrencePatternChanged" />
                    </td>
                </tr>
                <tr>
                    <td style="padding-right: 10px; padding-top: 7px" nowrap="nowrap">
                        <asp:radiobutton runat="server" id="yearly" groupname="patternRadioButtons" resourcekey="Yearly"
                            cssclass="Normal" autopostback="true" oncheckedchanged="RecurrencePatternChanged" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td></td>
        <td><hr /></td>
    </tr>
    <tr>
        <td>
            <asp:label runat="server" cssclass="SubHead" resourcekey="Range" />
        </td>
        <td>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td rowspan="3" nowrap="nowrap">
                        <asp:label runat="server" cssclass="Normal" resourcekey="StartOn" />
                    </td>
                    <td rowspan="3">
                        <table cellpadding="0" cellspacing="3" border="0">
                            <tr>
                                <td>
                                    <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="recurrenceStartDate" style="text-align: center" width="100" />
                                    <asp:requiredfieldvalidator runat="server" controltovalidate="recurrenceStartDate"
                                        enableclientscript="false" display="None" resourcekey="recurrenceStartDateValidator" />
                                    <asp:customvalidator runat="server" controltovalidate="recurrenceStartDate" onservervalidate="ValidateDateTime"
                                        display="None" resourcekey="recurrenceStartDateValidator2" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td rowspan="2">
                        <table cellpadding="0" cellspacing="3" border="0">
                            <tr>
                                <td nowrap="nowrap">
                                    <asp:radiobutton runat="server" id="recurrenceNoEndDateRadioButton" groupname="recurrenceEndDateRadioButtons"
                                        resourcekey="NoEndDate" checked="true" cssclass="Normal" />
                                </td>
                                <td>
                                    <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" enabled="false" style="visibility:hidden;"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:radiobutton runat="server" id="recurrenceEndAfterRadioButton" groupname="recurrenceEndDateRadioButtons"
                                        resourcekey="EndAfter" cssclass="Normal" />
                                </td>
                                <td>
                                    <span class="Normal">
                                        <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="recurrenceEndAfterTextBox"
                                            text="10" width="30px" />&nbsp;<asp:label runat="server" cssclass="Normal" resourcekey="occurrences" />
                                        <asp:customvalidator runat="server" controltovalidate="recurrenceEndAfterTextBox"
                                            onservervalidate="ValidateInt32" display="None" resourcekey="recurrenceEndAfterTextBoxValidator" />
                                        <asp:customvalidator runat="server" controltovalidate="recurrenceStartDate" onservervalidate="ValidateRecurrenceEndAfterRadioButton"
                                            display="None" resourcekey="recurrenceEndAfterTextBoxValidator2" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:radiobutton runat="server" id="recurrenceEndDateRadioButton" groupname="recurrenceEndDateRadioButtons"
                                        resourcekey="EndBy" cssclass="Normal" />
                                </td>
                                <td>
                                    <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td width="155">
                                                <asp:textbox cssclass="DNNSpecialists_Modules_Reservations_Input" runat="server" id="recurrenceEndDate" style="text-align: center" width="100" />
                                                <asp:customvalidator runat="server" controltovalidate="recurrenceStartDate" onservervalidate="ValidateRecurrenceEndTimeNotEmpty"
                                                    display="None" resourcekey="recurrenceEndDateValidator" />
                                                <asp:customvalidator runat="server" controltovalidate="recurrenceEndDate" onservervalidate="ValidateDateTime"
                                                    display="None" resourcekey="recurrenceEndDateValidator2" />
                                                <asp:customvalidator runat="server" controltovalidate="recurrenceEndDate" onservervalidate="ValidateRecurrenceEndDate"
                                                    display="None" resourcekey="recurrenceEndDateValidator3" />
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
    <tr>
        <td></td>
        <td><hr /></td>
    </tr>
    <tr>
        <td colspan="2" align="center">
            <asp:linkbutton runat="server" cssclass="DNNSpecialists_Modules_Reservations_CommandButton"
                causesvalidation="true" onclick="SubmitClicked"><asp:image runat="server" id="submitCommandButtonImage" /><asp:label runat="server" id="submitCommandButtonLabel" /></asp:linkbutton>
        </td>
    </tr>
</table>
