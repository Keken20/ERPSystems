var Purid;
var QoutItems;
var QouteId;


$(document).ready(function () {
    $('.view').on('click', function () {
        Purid = $(this).data('pur-id');
        var PurDate = $(this).data('pur-created');
        // Redirect to the controller action that sets ViewBag and renders the view
        window.location.href = '/PurchasingPage/PurchasingQoutationPage?purId=' + Purid + '&purCreated=' + PurDate;
    })

    $('.submitbtn').on('click', function () {
        Purid = $(this).data('pur-id');
        alert(Purid);
        $.ajax({
            url: "/PurchasingPage/SendPoToAdmin", // Replace with your actual server-side endpoint
            method: "POST", // or "GET" depending on your requirements
            data: { purid: Purid }, // Data to send to the server
            success: function (response) {
                // Handle the response from the server, e.g., update the UI
                alert(response.message)
                $('.invoice-wrapper.show').hide();
            },
            error: function (error) {
                console.error(error);
                alert(error);
                console.log(error);
            }
        });
    })

    $('.deletebtn').on('click', function () {
        Purid = $(this).data('pur-id');
        var confirmed = confirm("Are you sure you want to delete?");

        // Check the user's choice
        if (confirmed) {
            $.ajax({
                url: "/PurchasingPage/DeletePurchaseOrder", // Replace with your actual server-side endpoint
                method: "POST", // or "GET" depending on your requirements
                data: { purId: Purid }, // Data to send to the server
                success: function (response) {
                    // Handle the response from the server, e.g., update the UI
                    alert(response.message)
                    window.location.href = window.location.href;
                },
                error: function (error) {
                    alert(error);
                }
            });
        } else {
            // If the user clicks Cancel, do nothing or provide alternative behavior
            // For example, you can display a message or perform another action
            alert("Deletion canceled.");
        }
    });
    $('.poview').on('click', function () {       
        Purid = $(this).data('purchase-id');
        var tableBody = $('#dataTableBody');
         $.ajax({
            type: 'POST',
             url: '/PurchasingPage/GetQouteItemPo',
             data: { purid: Purid },
            success: function (response) {
                // Handle the response from the server
                // Assign the response to the ReqItems variable
                console.log(respone.requestItems);
                QoutItems = response.requestItems;
                console.log(response.message);
                QoutItems.forEach(function (requestItemObject) {
                    // Create a new row
                    var row = $('<tr>');

                    // Add cells for each property in the object
                    row.append($('<td>').text(requestItemObject.ProdId));
                    row.append($('<td>').text(requestItemObject.ProdName));
                    row.append($('<td>').text(requestItemObject.Description));
                    row.append($('<td>').text(requestItemObject.Unit));
                    row.append($('<td>').text(requestItemObject.Quantity));
                    // Add the row to the table body
                    tableBody.append(row);
                })
            },
            error: function (error) {
                // Handle errors
                console.error('Error:', error);
             }            
         });
        $('.invoice-wrapper.show').show();
        $('#printbtn').on('click', function () {          

            var contentToPrint = document.getElementById('print-area');
            var popupWin = window.open('', '_blank', 'width=600,height=600');
            popupWin.document.open();
            popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="/Style/purchaseorder.css" /></head><body onload="window.print()">' + contentToPrint.innerHTML + '</body></html>');
            popupWin.document.close();
          
        })
        $('#back').on('click', function () {
            $('.invoice-wrapper.show').hide();
        })
    });

});

