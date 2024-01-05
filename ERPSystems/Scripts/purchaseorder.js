var Purid;
var PurInfo;
var PurItem;
$(document).ready(function () {
    // Show modal on view button click
    $('.view').on('click', function () {
        var tableBody = $('#dataTableBody');
        PurId = $(this).data('pur-id');
        console.log("PurId:",PurId)
        //var ReqName = $(this).data('req-name');
        //var ReqDate = $(this).data('req-date');

        $('#purid').val(PurId);
    
        $(document).ready(function () {
            // Make an AJAX request to your controller action
            $.ajax({
                url: '/PurchasingPage/GetPurchaseOrderInfo', // Replace with the actual URL
                type: 'POST',
                data: { selectedPurId: PurId }, // Replace with the actual value
                success: function (data) {
                    // Handle the returned JSON data
                    console.log(data.purchaseorderinfo);

                    PurInfo = data.purchaseorderinfo;
                    for (var i = 0; i < PurInfo.length; i++) {
                        var currentObject = PurInfo[i];
                        var reqname = currentObject.RequestFrom;
                        $('#reqname').val(reqname);
                    }
                },
                error: function (error) {
                    console.error('Error occurred:', error);
                }
            });
            $.ajax({
                type: 'POST',
                url: '/PurchasingPage/GetPOItem',
                data: { selectedPurId: PurId },
                success: function (response) {
                    // Handle the response from the server
                    console.log('Received response:', response);

                    // Assign the response to the ReqItems variable
                    PurItem = response.purchaseorderitem;

                    PurItem.forEach(function (requestItemObject) {
                        // Create a new row
                        var row = $('<tr>');

                        // Add cells for each property in the object
                        row.append($('<td>').text(requestItemObject.PurId));
                        row.append($('<td>').text(requestItemObject.ProdId));
                        row.append($('<td>').text(requestItemObject.ProdName));
                        row.append($('<td>').text(requestItemObject.ProdDescription));
                        row.append($('<td>').text(requestItemObject.PurUnit));
                        row.append($('<td>').text(requestItemObject.PurQuantity));
                        // Add the row to the table body
                        tableBody.append(row);
                    })
                },
                error: function (error) {
                    // Handle errors
                    console.error('Error:', error);
                }
            });
        });
        
        $('.invoice-wrapper.show').show();

    });
});