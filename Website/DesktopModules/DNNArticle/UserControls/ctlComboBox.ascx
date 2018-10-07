<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlComboBox.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.ctlComboBox" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js" Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>


<script type="text/javascript">
    $(document).ready(function () {
        var url = "<%=ServiceUrl %>";// "/dnn610dev/desktopmodules/dnnarticle/services/getauthor.ashx?portalid=0";

        // prepare the data
        var source =
        {
            datatype: "json",
            datafields: [
                { name: '<%=DisplayField %>' },
                { name: '<%=ValueField %>' }
            ],
            url: url,
            data: {IncludeAll:'<%=IncludeAll %>'}
        };
        var dataAdapter = new $.jqx.dataAdapter(source,
            {
                formatData: function (data) {
                    if ($("#jqxComboBox<%=ClientID %>").jqxComboBox('searchString')) {
                                  data.name_startsWith = $("#jqxComboBox<%=ClientID %>").jqxComboBox('searchString');
                        return data;
                    }
                }
            }
        );

        // Create a jqxComboBox
            $("#jqxComboBox<%=ClientID %>").jqxComboBox(
            { source: dataAdapter,
                remoteAutoComplete: <%=RemoteAutoComplete %>,
                autoDropDownHeight: true,
                multiSelect: <%=MultiSelect %>,
                selectedIndex: -1,
                displayMember: "<%=DisplayField %>",
                valueMember: "<%=ValueField %>",
                width: '250',
                height: '25px',
               
                search: function (searchString) {
                   // alert("1:"+searchString);
                    dataAdapter.dataBind();
                }
            });
        <% if (MultiSelect == "true")
           {%>

        $("#jqxComboBox<%=ClientID %>").on('change', function (event) {
            var items = $("#jqxComboBox<%=ClientID %>").jqxComboBox('getSelectedItems');
            var selectedvalues = "";
            var selectedtexts = "";
            $.each(items, function(index) {
                selectedtexts += this.label;
                selectedvalues += this.value;

                if (items.length - 1 != index) {
                    selectedtexts += ";";
                    selectedvalues += ";";
                }
                
            });

            $("#<%=hidSelectedValue.ClientID %>").val(selectedvalues);
            $("#<%=hidSelectedText.ClientID %>").val(selectedtexts);
         
        });

        <% } else { %>
            $("#jqxComboBox<%=ClientID %>").on('select', function (event) {
            if (event.args) {
                var item = event.args.item;
                if (item) {
                    $("#<%=hidSelectedValue.ClientID %>").val(item.value);
                    $("#<%=hidSelectedText.ClientID %>").val(item.label);
                }
            }

            });
        <% }%>

        <% if (MultiSelect == "true")
           {%>
        if ($("#<%=hidSelectedValue.ClientID %>").val() && $("#<%=hidSelectedText.ClientID %>").val()) {
            var values = $("#<%=hidSelectedValue.ClientID %>").val();
            var texts=$("#<%=hidSelectedText.ClientID %>").val();
            var varrs = values.split(";");
            var textarr = texts.split(";");
            for (var i = 0; i < varrs.length; i++) {
                $("#jqxComboBox<%=ClientID %>").jqxComboBox('addItem', { label: textarr[i], value: varrs[i] });
                $("#jqxComboBox<%=ClientID %>").jqxComboBox('selectItem',  textarr[i]);
            }
        }

        <% } else { %>
        $("#jqxComboBox<%=ClientID %>").on('bindingComplete', function (event) {
            if ($("#<%=hidSelectedValue.ClientID %>").val() && $("#<%=hidSelectedText.ClientID %>").val()) {
                //$("#jqxComboBox<%=ClientID %>").jqxComboBox('addItem', { label: $("#<%=hidSelectedText.ClientID %>").val(), value: $("#<%=hidSelectedValue.ClientID %>").val() });
                //$("#jqxComboBox<%=ClientID %>").jqxComboBox('selectIndex', 0);
                //alert($("#<%=hidSelectedText.ClientID %>").val());
                $("#jqxComboBox<%=ClientID %>").jqxComboBox('selectItem', { label: $("#<%=hidSelectedText.ClientID %>").val(), value: $("#<%=hidSelectedValue.ClientID %>").val() });
            }
        });
        <% }%>
    });
</script>
    <div id='jqxComboBox<%=ClientID %>'></div>

<asp:HiddenField runat="server" id="hidSelectedValue"/>
<asp:HiddenField runat="server" id="hidSelectedText"/>
