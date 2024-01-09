var Purid;
var supplierInfo;
var prices;
var QuoteItems;
var tableData;
var rowData;
var quoteid;
var SupplierName;

$(document).ready(function () {
    // Show modal on view button click
    $('.create').on('click', function () {
        Purid = $(this).data('pur-id');
        console.log("PurId:", Purid);
        var PurDate = $(this).data('pur-date');
        var tableBody = $('#dataTableBody');
        $('#purid').val(Purid);
        $('#purdate').val(PurDate);

        clearInputs();

        $.ajax({
            type: 'POST',
            url: '/CustodianPage/ReceivedQuoteItem',
            data: { id: Purid },
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

                    console.log('Quantity from response:', quoteItemObject.Quantity);
                    // Create input elements for quantity, unit price, and discount
                    var quantityInput = $('<input>').attr({
                        'type': 'text',
                        'style': 'width:80px; text-align: center;',
                        'class': 'quantityInput',
                        'readonly': 'readonly',
                        'value': quoteItemObject.Quantity
                    });

                    row.append($('<td>').append(quantityInput));

                    row.append($('<td>').text(quoteItemObject.Unit));

                    var unitPriceInput = $('<input>').attr({
                        'type': 'text',
                        'style': 'width:80px; text-align: center;',
                        'class': 'unitPriceInput',
                        'oninput': 'calculate()'
                    });
                    row.append($('<td>').append(unitPriceInput));

                    var discountInput = $('<input>').attr({
                        'type': 'text',
                        'style': 'width:80px; text-align: center;',
                        'class': 'discountInput',
                        'oninput': 'calculate()'
                    });
                    row.append($('<td>').append(discountInput));

                    // Add the row to the table body
                    tableBody.append(row);
                });

                calculate();
                console.log('tableData:', tableData);
            },
            error: function (error) {
                console.error('Error:', error);
            },
        });

        $('.invoice-wrapper.show').show();
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
        } else {
            // User clicked Cancel on the confirmation dialog
            console.log('Deletion canceled.');
        }
    });
});

function saveQuotation() {
    supplierInfo = {
        quoteid: $('#quoteid').val(),
        suppname: $('#suppname').val(),
        suppzipcode: $('#suppzcode').val(),
        suppbarangay: $('#suppbarangay').val(),
        suppcity: $('#suppcity').val(),
        suppmunicipality: $('#suppmunicipality').val(),
        suppphone: $('#suppphone').val()
    };

    prices = {
        subtotal: $('#subtotal').val(),
        discount: $('[class^="discountInput"]').val(),
        total: $('#total').val()
    };

    quoteFormItem = {
        PurID: Purid,
        QuoteId: supplierInfo.quoteid,
        SupplierName: supplierInfo.suppname,
        SupplierZipcode: supplierInfo.suppzipcode,
        SupplierBarangay: supplierInfo.suppbarangay,
        SupplierCity: supplierInfo.suppcity,
        SupplierMunicipality: supplierInfo.suppmunicipality,
        SupplierPhone: supplierInfo.suppphone,
        Subtotal: prices.subtotal,
        Discount: prices.discount,
        TotalPrice: prices.total
    };

    console.log('info', supplierInfo);
    console.log('prices', prices);

    // Perform AJAX request to save data
    $.ajax({
        type: 'POST',
        url: '/CustodianPage/CreateQuotationForm',
        data: JSON.stringify({ purid: Purid, supplierInfo: quoteFormItem, prices: prices }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            // Handle success, update the view if needed
            alert(response.message);

            // Proceed to save QuotationItem after successful QuotationForm insertion
            saveQuotationItem();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            console.log('Response Text:', jqXHR.responseText);
            console.log('Response JSON:', jqXHR.responseJSON);
            alert('Error creating quotation form. Please try again.');
        }
    });
}

function saveQuotationItem() {
    // Perform AJAX request to fetch quoteid from the database
    tableData = [];

    // Iterate over each row in the table body
    $('#dataTableBody tr').each(function (index) {
        var row = $(this);
        var rowData = {};
        var rowCells = row.find('td');

        rowCells.each(function (j) {
            var cellContent = $(this).find('input').length > 0 ? $(this).find('input').val() : $(this).text();
            rowData['column' + (j + 1)] = cellContent;
        });

        tableData.push(rowData);
    });

    var jsonData = JSON.stringify({
        tableData: tableData,
        suppname: supplierInfo.suppname
    });

    console.log('Response Json', jsonData);

    $.ajax({
        type: 'POST',
        url: '/CustodianPage/SaveQuotationForm',
        data: jsonData,
        contentType: 'application/json',
        success: function (response) {
            if (response.success && response.quoteid !== undefined) {
                // Fetched quoteid successfully
                quoteId = response.quoteid;
                SupplierName = response.suppname;
                console.log(`Fetched quoteid successfully: ${response.quoteid}`);
                alert(response.message);
            } else {
                alert('Failed to fetch quoteid. Please try again.');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            alert('Error fetching quoteid. Please try again.');
        }
    });

    console.log('Table Data:', tableData);
}

function clearInputs() {
    $('#dataTableBody').empty();
    $('#quantityInput, #unitPriceInput, #discountInput').val('');
    $('#suppname, #suppzcode, #suppbarangay, #suppcity, #suppmunicipality, #suppphone, #subtotal, #total').val('');
}

function goBack() {
    // Hide the modal
    $('.invoice-wrapper.show').hide();
}

function calculate() {
    // Calculate subtotal for each row and update the total
    var totalSubtotal = 0;
    var totalDiscount = 0;

    $('[class^="quantityInput"]').each(function (index) {
        var quantity = parseFloat($(this).val()) || 0;
        var unitPrice = parseFloat($('[class^="unitPriceInput"]').eq(index).val()) || 0;
        var discount = parseFloat($('[class^="discountInput"]').eq(index).val()) || 0;
        var discountPercent = discount / 100;

        var subtotal = quantity * unitPrice;
        totalSubtotal += subtotal;

        // Calculate discount for each row and update total discount
        var rowDiscount = subtotal * discountPercent;
        totalDiscount += rowDiscount;
    });

    // Update result inputs
    $('#subtotal').val(totalSubtotal.toFixed(2));
    $('#totalDiscount').val(totalDiscount.toFixed(2));

    // Calculate total after applying discount
    var total = totalSubtotal - totalDiscount;
    $('#total').val(total.toFixed(2));
}
