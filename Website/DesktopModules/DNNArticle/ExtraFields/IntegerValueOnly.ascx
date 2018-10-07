<%@ Control CodeBehind="IntegerValueOnly.ascx.cs" Language="C#" AutoEventWireup="true" Inherits=" ZLDNN.Modules.DNNArticle.ExtraFields.IntegerValueOnly" %>

            <asp:TextBox id="txtValue" runat="server" Width="224px" EnableViewState="True" CssClass="NormalTextBox" ></asp:TextBox>
            <asp:RangeValidator id="rv" runat="server" ControlToValidate="txtValue" Display="Dynamic"  ValidationGroup="ArticleEditor" ></asp:RangeValidator>
            <asp:RequiredFieldValidator id="rq" runat="server" ControlToValidate="txtValue" ErrorMessage="*"  ValidationGroup="ArticleEditor" 
                                        Display="Dynamic" ></asp:RequiredFieldValidator>
       