<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ctlDropDownTree.ascx.cs" Inherits="ZLDNN.Modules.DNNArticle.UserControls.ctlDropDownTree" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<dnn:DnnJsInclude ID="DnnJsInclude1" runat="server"  FilePath="~/desktopmodules/DNNArticle/javascript/jqx-all.js" Priority="200" />

<dnn:DnnCssInclude ID="DnnCssInclude3" runat="server"  FilePath="~/desktopmodules/DNNArticle/css/jqx.base.css" Priority="201"/>

<script type="text/javascript">
    $(document).ready(function () {
        var source = <%=JsonData %>;
        $("#dropDownButton<%=ClientID %>").jqxDropDownButton({ width: 250, height: 25 });
        $('#dropDownButton<%=ClientID %>').on('open', function() {
            //alert();
             $('#jqxTree<%=ClientID %>').show();
        });

        $("#jqxComboBox<%=ClientID %>").jqxComboBox(
            { 
                multiSelect: true,
                width: '250',
                height: '25px',
                showArrow: false
            });

        $('#jqxTree<%=ClientID %>').on('select', function (event) {
            var args = event.args;
            var item = $('#jqxTree<%=ClientID %>').jqxTree('getItem', args.element);

            var dropDownContent = '<div style="position: relative; margin-left: 3px; margin-top: 5px;">' + item.label + '</div>';
            $("#dropDownButton<%=ClientID %>").jqxDropDownButton('setContent', dropDownContent);
            $('#jqxTree<%=ClientID %>').hide();
            var newitem1={ label: item.label, value: item.value }
            $("#jqxComboBox<%=ClientID %>").jqxComboBox('addItem', newitem1);
            $("#jqxComboBox<%=ClientID %>").jqxComboBox('selectItem',  newitem1);

            //setValue();

        });
        $("#jqxTree<%=ClientID %>").jqxTree({source:source, width: 250, height: 220 });

        if ($("#<%=hidSelectedValue.ClientID %>").val() && $("#<%=hidSelectedText.ClientID %>").val()) {
            var values = $("#<%=hidSelectedValue.ClientID %>").val();
            var texts=$("#<%=hidSelectedText.ClientID %>").val();
            var varrs = values.split(";");
            var textarr = texts.split(";");

            for (var i = 0; i < varrs.length; i++) {
                var newitem = { label: textarr[i], value: varrs[i] };
                $("#jqxComboBox<%=ClientID %>").jqxComboBox('addItem', newitem);
                $("#jqxComboBox<%=ClientID %>").jqxComboBox('selectItem',  newitem);
            }
            $("#divComboBox<%=ClientID %>").show();
        }

        $("#jqxComboBox<%=ClientID %>").on('change',
            function(event) {
                var items = $("#jqxComboBox<%=ClientID %>").jqxComboBox('getSelectedItems');
                var selectedvalues = "";
                var selectedtexts = "";
                $.each(items,
                    function(index) {
                        selectedtexts += this.label;
                        selectedvalues += this.value;

                        if (items.length - 1 != index) {
                            selectedtexts += ";";
                            selectedvalues += ";";
                        }

                    });

                $("#<%=hidSelectedValue.ClientID %>").val(selectedvalues);
                $("#<%=hidSelectedText.ClientID %>").val(selectedtexts);
                if (selectedvalues) {
                    $("#divComboBox<%=ClientID %>").show();
                } else {
                    $("#divComboBox<%=ClientID %>").hide();
                }
                
            });
    });
</script>
<div style="display:inline-block">
    <div id="divComboBox<%=ClientID %>" style="display: none">
        <div id='jqxComboBox<%=ClientID %>'>
        </div>
    </div>
    <div >
        <div id="dropDownButton<%=ClientID %>">
            <div style="border: none;" id='jqxTree<%=ClientID %>'>
            </div>
        </div>
    </div>
   
</div>
<asp:HiddenField runat="server" id="hidSelectedValue"/>
<asp:HiddenField runat="server" id="hidSelectedText"/>