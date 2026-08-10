$(document).ready(function () {
    $(".hc-search-input button").click(function (e) {
        DoSearch();
        e.preventDefault();
    });

    function DoSearch() {
        $(".hc-search-input input[type='image']").click();
    }
});