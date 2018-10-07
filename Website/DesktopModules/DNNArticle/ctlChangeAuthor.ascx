<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlChangeAuthor.ascx.cs"
    Inherits="ZLDNN.Modules.DNNArticle.ctlChangeAuthor" %>
    
    
    
<asp:PlaceHolder ID="ph" runat="server"></asp:PlaceHolder>

<asp:Label ID="lblOwner" runat="server" CssClass="NormalBold"></asp:Label>
<asp:LinkButton ID="lnkChange" runat="server" CssClass="UserLabel" resourcekey="lnkChangeOwner"
    OnClick="lnkChange_Click" CausesValidation="False" BorderStyle="none"></asp:linkbutton> 

