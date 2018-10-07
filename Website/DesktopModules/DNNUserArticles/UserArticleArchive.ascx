<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserArticleArchive.ascx.cs" Inherits="ZLDNN.Modules.DNNUserArticles.UserArticleArchive" %>
<table>
    <tr>
        <td><asp:label id="lblArchive" runat="server" cssclass="SubHead" ResourceKey="lblArchive">Archive</asp:label>
        </td>
    </tr>
    <tr>
        <td><asp:calendar 
                id="calMonth" 
                runat="server" 
                cssclass="Normal"
                DayHeaderStyle-CssClass="DNNArticle_Archive_DayHeader"
                DayStyle-CssClass="DNNArticle_Archive_Day"
                NextPrevStyle-CssClass="DNNArticle_Archive_NextPrev"
                OtherMonthDayStyle-CssClass="DNNArticle_Archive_OtherMonth"
                SelectedDayStyle-CssClass="DNNArticle_Archive_SelectedDay"
                SelectorStyle-CssClass="DNNArticle_Archive_Selector"
                TitleStyle-CssClass="DNNArticle_Archive_Title"
                TodayDayStyle-CssClass="DNNArticle_Archive_TodayDay"
                WeekendDayStyle-CssClass="DNNArticle_Archive_WeekendDay"
                 OnSelectionChanged="calMonth_SelectionChanged"
                 OnVisibleMonthChanged="calMonth_VisibleMonthChanged">
                <TodayDayStyle CssClass="DNNArticle_Archive_TodayDay">
                </TodayDayStyle>

                <SelectorStyle CssClass="DNNArticle_Archive_Selector">
                </SelectorStyle>

                <DayStyle CssClass="DNNArticle_Archive_Day">
                </DayStyle>

                <NextPrevStyle CssClass="DNNArticle_Archive_NextPrev">
                </NextPrevStyle>

                <DayHeaderStyle CssClass="DNNArticle_Archive_DayHeader">
                </DayHeaderStyle>

                <SelectedDayStyle CssClass="DNNArticle_Archive_SelectedDay">
                </SelectedDayStyle>

                <TitleStyle CssClass="DNNArticle_Archive_Title">
                </TitleStyle>

                <WeekendDayStyle CssClass="DNNArticle_Archive_WeekendDay">
                </WeekendDayStyle>

                <OtherMonthDayStyle CssClass="DNNArticle_Archive_OtherMonth">
                </OtherMonthDayStyle></asp:calendar>
        </td>
    </tr>
    <tr>
        <td><asp:label id="lblMonthly" runat="server" cssclass="SubHead" ResourceKey="lblMonthly">Monthly</asp:label>
        </td>
    </tr>
    <tr>
        <td><asp:datalist id="lst" runat="server" Width="100%" CssClass="CommandButton" ItemStyle-HorizontalAlign="left" CellPadding="0" OnItemDataBound="lst_ItemDataBound" CellSpacing="0" BorderWidth="0">
                <itemtemplate>
                    <asp:hyperlink runat="server" id="lnkMonthYear" cssclass="CommandButton"></asp:hyperlink>
                </itemtemplate>
            </asp:datalist>
        </td>
    </tr>
</table>