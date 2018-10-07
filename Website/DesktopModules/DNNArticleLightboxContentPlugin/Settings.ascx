<%@ Control Language="C#" AutoEventWireup="true" Inherits="ZLDNN.Modules.DNNArticleLightboxContentPlugin.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>

<div class="dnnFormItem" >
    <dnn:Label ID="lbToken" runat="server" ></dnn:Label>
    <asp:TextBox ID="txtToken" CssClass="NormalTextBox" Width="390" TextMode="SingleLine" 
                         MaxLength="2000" runat="server" ReadOnly="true" />
</div>


<div class="dnnFormItem" >
    <dnn:Label ID="lbTokenForContentSlider" runat="server" ></dnn:Label>
    <asp:TextBox ID="txtTokenForContentSlider" CssClass="NormalTextBox" Width="390" TextMode="SingleLine" 
                         MaxLength="2000" runat="server" ReadOnly="true" />
</div>


<div class="dnnFormItem" >
    <dnn:Label ID="lbContentDisplayNumber" runat="server" Suffix=":" />
    <asp:TextBox ID="txtContentDisplayNumber" runat="server" Width="170px"  CssClass="NormalTextBox"></asp:TextBox>
</div>


<div class="dnnFormItem" >
    <dnn:Label ID="lblTemplate" runat="server" ControlName="txtTemplate" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtTemplate" CssClass="NormalTextBox" Width="390" Columns="30" TextMode="MultiLine"
                         Rows="10" MaxLength="2000" runat="server" />
</div>

<div class="dnnFormItem">
     <dnn:Label ID="lbRepeatLayout" runat="server"></dnn:Label>
      <asp:DropDownList ID="cboRepeatLayout" runat="server" >
                <asp:ListItem Value="0">Table</asp:ListItem>
                <asp:ListItem Value="1">Flow</asp:ListItem>
            </asp:DropDownList>     
</div>

<div class="dnnFormItem" >
    <dnn:Label ID="lbRepeatColumn" runat="server" ></dnn:Label>
    <asp:TextBox ID="txtRepeatColumn" runat="server"></asp:TextBox><asp:RangeValidator
                ID="RangeValidator4" runat="server" ControlToValidate="txtRepeatColumn" Type="Integer"
                MinimumValue="1" MaximumValue="40" ErrorMessage="(1-40)"></asp:RangeValidator>
</div>


<div class="dnnFormItem" >
    <dnn:Label ID="lbRepeatDirection" runat="server" ></dnn:Label>
    <asp:DropDownList ID="cboRepeatDirection" runat="server" Width="152px">
                    <asp:ListItem Value="0">Horizontal</asp:ListItem>
                    <asp:ListItem Value="1">Vertical</asp:ListItem>
                </asp:DropDownList>
</div>


<div class="dnnFormItem" >
    <dnn:label id="lbAnimationSpeed" runat="server" controlname="txtTemplate" suffix=":"></dnn:label>
     <asp:DropDownList ID="cboAnimationSpeed" runat="server">
                <asp:ListItem Text="Slow" Value="slow"></asp:ListItem>
                <asp:ListItem Text="Fast" Value="fast"></asp:ListItem>
                <asp:ListItem Text="Normal" Value="Normal"></asp:ListItem>
            </asp:DropDownList>
</div>


<div class="dnnFormItem" >
    <dnn:label id="lbtheme" runat="server" controlname="txtTemplate" suffix=":"></dnn:label>
    <asp:DropDownList ID="cbotheme" runat="server">
                <asp:ListItem Text="light_rounded" Value="light_rounded"></asp:ListItem>
                <asp:ListItem Text="dark_rounded" Value="dark_rounded"></asp:ListItem>
                <asp:ListItem Text="light_square" Value="light_square"></asp:ListItem>
                <asp:ListItem Text="dark_square" Value="dark_square"></asp:ListItem>
                <asp:ListItem Text="facebook" Value="facebook"></asp:ListItem>
            </asp:DropDownList>
</div>


<div class="dnnFormItem" >
    <dnn:label id="lbAutoplay" runat="server" controlname="txtTemplate" suffix=":"></dnn:label>
    <asp:DropDownList ID="cboAutoPlay" runat="server">
                <asp:ListItem Text="False" Value="false"></asp:ListItem>
                <asp:ListItem Text="True" Value="true"></asp:ListItem>
            
            </asp:DropDownList>
</div>


<div class="dnnFormItem" >
    <dnn:label id="lbOverlayGallery" runat="server" controlname="txtTemplate" suffix=":"></dnn:label>
    <asp:DropDownList ID="cboOverlayGallery" runat="server">
                <asp:ListItem Text="True" Value="true"></asp:ListItem>
                <asp:ListItem Text="False" Value="false"></asp:ListItem>
           
            
            </asp:DropDownList>
</div>

<div class="dnnFormItem" >
    <dnn:label id="lbSlideShow" runat="server" controlname="txtTemplate" suffix=":"></dnn:label>
    <asp:TextBox ID="txtSlideShow" runat="server"></asp:TextBox><asp:RangeValidator ID="RangeValidator1" Type="Integer" ControlToValidate="txtSlideShow"  MaximumValue="999999" MinimumValue="1"
                                                                                            runat="server" ErrorMessage="(1-999999)"></asp:RangeValidator>
  
</div>

<div class="dnnFormItem" >
    <dnn:Label ID="lblNextButtonText" runat="server" Suffix=":" />
    <asp:TextBox ID="txtNextButtonText" runat="server" Width="170px"  CssClass="NormalTextBox"></asp:TextBox>
</div>

<div class="dnnFormItem" >
    <dnn:Label ID="lblContentSliderCSS" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtContentSliderCSS" CssClass="NormalTextBox" Width="300" Columns="30" TextMode="MultiLine"
                         Rows="10" MaxLength="2000" runat="server" />
</div>
<div class="dnnFormItem" >
    <dnn:Label ID="lbContentWidth" runat="server" Suffix=":"></dnn:Label>
     <asp:TextBox ID="txtContentWidth" CssClass="NormalTextBox" Width="300"  runat="server" />
</div>
<div class="dnnFormItem" >
    <dnn:Label ID="lbContentHeight" runat="server" Suffix=":"></dnn:Label>
    <asp:TextBox ID="txtContentHeight" CssClass="NormalTextBox" Width="300"  runat="server" />
</div>

