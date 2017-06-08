<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopFive.ascx.cs" Inherits="Hotcakes.Modules.Core.Admin.Dashboard.TopFive" %>
<script type="text/javascript">
    jQuery(function ($) {
        var Top5 = {
            init: function () {
                this.model = null;
                this.$hcTop5Block = $(".hcTop5Block");
                this.$hcProductsBlock = $(".hcProductsBlock");
                this.$hcCustomersBlock = $(".hcCustomersBlock");
                this.$hcVendorsBlock = $(".hcVendorsBlock");
                this.$hcAffiliatesBlock = $(".hcAffiliatesBlock");

                this.$prMode = $("#<%=ddlProductMode.ClientID%>");
                this.$prMode.change(Top5.loadProducts);
                this.$cmMode = $("#<%=ddlCustomerMode.ClientID%>");
                this.$cmMode.change(Top5.loadCustomers);
                this.$vType = $("#<%=ddlVendorType.ClientID%>");
                this.$vType.change(Top5.loadVendors);
                this.$aType = $("#<%=ddlAffiliateMode.ClientID%>");
                this.$aType.change(Top5.loadAffiliates);

                this.load("ALL", this.$hcTop5Block);
            },
            load: function (mode, scope) {
                var self = Top5;
                scope.ajaxLoader("start");

                $.post('Dashboard/DashboardHandler.ashx',
                    {
                        "method": "GetTop5Data",
                        "mode": mode,
                        "productMode": self.$prMode.val(),
                        "customerMode": self.$cmMode.val(),
                        "vendorType": self.$vType.val(),
                        "affMode": self.$aType.val()
                    })
                    .done(function (data) {
                        if (data.Products) {
                            $.each(data.Products, function (i, v) {
                                v.Url = "/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Performance.aspx?id=" + v.Id;
                            });
                            if (self.model)
                                ko.mapping.fromJS(data.Products, self.model.Products);
                        }
                        if (data.AbandonedProducts) {
                            $.each(data.AbandonedProducts, function (i, v) {
                                v.Url = "/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Performance.aspx?id=" + v.Id;
                            });
                            if (self.model)
                                ko.mapping.fromJS(data.AbandonedProducts, self.model.AbandonedProducts);
                        }
                        if (data.Customers) {
                            $.each(data.Customers, function (i, v) {
                                v.Url = "/DesktopModules/Hotcakes/Core/Admin/people/Users_Edit.aspx?id=" + v.Id;
                            });
                            if (self.model)
                                ko.mapping.fromJS(data.Customers, self.model.Customers);
                        }
                        if (data.Vendors) {
                            $.each(data.Vendors, function (i, v) {
                                v.Url = "/DesktopModules/Hotcakes/Core/Admin/people/Vendors_edit.aspx?id=" + v.Id;
                            });
                            if (self.model)
                                ko.mapping.fromJS(data.Vendors, self.model.Vendors);
                        }
                        if (data.Affiliates) {
                            $.each(data.Affiliates, function (i, v) {
                                v.Url = "/DesktopModules/Hotcakes/Core/Admin/people/Affiliates_Profile.aspx?id=" + v.Id;
                            });
                            if (self.model)
                                ko.mapping.fromJS(data.Affiliates, self.model.Affiliates);
                        }
                        if (!self.model) {
                            self.model = {};
                            self.model.Products = ko.mapping.fromJS(data.Products);
                            self.model.AbandonedProducts = ko.mapping.fromJS(data.AbandonedProducts);
                            self.model.Customers = ko.mapping.fromJS(data.Customers);
                            self.model.SearchTerms = ko.mapping.fromJS(data.SearchTerms);
                            self.model.Vendors = ko.mapping.fromJS(data.Vendors);
                            self.model.Affiliates = ko.mapping.fromJS(data.Affiliates);
                            ko.applyBindings(self.model, scope[0]);
                        }
                    })
                    .always(function () { scope.ajaxLoader("stop"); });
            },
            loadProducts: function () { Top5.load("P", Top5.$hcProductsBlock); },
            loadCustomers: function () { Top5.load("C", Top5.$hcCustomersBlock); },
            loadVendors: function () { Top5.load("V", Top5.$hcVendorsBlock); },
            loadAffiliates: function () { Top5.load("A", Top5.$hcAffiliatesBlock); }
        };

        Top5.init();
    });
</script>
<div class="hcBlock hcTop5Block">
    <h2><i class="hcDIconTop5"></i><%=Localization.GetString("Top5") %></h2>
    <div class="hcBlockRow">
        <div class="hcBlockContent33p hcProductsBlock">
            <h3><%=Localization.GetString("Products") %></h3>
            <asp:DropDownList ID="ddlProductMode" runat="server" CssClass="hcSelectionRibbon">
                <asp:ListItem resourcekey="Tab_Amount" Value="1" />
                <asp:ListItem resourcekey="Tab_Quanity" Value="2" />
                <asp:ListItem resourcekey="Tab_Rating" Value="3" />
                <asp:ListItem resourcekey="Tab_Review" Value="4" />
            </asp:DropDownList>
            <!-- ko if: Products().length -->
            <ul class="hcTable" data-bind="foreach: Products">
                <li><a data-bind="text: Name, attr: { href: Url, title: Name }" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                    <label data-bind="text: Amount"></label>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: Products().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems" />
            <!-- /ko -->
        </div>
        <div class="hcBlockContent33p">
            <h3><%=Localization.GetString("AbandonedProducts") %></h3>
            <!-- ko if: AbandonedProducts().length -->
            <ul class="hcTable hcTopSpacer" data-bind="foreach: AbandonedProducts">
                <li><a data-bind="text: Name, attr: { href: Url, title: Name }" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: AbandonedProducts().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems hcTopSpacer" />
            <!-- /ko -->
        </div>
        <div class="hcBlockContent33p hcCustomersBlock">
            <h3><%=Localization.GetString("Customers") %></h3>
            <asp:DropDownList ID="ddlCustomerMode" runat="server" CssClass="hcSelectionRibbon">
                <asp:ListItem resourcekey="Tab_Amount" Value="1" />
                <asp:ListItem resourcekey="Tab_Frequency" Value="2" />
                <asp:ListItem resourcekey="Tab_Activity" Value="3" />
                <asp:ListItem resourcekey="Tab_Abandoned" Value="4" />
            </asp:DropDownList>
            <!-- ko if: Customers().length -->
            <ul class="hcTable" data-bind="foreach: Customers">
                <li><a data-bind="text: Name, attr: { href: Url, title: Name }" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                    <label data-bind="text: Amount"></label>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: Customers().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems" />
            <!-- /ko -->
        </div>
    </div>
    <div class="hcBlockRow">
        <div class="hcBlockContent33p">
            <h3><%=Localization.GetString("PopularSearchTerms") %></h3>
            <!-- ko if: SearchTerms().length -->
            <ul class="hcTable hcTopSpacer" data-bind="foreach: SearchTerms">
                <li><a data-bind="text: Name" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: SearchTerms().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems hcTopSpacer" />
            <!-- /ko -->
        </div>
        <div class="hcBlockContent33p hcVendorsBlock">
            <h3>
                <asp:DropDownList ID="ddlVendorType" runat="server" CssClass="hcSelectionRibbon">
                    <asp:ListItem resourcekey="Tab_Vendors" Value="1" />
                    <asp:ListItem resourcekey="Tab_Maunfactures" Value="2" />
                </asp:DropDownList>
            </h3>
            <!-- ko if: Vendors().length -->
            <ul class="hcTable" data-bind="foreach: Vendors">
                <li><a data-bind="text: Name, attr: { href: Url, title: Name }" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                    <label data-bind="text: Amount"></label>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: Vendors().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems" />
            <!-- /ko -->
        </div>
        <div class="hcBlockContent33p hcAffiliatesBlock">
            <h3><%=Localization.GetString("Affiliates") %></h3>
            <asp:DropDownList ID="ddlAffiliateMode" runat="server" CssClass="hcSelectionRibbon">
                <asp:ListItem resourcekey="Tab_Referral" Value="1" />
                <asp:ListItem resourcekey="Tab_Revenue" Value="2" />
            </asp:DropDownList>
            <!-- ko if: Affiliates().length -->
            <ul class="hcTable" data-bind="foreach: Affiliates">
                <li><a data-bind="text: Name, attr: { href: Url, title: Name }" target="_blank"></a>
                    <span data-bind="text: Count"></span>
                    <label data-bind="text: Amount"></label>
                </li>
            </ul>
            <!-- /ko -->
            <!-- ko ifnot: Affiliates().length -->
            <asp:Label runat="server" resourcekey="NoItemsFound" CssClass="hcNoItems" />
            <!-- /ko -->
        </div>
    </div>
</div>
