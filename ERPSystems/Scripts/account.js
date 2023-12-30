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

        // Populate the form fields
        $('#userId').val(userId);
        $('#userFullName').val(userFullName);
        $('#userName').val(userName);
        $('#userPassword').val(userPassword);
        $('#userType').val(userType);

        // Show the modal
        $('.modal').show();
    });
});

