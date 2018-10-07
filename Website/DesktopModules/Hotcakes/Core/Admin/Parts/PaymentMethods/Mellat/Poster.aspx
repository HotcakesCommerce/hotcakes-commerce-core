<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Poster.aspx.cs" Inherits="Hotcakes.Shetab.Mellat.Poster" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>BP PGW Poster</title>

    <script language="javascript" type="text/javascript">
        function postRefId(refIdValue) {
            var form = document.createElement("form");
            form.setAttribute("method", "POST");
            form.setAttribute("action", "https://pgw.bpm.bankmellat.ir/pgwchannel/startpay.mellat");
            form.setAttribute("target", "_self");
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("name", "RefId");
            hiddenField.setAttribute("value", refIdValue);
            form.appendChild(hiddenField);
            document.body.appendChild(form);
            form.submit();
            document.body.removeChild(form);
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    
    </form>
</body>
</html>
