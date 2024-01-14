var Purid;
var Prodid;
var Quoteid;
var tableData;
var head;
var firstButtonClicked = false;
var itemsReceived = false;

$(document).ready(function () {
    $('.view').on('click', function () {
        Purid = $(this).data('pur-id');
        Prodid = $(this).data('pur-id');
        var PurDate = $(this).data('pur-date');
        var SuppName = $(this).data('supp-name');
        var SuppZcode = $(this).data('supp-zcode');
        var SuppBarangay = $(this).data('supp-barangay');
        var SuppCity = $(this).data('supp-city');
        var SuppMunicipality = $(this).data('supp-municipality');
        var SuppPhone = $(this).data('supp-phone');
        Quoteid = $(this).data('quote-id');
        var Discount = $(this).data('quote-discount');
        var Subtotal = $(this).data('quote-subtotal');
        var Total = $(this).data('quote-total');
        var tableBody = $('#dataTableBody');
        $('#purid').val(Purid);
        $('#prodid').val(Prodid);
        $('#purdate').val(PurDate);
        $('#suppname').val(SuppName);
        $('#suppzcode').val(SuppZcode);
        $('#suppbarangay').val(SuppBarangay);
        $('#suppcity').val(SuppCity);
        $('#suppmunicipality').val(SuppMunicipality);
        $('#suppphone').val(SuppPhone);
        $('#quoteid').val(Quoteid);
        $('#discount').val(Discount);
        $('#subtotal').val(Subtotal);
        $('#total').val(Total);

        $('#dataTableBody').empty();

        $.ajax({
            type: 'POST',
            url: '/CustodianPage/ReceivedPOItem',
            data: { id: Quoteid },
            success: function (response) {
                // Handle the response from the server
                console.log('Received response:', response);

                // Assign the response to the QuoteItems variable
                tableData = response.quoteItems;

                tableData.forEach(function (quoteItemObject) {
                    // Create a new row
                    var row = $('<tr>');

                    // Add cells for each property in the object
                    row.append($('<td>').text(quoteItemObject.ProdId));
                    row.append($('<td>').text(quoteItemObject.ProdName));
                    row.append($('<td>').text(quoteItemObject.Description));
                    row.append($('<td>').text(quoteItemObject.Unit));
                    row.append($('<td>').text(quoteItemObject.UnitPrice));
                    var quantityInput = $('<input>').attr({
                        'type': 'text',
                        'style': 'width:80px; text-align: center;',
                        'class': 'quantityInput',
                        'readonly': 'readonly',
                        'value': quoteItemObject.Quantity
                    });
                    row.append($('<td>').append(quantityInput));

                    // Add a cell for 'Update Inventory' button in each row
                    var updateInventoryCell = $('<td>');
                    var updateInventoryButton = $('<button class="updateInventoryButton">Update QOH</button>');

                    // Attach a click event handler to the 'Update Inventory' button
                    updateInventoryButton.on('click', function () {
                        // Check if the data has already been updated
                        if ($(this).hasClass('alreadyUpdated')) {
                            alert('Data for this item has already been updated and cannot be updated again.');
                            return;
                        }

                        // Check if the items are already received
                        if (itemsReceived) {
                            alert('Items for this purchase order have already been received. Cannot update QOH.');
                            return;
                        }

                        $.ajax({
                            type: 'POST',
                            url: '/CustodianPage/UpdateQOH',
                            data: { updateInventory: tableData, purid: Purid },
                            success: function (response) {
                                // Handle the response from the server
                                console.log('Received response:', response);

                                if (response.success) {
                                    // Mark the button as already updated
                                    updateInventoryButton.addClass('alreadyUpdated');

                                    // Set the itemsReceived variable to true
                                    itemsReceived = true;
                                } else {
                                    // If the update is not successful, display an error message
                                    alert('Error updating inventory: ' + response.message);
                                }
                            },
                            error: function (error) {
                                console.error('Error:', error);
                            }
                        });

                        alert('Updating inventory for item in row: ' + row.index());
                    });

                    updateInventoryCell.append(updateInventoryButton);
                    row.append(updateInventoryCell);
                    // Add the row to the table body
                    tableBody.append(row);
                })
                console.log('tableData:', tableData);
            },
            error: function (error) {
                console.error('Error:', error);
            },
        });

        $('#returnButton').on('click', function () {
            var confirmed = confirm("Are you sure you want to return items to the supplier?");
            console.log("Purid:", Purid);

            // Check the user's choice
            if (confirmed) {
                $.ajax({
                    url: '/CustodianPage/ReturnToSupplier',
                    type: 'POST',
                    data: { purid: Purid },
                    success: function (result) {
                        if (result.response) {
                            alert(result.message); // You can handle the success response as needed
                            window.location.href = '/CustodianPage/CustodianPurchaseOrder';
                        } else {
                            alert(result.message); // Handle the case where there is an error
                        }
                    },
                    error: function () {
                        alert('Error occurred while processing the request.');
                    }
                });
            } else {
                // If the user clicks Cancel, do nothing or provide alternative behavior
                // For example, you can display a message or perform another action
                alert("Return canceled.");
            }
        });

        $('.invoice-wrapper.show').show();
    });

    $('#receiveButton').on('click', function () {
        // Check if the items have been received (QOH updated)
        if (!itemsReceived) {
            alert('Please initiate the QOH update before proceeding.');
            return;
        }

        alert('Received Items Successfully');
        window.location.href = '/CustodianPage/CustodianPurchaseOrder';
    });

    $('.delete').on('click', function () {
        Purid = $(this).data('pur-id');
        var confirmed = confirm("Are you sure you want to delete?");

        // Check the user's choice
        if (confirmed) {
            $.ajax({
                url: "/CustodianPage/DeletePurchaseOrder", // Replace with your actual server-side endpoint
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
        }
        else {
            // If the user clicks Cancel, do nothing or provide alternative behavior
            // For example, you can display a message or perform another action
            alert("Deletion canceled.");
        }
    });
});

function goBack() {
    // Hide the modal
    $('.invoice-wrapper.show').hide();
}