var Purid;
var PurInfo;
var PurItem;
var QuoteItems;

function saveSupplierInfo() {
    // Gather input values
    var supplierInfo = {
        suppname: $('#suppname').val(),
        suppzipcode: $('#suppzcode').val(),
        suppbarangay: $('#suppbarangay').val(),
        suppcity: $('#suppcity').val(),
        suppmunicipality: $('#suppmunicipality').val(),
        suppphone: $('#suppphone').val()
    };

    // Perform AJAX request to save data
    $.ajax({
        type: 'POST',
        url: '/CustodianPage/SaveQuotationForm', // Replace with the actual URL
        data: supplierInfo,
        success: function (response) {
            console.log('Received save response:', response);

            // Display a message or take appropriate action based on the response
            if (response.success) {
                alert(response.message);
            } else {
                alert('Failed to save data: ' + response.message);
            }
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });
}


function goBack() {
    // Hide the modal
    $('.invoice-wrapper.show').hide();
}

$(document).ready(function () {
    // Show modal on view button click
    $('.create').on('click', function () {
        PurId = $(this).data('pur-id');
        console.log("PurId:", PurId)
        var PurDate = $(this).data('pur-date');
        var tableBody = $('#dataTableBody');
        $('#purid').val(PurId);
        $('#purdate').val(PurDate);

        tableBody.empty();

        $.ajax({
            type: 'POST',
            url: '/CustodianPage/ReceivedQuoteItem',
            data: { id: PurId },
            success: function (response) {
                // Handle the response from the server
                console.log('Received response:', response);

                // Assign the response to the ReqItems variable
                QuoteItems = response.quoteItems;

                QuoteItems.forEach(function (quoteItemObject) {
                    // Create a new row
                    var row = $('<tr>');

                    // Add cells for each property in the object
                    row.append($('<td>').text(quoteItemObject.ProdId));
                    row.append($('<td>').text(quoteItemObject.ProdName));
                    row.append($('<td>').text(quoteItemObject.Description));
                    row.append($('<td>').text(quoteItemObject.Unit));
                    row.append($('<td>').text(quoteItemObject.Quantity));
                    // Add the row to the table body
                    tableBody.append(row);
                });
                console.log('QuoteItems:', QuoteItems);
            },
            error: function (error) {
                console.error('Error:', error);
            },
        });
        $('.invoice-wrapper.show').show();

        ////var allrow = $('#dataTableBody tr');
        ////$.ajax({
        ////    type: 'POST',
        ////    url: '/CustodianPage/SaveQuotationForm',
        ////    data: { id: PurId },
        ////    dataType: 'json',
        ////    success: function (success) {
        ////        // Handle success, update the view if needed
        ////        alert(success.message);
        ////        head.append($('<td class="text-bold">').text('Purchase Quantity'));
        ////        allrow.each(function (i) {
        ////            var inputField = $('<input>').attr('type', 'text', 'style', 'width:85px');
        ////            inputField.css('width', '60px');

        ////            // Append the input field and button to the current row
        ////            $(this).append($('<td>').append(inputField));

        ////            console.log("Row:", i + 1);
        ////        });
        ////        $('#dataTableBody tr').each(function (p) {
        ////            var clickedRow = $(this).closest('tr');
        ////            var cells = clickedRow.find('td');
        ////            rowData = {};

        ////            cells.each(function (j) {
        ////                var cellContent = $(this).find('input').length > 0 ? $(this).find('input').val() : $(this).text();
        ////                rowData['column' + (j + 1)] = cellContent;
        ////            });

        ////            if (p + 1 === rowId) {
        ////                tableData.push(rowData);
        ////            }
        ////        });
        ////        var jsonData = JSON.stringify({ tableData: tableData, reqid: ReqId });
        ////        console.log('Data', jsonData);
        ////        $.ajax({
        ////            type: 'POST',
        ////            url: '/PurchasingPage/SaveProductToPO',
        ////            data: jsonData,
        ////            contentType: 'application/json; charset=utf-8',
        ////            dataType: 'json',
        ////            success: function (response) {
        ////                // Handle the success response from the server
        ////                console.log(response);

        ////                // You can show a success message or perform other actions
        ////                alert(response.message);
        ////            },
        ////            error: function (jqXHR, textStatus, errorThrown) {
        ////                // Handle the error response from the server
        ////                console.error('AJAX Error:', textStatus, errorThrown);

        ////                // You can show an error message or perform other actions
        ////                alert('Error saving data. Please try again.');
        ////            }
        ////        });
        ////    }
        ////});
    });

    // Delete button click
    $('.delete').on('click', function () {
        var quoteId = $(this).data('quote-id');

        // Show confirmation dialog
        var confirmDelete = confirm("Are you sure you want to delete this quotation form?");

        if (confirmDelete) {
            // Perform the AJAX request to delete the quotation form
            $.ajax({
                type: 'POST',
                url: '/CustodianPage/DeleteQuote',
                data: { id: quoteId },
                success: function (response) {
                    console.log('Received delete response:', response);

                    // Display a message or take appropriate action based on the response
                    if (response.success) {
                        alert(response.message);

                        // Check if the form was logically deleted
                        if (response.isDeleted == true) {
                            // Optionally, hide or remove the form on the client side
                            $('.data-quote').hide();
                        } else {
                            // Optionally, reload or update the page
                            location.reload();
                        }
                    } else {
                        alert('Failed to delete quotation form: ' + response.message);
                    }
                },
                error: function (error) {
                    console.error('Error:', error);
                }
            });
        }
        else {
            // User clicked Cancel on the confirmation dialog
            console.log('Deletion canceled.');
        }
    });
});