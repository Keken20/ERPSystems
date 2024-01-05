
var Aiken;
var ReqItems;
var ReqId;
var ProdId = [];
var Quantity = [];
var firstButtonClicked = false;
var head;
var tableData;
var rowData;

$(document).ready(function () {
    // Show modal on view button click
    $('.view').on('click', function () {
        ReqId = $(this).data('req-id');
        var ReqName = $(this).data('req-name');
        var ReqDate = $(this).data('req-date');
        var tableBody = $('#dataTableBody');
        $('#reqid').val(ReqId);
        $('#reqname').val(ReqName);
        $('#reqdate').val(ReqDate);
        $.ajax({
            type: 'POST',
            url: '/AdminPage/ReceivedRequestItem',
            data: { id: ReqId },
            success: function (response) {
                // Handle the response from the server
               

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
                    ProdId.push(requestItemObject.ProdId);
                    Quantity.push(requestItemObject.Quantity);
                })
            },
            error: function (error) {
                // Handle errors
                console.error('Error:', error);
            }
        });

        // Other parts of your code can now access ReqItems
        // For example, you can use it outside the Ajax request
      


        // Show the modal
        $('.invoice-wrapper.show').show();

    });

    $('#checkinventory').on('click', function () {
        this.disabled = true;
        firstButtonClicked = true;
        head = $('#dataTableHead tr');
        var allrow = $('#dataTableBody tr');
        var Onhand;
        
        $.ajax({
            type: "POST",
            url: '/PurchasingPage/ReceivedProdId',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ id: ProdId }),
            success: function (result) {             
                Onhand = result.inventory;
                head.append($('<td class="text-bold">').text('On Hand'));
                // Handle success             
             
                allrow.each(function (index) {
                    var onHandValue = Onhand[index].ProdQoh;
                    var quantityTable = Quantity[index];
                    var colorClass = '';
                    if (onHandValue > quantityTable) {
                        colorClass = 'text-green';  // Change to a CSS class for green text
                    } else if (onHandValue < quantityTable) {
                        colorClass = 'text-red';    // Change to a CSS class for red text
                    }
                    else {
                        colorClass = 'text-blue';    // Change to a CSS class for red text
                    }
                    $(this).append($('<td>').text(onHandValue).addClass(colorClass));
                });
            },
            error: function (error) {
                
                
            }         
        });       
    });
    $('#dropdown').on('click', function () {
   
        if (!firstButtonClicked) {
            // Display a prompt message if the first button is not clicked
            alert("Please click the first button before clicking the second button.");
            return;
        }
        else {
            alert("Successful");
        }
    });
    $('#dropdown1').on('click', function () {
        if (!firstButtonClicked) {
            // Display a prompt message if the first button is not clicked
            alert("Please click the first button before clicking the second button.");
            return;
        } else
        {
            var allrow = $('#dataTableBody tr');
            $.ajax({
                type: 'POST',
                url: '/PurchasingPage/CreatePurchaseOrderForm',
                data: { id: ReqId },
                dataType: 'json',
                success: function (success) {
                    // Handle success, update the view if needed
                    alert(success.message);                  
                    head.append($('<td class="text-bold">').text('Purchase Quantity'));
                    allrow.each(function (i) {
                        var inputField = $('<input>').attr('type', 'text', 'style', 'width:85px');
                        inputField.css('width', '60px');

                        // Create a new button
                        var newButton = $('<button>').text('Add').addClass('newButtonClass').data('row-id', i + 1);

                        // Append the input field and button to the current row
                        $(this).append($('<td>').append(inputField));
                        $(this).append($('<td>').append(newButton));

                        console.log("Row:", i + 1);
                    });
                    // Event handler for the dynamically added buttons
                    $('.newButtonClass').on('click', function () {
                        event.stopPropagation();
                        var rowId = $(this).data('row-id');
                        tableData = [];

                        $('#dataTableBody tr').each(function (p) {
                            var clickedRow = $(this).closest('tr');
                            var cells = clickedRow.find('td');
                            rowData = {};

                            cells.each(function (j) {
                                var cellContent = $(this).find('input').length > 0 ? $(this).find('input').val() : $(this).text();
                                rowData['column' + (j + 1)] = cellContent;
                            });

                            if (p + 1 === rowId) {
                                tableData.push(rowData);                               
                            }                          
                        });                      
                        var jsonData = JSON.stringify({ tableData: tableData, reqid: ReqId });
                        console.log('Data', jsonData);
                        $.ajax({
                            type: 'POST',
                            url: '/PurchasingPage/SaveProductToPO',
                            data: jsonData,
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (response) {
                                // Handle the success response from the server
                                console.log(response);

                                // You can show a success message or perform other actions
                                alert(response.message);
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                // Handle the error response from the server
                                console.error('AJAX Error:', textStatus, errorThrown);

                                // You can show an error message or perform other actions
                                alert('Error saving data. Please try again.');
                            }
                        });
                         console.log('RowData:', rowData);
                        console.log('Table Data for Row ' + rowId + ':', tableData);
                       

                       ;
                        alert('Data added successfully for Row ' + rowId);
                    });
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error('AJAX Error:', textStatus, errorThrown);
                    console.log('Response Text:', jqXHR.responseText);
                    console.log('Response JSON:', jqXHR.responseJSON);
                    alert('Error saving data. Please try again.');
                }
            });
        }
    });
    $('#dropdown2').on('click', function () {

        if (!firstButtonClicked) {
            // Display a prompt message if the first button is not clicked
            alert("Please click the first button before clicking the second button.");
            return;
        }
        else {
            alert("Successful 2");
        }

    });
    $('#dropdown3').on('click', function () {

        if (!firstButtonClicked) {
            // Display a prompt message if the first button is not clicked
            alert("Please click the first button before clicking the second button.");
            return;
        }
        else {
            alert("Successful 3");
        }
    });
});

$('#myTable').on('click', '.newButtonClass', function () {
    // 'this' refers to the clicked button
    var buttonClass = $(this).attr('class');
    console.log('Button Class:', buttonClass);
});