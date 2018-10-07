<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssSettings.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.RssSettings" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>



<dnn:SectionHead ID="dshRssSettings" runat="server" IncludeRule="True" Section="tblRSSSettings"
                  IsExpanded="false"></dnn:SectionHead>
<table id="tblRSSSettings" width="100%" runat="server" >
    <tr>
        <td style="width: 25px">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
        <td>
            <dnn:SectionHead ID="dshChannelSettings" runat="server" IncludeRule="True" Section="tblChannelSettings"
                              IsExpanded="true"></dnn:SectionHead>
            <table id="tblChannelSettings" runat="server" cellspacing="3" cellpadding="3" border="0"
                   summary="DNNArticle Settings Design Table">
                <tr>
                    <td style="width: 25px">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <table cellspacing="3" cellpadding="3" border="0" summary="DNNArticle Settings Design Table" class="dnnFormItem">
                            <tr>
                                <td>
                                    <dnn:label ID="lbShowArticleNumber" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtShowArticleNumber" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                    <asp:RangeValidator ID="RangeValidator1" ControlToValidate="txtShowArticleNumber"  MaximumValue="1000" MinimumValue="1" Type="Integer" runat="server" ErrorMessage="(1-1000)"></asp:RangeValidator>
                                </td>
                            </tr>
                            <tr>
                                <td  >
                                    <dnn:label ID="lbChannelTitle" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtChannelTitle" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbChannelDescription" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtChannelDescription" TextMode="MultiLine" Rows="5" Width="300px"   CssClass="NormalTextBox"
                                                 runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbChannelLink" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtChannelLink" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbChannelCopyright" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtChannelCopyright" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbChannelWebMaster" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtlbChannelWebMaster" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="width: 25px">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </td>
        <td>
            <dnn:SectionHead ID="dshItemSettings" runat="server" IncludeRule="True" Section="tblItemSettings"
                              IsExpanded="true"></dnn:SectionHead>
            <table id="tblItemSettings" runat="server" cellspacing="3" cellpadding="3" border="0"
                   summary="DNNArticle Settings Design Table"  >
                <tr>
                    <td style="width: 25px">
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    <td>
                        <table cellspacing="3" cellpadding="3" border="0" summary="DNNArticle Settings Design Table" class="dnnFormItem">
                            <tr>
                                <td >
                                    <dnn:label ID="lbItemTitle" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtItemTitle" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbItemDescription" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtItemDescription" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <dnn:label ID="lbItemCreator" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtItemCreator" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                            
                            <tr>
                                <td >
                                    <dnn:label ID="lblPublishDate" runat="server" ></dnn:label>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPublishDate" Width="300px" runat="server"  CssClass="NormalTextBox"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>