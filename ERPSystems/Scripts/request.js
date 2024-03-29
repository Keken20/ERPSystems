﻿var ReqItems;
var ReqId;


$(document).ready(function () {
    // Show modal on view button click
    $('.view').on('click', function () {
        var reqId = $(this).data('req-id');       
        var ReqName = $(this).data('req-name');
        var ReqDate = $(this).data('req-date');
        var tableBody = $('#dataTableBody');
        $('#reqid').val(reqId);
        $('#reqname').val(ReqName);
        $('#reqdate').val(ReqDate);
        
        console.log(ReqName);
        $.ajax({
            type: 'POST',
            url: '/AdminPage/ReceivedRequestItem',
            data: { id: reqId },
            success: function (response) {
                // Handle the response from the server
                console.log('Received response:', response);

                // Assign the response to the ReqItems variable
                ReqItems = response.requestItems;

                ReqItems.forEach(function (requestItemObject) {
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

        // Other parts of your code can now access ReqItems
        // For example, you can use it outside the Ajax request
        console.log('Outside Ajax request:', ReqItems);

        
        // Show the modal
        $('.invoice-wrapper.show').show();        
    });

    $('.deletebtn').on('click', function () {
        ReqId = $(this).data('req-id');
        console.log(ReqId);
        var confirmed = confirm("Are you sure you want to delete?");

        // Check the user's choice
        if (confirmed) {
            $.ajax({
                url: "/PurchasingPage/DeleteRequest", // Replace with your actual server-side endpoint
                method: "POST", // or "GET" depending on your requirements
                data: { reqId: ReqId }, // Data to send to the server
                success: function (response) {
                    // Handle the response from the server, e.g., update the UI
                    alert(response.message)
                    location.reload();
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

});