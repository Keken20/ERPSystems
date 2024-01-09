var Purid;
$('.view').on('click', function () {
    Purid = $(this).data('pur-id');
    var PurDate = $(this).data('pur-created');
    console.log(Purid, PurDate)
    // Redirect to the controller action that sets ViewBag and renders the view
    window.location.href = '/AdminPage/AdminQuotationList?purId=' + Purid + '&purCreated=' + PurDate;
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
})




