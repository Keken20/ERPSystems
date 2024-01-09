function closeModal() {
    $('.modal').hide();
}

$(document).ready(function () {
    // Show modal on view button click
    $('.view').on('click', function () {
        var userId = $(this).data('user-id');
        var userFullName = $(this).data('user-fullname');
        var userName = $(this).data('user-name');
        var userPassword = $(this).data('user-password');
        var userType = $(this).data('user-type');
        var userStatus = $(this).data('acc-status');

        // Toggle the display of activate and deactivate buttons
        $('#activatebtn').toggle(userStatus !== 'Active');
        $('#deactbtn').toggle(userStatus === 'Active');

        // Populate the form fields
        $('#AccId').val(userId);
        $('#AccFullName').val(userFullName);
        $('#AccUserName').val(userName);
        $('#AccPassword').val(userPassword);
        $('#AccType').val(userType);
        $('#AccStatus').val(userStatus);

        // Show the modal
        $('.modal').show();
    });
    $('.back').on('click', function () {
        closeModal();
    });
});

