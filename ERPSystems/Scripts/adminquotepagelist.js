var QoutItems;
var QouteId;

$(document).ready(function () {
    // Show modal on view button click
    $('.view').on('click', function () {
        var tableBody = $('#dataTableBody');
        QouteId = $(this).data('qoute-id');
        var Date = $(this).data('created-at');
        var Discount = $(this).data('qoute-disc');
        var Subtotal = $(this).data('qoute-sub');
        var Total = $(this).data('qoute-total');
        $('#subtotal').text(Subtotal);
        $('#discount').text(Discount);
        $('#total').text(Total);
        $('#Quoteid').val(QouteId);
        $('#Quotedate').val(Date);
        $('.invoice-wrapper.show').show();
        $.ajax({
            type: 'POST',
            url: '/AdminPage/GetQuoteItem',
            data: { Qouteid: QouteId },
            success: function (response) {
                // Handle the response from the server
                // Assign the response to the ReqItems variable
                console.log(response);
                QoutItems = response.quotationitem;
                console.log(QoutItems);
                QoutItems.forEach(function (requestItemObject) {
                    // Create a new row
                    var row = $('<tr>');
                    // Add cells for each property in the object
                    row.append($('<td>').text(requestItemObject.ProdId));
                    row.append($('<td>').text(requestItemObject.ProdName));
                    row.append($('<td>').text(requestItemObject.ProdDescription));
                    row.append($('<td>').text(requestItemObject.QuoteUnit));
                    row.append($('<td>').text(requestItemObject.QuoteQouantity));
                    row.append($('<td>').text(requestItemObject.QuotePricePerUnit));
                    // Add the row to the table body
                    tableBody.append(row);
                    //ProdId.push(requestItemObject.ProdId);
                    //Quantity.push(requestItemObject.Quantity);
                })
            },
            error: function (error) {
                // Handle errors
                console.error('Error:', error);
            }
        });
    });
});