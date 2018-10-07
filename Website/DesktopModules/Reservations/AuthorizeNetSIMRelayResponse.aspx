<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuthorizeNetSIMRelayResponse.aspx.cs" Inherits="DNNSpecialists.Modules.Reservations.AuthorizeNetSIMRelayResponse" %>
<script type="text/javascript" language="javascript">
    document.location = "<%=ReceiptUrl%>";
</script>
<a href="<%=ReceiptUrl%>"><asp:label runat="server" id="label" /></a>