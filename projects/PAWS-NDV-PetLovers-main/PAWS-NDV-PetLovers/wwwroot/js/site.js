// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function validateDecimalInput(input) {
    // Allow only numbers and a single decimal point
    input.value = input.value.replace(/[^0-9.]/g, '');

    // Ensure only one decimal point is present
    if ((input.value.match(/\./g) || []).length > 1) {
        input.value = input.value.replace(/\.+$/, "");
    }

    // Limit the total length to 7 characters
    if (input.value.length > 7) {
        input.value = input.value.slice(0, 7);
    }
}

//password Hide/Show
$(document).ready(function () {
    $('.toggle-password-icon').on('click', function () {
        const inputField = $(this).closest('.position-relative').find('.password-field');
        const iconElement = $(this).find('.togglePasswordIcon');

        // Toggle password visibility
        if (inputField.attr('type') === 'password') {
            inputField.attr('type', 'text');
            iconElement.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            inputField.attr('type', 'password');
            iconElement.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });
});




//Reports
$(document).ready(function () {
    // Function to toggle the status dropdown based on the selectType value
    function toggleStatusVisibility() {
        var selectedType = $('#selectType').val();
        var $statusSelect = $('#statusVisibility select');

        if (selectedType === 'all') {
            $statusSelect.prop('disabled', true); // Disable status dropdown when 'all' is selected
            $statusSelect.addClass('disabled-select'); // Add class for styling
        } else {
            $statusSelect.prop('disabled', false); // Enable status dropdown for other selections
            $statusSelect.removeClass('disabled-select'); // Remove class when enabled
        }
    }

    // Run the function on page load to handle the initial state
    toggleStatusVisibility();

    // On change event of the selectType dropdown
    $('#selectType').on('change', function () {
        toggleStatusVisibility();
    });
});


//Numbers/Decimal only for inputs
$(document).ready(function () {
    function validateDecimalInput(input) {
        var value = $(input).val().replace(/[^0-9.]/g, ''); // Allow only numbers and periods
        if ((value.match(/\./g) || []).length > 1) {
            value = value.replace(/\.+$/, ""); // Remove extra periods
        }

        // Limit the maximum length to 10
        if (value.length > 10) {
            value = value.substring(0, 10); // Truncate to 10 characters
        }

        $(input).val(value);
    }

    // Apply the function on input event
    $('input.decimal-input').on('input', function () {
        validateDecimalInput(this);
    });
});


//auto fill date
$(document).ready(function () {
    // Get the current date
    var currentDate = new Date();

    // Format the date in YYYY-MM-DD for input field
    var formattedDate = currentDate.getFullYear() + '-' +
        ('0' + (currentDate.getMonth() + 1)).slice(-2) + '-' +
        ('0' + currentDate.getDate()).slice(-2);

    // Set the value of the input field with id 'dateInput'
    $('#dateInput, .dateInput').val(formattedDate);
});

//max date for input type date
$(document).ready(function () {
    // Get the current date
    var today = new Date().toISOString().split('T')[0];

    // Set max attribute to today's date
    $('.maxDate').attr('max', today);
});

//minimum date - not include past dates
$(document).ready(function () {
    // Get the current date in YYYY-MM-DD format
    var today = new Date().toISOString().split('T')[0];

    // Set min attribute to today's date to prevent selecting past dates
    $('.minDate').attr('min', today);
});


//CREATE -- alert notification For success duration
$(document).ready(function () {
    var alert = $('#success-alert');
    if (alert.length) {
        setTimeout(function () {
            alert.alert('close');
        }, 5000); // 5 seconds
    }
});


/*form-add-for-pet*/
$(document).ready(function () {
    // Function to update the name attributes of the rows
    function updateRowNames() {
        $('#PetTable tbody tr').each(function (index) {
            $(this).find('input, select').each(function () {
                var name = $(this).attr('name');
                if (name) {
                    var newName = name.replace(/\d+/, index);
                    $(this).attr('name', newName);
                }
            });
        });
    }

    // Add row
    $('#addRow').click(function () {
        var rowCount = $('#PetTable tbody tr').length;
        var newRow = $('.detailRow').first().clone();

        newRow.find('input, select').each(function () {
            var name = $(this).attr('name');
            if (name) {
                var newName = name.replace(/\d+/, rowCount);
                $(this).attr('name', newName);
            }
            if ($(this).attr('id') !== 'dateInput') {
                $(this).val(''); // Clear the value of the input/select except dateInput
            }
        });

        newRow.appendTo($('#PetTable tbody'));
    });

    // Remove row
    $(document).on('click', '.removeRow', function () {
        if ($('#PetTable tbody tr').length > 1) {
            $(this).closest('tr').remove();
            updateRowNames(); // Update the name attributes of the remaining rows
        }
    });
    // =================

    // Function to set the max date for birthdate fields

    function setMaxBirthdate() {
        var today = new Date().toISOString().split('T')[0];
        $('.birthdate').attr('max', today);
    }

    // Auto-fill age field based on birthdate // owner create pets
    $(document).on('change', '.birthdate', function () {
        var birthdate = $(this).val();
        var age = calculateAge(birthdate);
        $(this).closest('tr').find('.age').val(age);
    });


    //pets
    // Auto-fill age field based on birthdate
    $(document).on('change', '.birthdate', function () {
        var birthdate = $(this).val();
        var age = calculateAge(birthdate);
        $(this).closest('.petrow').find('.age').val(age);
    });


    // Function to calculate age
    function calculateAge(birthdate) {
        var today = new Date();
        var birthDate = new Date(birthdate);
        var age = today.getFullYear() - birthDate.getFullYear();
        var month = today.getMonth() - birthDate.getMonth();
        if (month < 0 || (month === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }
        return age;
    }

    setMaxBirthdate(); // Set max date on initial load
});


//get the pet id when clicking the modal edit
document.addEventListener('DOMContentLoaded', function () {
    $('#editPetModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget);
        var petId = button.data('id');

        var modal = $(this);
        $.get('/Owners/EditPet', { id: petId }, function (data) {
            modal.find('.modal-body').html($(data).find('.modal-body').html());
        });
    });
});

//contact number fields
$(document).ready(function () {
    // Target the contact input field
    var contactInput = $('#contact');

    // Function to clean and format the input value
    function formatContact(value) {
        // Remove non-numeric characters
        value = value.replace(/\D/g, '');

        // Ensure it starts with "09"
        if (!value.startsWith('09')) {
            value = '09' + value;
        }

        // Limit the length of the value (e.g., 11 digits)
        if (value.length > 11) {
            value = value.slice(0, 11);
        }

        return value;
    }

    // On input event, format the contact field
    contactInput.on('input', function () {
        var formattedValue = formatContact($(this).val());
        $(this).val(formattedValue);
    });

    // On focus event, ensure the contact field starts with "09"
    contactInput.on('focus', function () {
        var value = $(this).val();
        if (!value.startsWith('09')) {
            $(this).val('09' + value);
        }
    });

    // On blur event, ensure the contact field starts with "09"
    contactInput.on('blur', function () {
        var value = $(this).val();
        if (!value.startsWith('09')) {
            $(this).val('09' + value);
        }
    });
});