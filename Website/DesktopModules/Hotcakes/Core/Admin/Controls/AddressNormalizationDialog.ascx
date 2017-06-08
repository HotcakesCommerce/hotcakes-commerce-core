<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressNormalizationDialog.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Controls.AddressNormalizationDialog" %>

<div id="hcNormalizedAddressDlg" data-title="<%=Localization.GetString("NormalizeQuestion") %>" data-width="550" data-height="300" style="display: none;">
    <div class="hcPrimaryBlock">
        <h4 id="lblPrimaryAddress" class="hcLabel" runat="server"></h4>
        <table class="hcGrid">
            <tr>
                <th colspan="2"><%=Localization.GetString("Normalized") %></th>
                <th colspan="2"><%=Localization.GetString("Original") %></th>
            </tr>
            <tr class="hcGridRow">
                <td>
                    <input type="radio" id="primarygNm" name="primary" value="N" checked="checked" class="normalRadioButton" />
                </td>
                <td class="hcNormalizedAddress"></td>
                <td>
                    <input type="radio" id="primaryOr" name="primary" value="O" class="normalRadioButton" />
                </td>
                <td class="hcOriginalAddress"></td>
            </tr>
        </table>
    </div>
    <div class="hcSecondaryBlock">
        <h4 id="lblSecondaryAddress" class="hcLabel" runat="server"></h4>
        <table class="hcGrid ">
            <tr>
                <th colspan="2"><%=Localization.GetString("Normalized") %></th>
                <th colspan="2"><%=Localization.GetString("Original") %></th>
            </tr>
            <tr class="hcGridRow">
                <td>
                    <input type="radio" id="secondaryNm" name="secondary" value="N" checked="checked" class="normalRadioButton" />
                </td>
                <td class="hcNormalizedAddress"></td>
                <td>
                    <input type="radio" id="secondaryOr" name="secondary" value="O" class="normalRadioButton" />
                </td>
                <td class="hcOriginalAddress"></td>
            </tr>
        </table>
    </div>
    <ul class="hcActions">
        <li>
            <a  href="#" class="hcPrimaryAction hcSaveNormalizedAction"><%=Localization.GetString("UpdateAddress") %></a>
        </li>
    </ul>
</div>


