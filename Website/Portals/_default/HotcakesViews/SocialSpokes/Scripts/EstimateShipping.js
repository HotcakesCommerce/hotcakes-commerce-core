jQuery(function ($) {

    var $regionDropdown = $("#hcEstimateShipping #RegionId");
    var $countryDropdown = $("#hcEstimateShipping #CountryId");
    var $postalCodeInput = $("#hcEstimateShipping #PostalCode");
    var $tempRegionIdHidden = $("#hcEstimateShipping #TempRegionId");

    function LoadRegionsWithSelection(regionlist, countryid, selectedregion) {
    }

    $countryDropdown.change(function () {
        var countryid = $countryDropdown.find("option:selected").val();
        $.post(hcc.getServiceUrl("estimateshipping/getregions/" + countryid),
            { 
                "regionid": $tempRegionIdHidden.val() 
            },
            function (data) {
                $regionDropdown.html(data.Regions);
                $tempRegionIdHidden.val('');
            },
            "json"
        );
    });


    $("#hcGetRates").click(function () {
        var countryid = $countryDropdown.find("option:selected").val();
        var regionid = $regionDropdown.find("option:selected").val();
        var ratesList = $("#hcShippingRatesList");

        $.post(hcc.getServiceUrl("estimateshipping/getrates"),
            {
                "countryId": countryid,
                "regionId": regionid,
                "postalCode" : $postalCodeInput.val()
            },
            function (data) {
                ratesList.empty();
                $.each(data, function (i, o) {
                    ratesList.append($("<li>").html(o.RateAndNameForDisplay));
                })
            },
            "json"
         );
    });

    $countryDropdown.change();
});