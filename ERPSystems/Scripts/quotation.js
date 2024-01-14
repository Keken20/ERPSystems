var Purid;
var ProdId;
var Quantity;
var supplierInfo;
var prices;
var QuoteItems;
var QuoteId;
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
                        'type': 'number',
                        'style': 'width:80px; text-align: center;',
                        'class': 'unitPriceInput',
                        'oninput': 'calculate()',
                        'min': 1,
                        'max': 100000
                    });
                    row.append($('<td>').append(unitPriceInput));

                    // Add the row to the table body
                    tableBody.append(row);
                })

                calculate();
                console.log('tableData:', tableData);
            },
            error: function (error) {
                console.error('Error:', error);
            },
        });

        $('.invoice-wrapper.show').show();
    });

    $('#saveButton').on('click', function () {
        if (validateInputs()) {
            // Proceed with the save operation
            saveQuotation();
        } else {
            // Handle invalid inputs, e.g., show an error message
            alert('Please correct the input errors before saving.');
        }
    });

    function validateInputs() {
        var requiredInputs = [
            { selector: '#suppname', pattern: /\S/, errorMessage: 'Company name cannot be empty.' },
            { selector: '#suppzcode', pattern: /^[0-9]{4}$/, errorMessage: 'Invalid zip code. Please enter a 4-digit zip code.' },
            { selector: '#suppbarangay, #suppcity, #suppmunicipality', pattern: /\S/, errorMessage: 'Address cannot be empty.' },
            { selector: '#suppphone', pattern: /^(?:\d{7,11})$/, errorMessage: 'Invalid phone number. Please enter a valid phone number.' },
            { selector: '.unitPriceInput', pattern: /^-?\d{1,6}$/, errorMessage: 'Please enter a valid number between 0 and 100000.' },
            { selector: '.discountInput', pattern: /^-?\d{1,6}$/, errorMessage: 'Please enter a valid number between 0 and 100000.' }
        ];

        for (var i = 0; i < requiredInputs.length; i++) {
            var input = requiredInputs[i];
            var inputValue = $(input.selector).val().trim();

            if (inputValue === '' || !input.pattern.test(inputValue)) {
                alert(input.errorMessage);
                return false;
            }
        }

        return true;
    }
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
    $('#dataTableBody tr').each(function () {
        var row = $(this);
        var rowData = getRowData(row);  // Use the getRowData function to fetch row data
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
            // Fetched quoteid successfully
            QuoteId = response.quoteid;
            SupplierName = response.suppname;
            console.log(`Fetched quoteid successfully: ${response.quoteid}`);
            alert(response.message);

            window.location.href = '/CustodianPage/CustodianQuotation';
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('AJAX Error:', textStatus, errorThrown);
            alert('Data already inserted');
        }
    });

    console.log('Table Data:', tableData);
}

// Updated getRowData function to fetch data correctly
function getRowData(row) {
    var rowData = {
        ProdId: row.find('td:eq(0)').text(),
        QuoteQuantity: row.find('.quantityInput').val(),
        QuoteUnit: row.find('td:eq(4)').text(),
        UnitPrice: row.find('.unitPriceInput').val(),
    };
    return rowData;
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

    $('[class^="quantityInput"]').each(function (index) {
        var quantity = parseFloat($(this).val()) || 0;
        var unitPrice = parseFloat($('[class^="unitPriceInput"]').eq(index).val()) || 0;

        var subtotal = quantity * unitPrice;
        totalSubtotal += subtotal;
    });

    // Calculate discount for the entire purchase
    var totalDiscountPercent = parseFloat($('[class^="discountInput"]').val()) || 0;
    var totalDiscount = totalSubtotal * (totalDiscountPercent / 100);

    // Update result inputs
    $('#subtotal').val(totalSubtotal.toFixed(2));
    $('#totalDiscount').val(totalDiscount.toFixed(2));

    // Calculate total after applying discount
    var total = totalSubtotal - totalDiscount;
    $('#total').val(total.toFixed(2));
}


function validateInputs() {
    var requiredInputs = [
        '#suppname',
        '#suppzcode',
        '#suppbarangay',
        '#suppcity',
        '#suppmunicipality',
        '#suppphone',
        '.unitPriceInput',
        '.discountInput'
    ];

    for (var i = 0; i < requiredInputs.length; i++) {
        var inputId = requiredInputs[i];
        var inputValue = $(inputId).val().trim();

        if (inputValue === '') {
            alert('Please fill in all required fields.');
            return false;
        }
    }

    return true;
}