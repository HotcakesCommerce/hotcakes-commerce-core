<%@ Control CodeBehind="RollingSet.ascx.cs" Language="C#" AutoEventWireup="true" Inherits=" ZLDNN.Modules.DNNArticle_List.RollingSet" %>

<asp:Label ID="lbTitle" runat="server"  resourcekey="lbTitle">Scroll Setting</asp:Label>

<table id="Table1" cellspacing="1" cellpadding="1" width="367" 
       border="0" class="dnnFormItem">
    <tr>
        <td >
            <asp:Label ID="lbEnablerolling" resourcekey="lbEnablerolling" runat="server" >Enable rolling</asp:Label></td>
        <td>
            <asp:CheckBox ID="ckEnabled" runat="server" Checked="True"></asp:CheckBox></td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lbHeight" resourcekey="lbHeight" runat="server" >Height</asp:Label></td>
        <td>
            <asp:TextBox ID="txtHeight" runat="server"  CssClass="NormalTextBox">100</asp:TextBox>px<asp:RangeValidator
                                                                                                        ID="RangeValidator2" runat="server" MaximumValue="1000" MinimumValue="1" Type="Integer"
                                                                                                        ControlToValidate="txtHeight" ErrorMessage="(1-1000)"></asp:RangeValidator></td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lbWidth" runat="server" resourcekey="lbWidth" >Width</asp:Label></td>
        <td>
            <asp:TextBox ID="txtWidth" runat="server"  CssClass="NormalTextBox"></asp:TextBox>px<asp:RangeValidator ID="RangeValidator1"
                                                                                                                    runat="server" MaximumValue="1000" MinimumValue="1" Type="Integer" ControlToValidate="txtWidth"
                                                                                                                    ErrorMessage="(1-1000)"></asp:RangeValidator></td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lbDirection" runat="server" resourcekey="lbDirection" >Direction</asp:Label></td>
        <td>
            <asp:DropDownList ID="dlDirection" runat="server" Width="125px"  CssClass="Normal">
                <asp:ListItem Value="up">Up</asp:ListItem>
                <asp:ListItem Value="down">Down</asp:ListItem>
                <asp:ListItem Value="left">Left</asp:ListItem>
                <asp:ListItem Value="right">Right</asp:ListItem>
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lbDelay" runat="server" resourcekey="lbDelay" >Delay</asp:Label></td>
        <td>
            <asp:TextBox ID="txtDelay" runat="server"  CssClass="NormalTextBox">50</asp:TextBox>ms<asp:RangeValidator ID="RangeValidator4"
                                                                                                                      runat="server" MaximumValue="10000" MinimumValue="1" Type="Integer" ControlToValidate="txtDelay"
                                                                                                                      ErrorMessage="(1-10000)"></asp:RangeValidator></td>
    </tr>
    <tr>
        <td >
            <asp:Label ID="lbScrollamount" runat="server" resourcekey="lbScrollamount" >Scrollamount</asp:Label></td>
        <td>
            <asp:TextBox ID="txtScrollamount" runat="server"  CssClass="NormalTextBox">1</asp:TextBox><asp:RangeValidator
                                                                                                          ID="RangeValidator3" runat="server" MaximumValue="1000" MinimumValue="1" Type="Integer"
                                                                                                          ControlToValidate="txtScrollamount" ErrorMessage="(1-1000)"></asp:RangeValidator></td>
    </tr>
</table>


<asp:LinkButton ID="cmdUpdate" runat="server" resourcekey="cmdUpdate" CssClass="CommandButton"
                BorderStyle="none" Text="Update" OnClick="cmdUpdate_Click"></asp:LinkButton>&nbsp;
<asp:LinkButton ID="cmdCancel" runat="server" resourcekey="cmdCancel" CssClass="CommandButton"
                BorderStyle="none" Text="Cancel" CausesValidation="False" OnClick="cmdCancel_Click"></asp:LinkButton>