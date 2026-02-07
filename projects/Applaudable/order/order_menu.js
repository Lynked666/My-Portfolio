
// ============ menubar_function ==============

let menubar_function = document.getElementsByClassName("menubar_function");
let menu_container_sorted = document.getElementById("menu_container_sorted");
// let product_key = document.querySelectorAll('.menu_cat_con');
let notfound = document.getElementById('product_not_found');


for(let i = 0;i < menubar_function.length; i++){
    menubar_function[i].addEventListener('click',  () => {
        if (i === 0){
            console.log("All menu Showing");
            product_key.forEach(image => {
                image.style.display = 'block'; // Show the image
            });
            
        } if (i === 1){
            console.log("Brunch is Showing");
            product_key.forEach(image => {
                // Get the value of the data-keywords attribute and convert it to lowercase
                let keywords = image.getAttribute('data-keywords').toLowerCase();

                // Check if the keywords match the keyword you're interested in
                if (keywords.includes('brunch')) {
                    // If the keyword is matched, show the image
                    console.log("Showing brunch");
                    image.style.display = 'block'; // Show the image
                    notfound.style.display = 'none';
                } else {
                    // If the keyword is not matched, hide the image
                    console.log("Hiding not brunch");
                    image.style.display = 'none'; // Hide the image
                }
            });

        }if (i === 2){
            console.log("Drinks is Showing");
            product_key.forEach(image => {
                // Get the value of the data-keywords attribute and convert it to lowercase
                let keywords = image.getAttribute('data-keywords').toLowerCase();

                // Check if the keywords match the keyword you're interested in
                if (keywords.includes('drink')) {
                    // If the keyword is matched, show the image
                    console.log("Showing Drink");
                    image.style.display = 'block'; // Show the image
                    notfound.style.display = 'none';
                } else {
                    // If the keyword is not matched, hide the image
                    console.log("Hiding not Drink");
                    image.style.display = 'none'; // Hide the image
                }
            });
           
        }if (i === 3){
            console.log("Dessert is Showing");
            product_key.forEach(image => {
                // Get the value of the data-keywords attribute and convert it to lowercase
                let keywords = image.getAttribute('data-keywords').toLowerCase();

                // Check if the keywords match the keyword you're interested in
                if (keywords.includes('dessert')) {
                    // If the keyword is matched, show the image
                    console.log("Showing Dessert");
                    image.style.display = 'block'; // Show the image
                    notfound.style.display = 'none';
                } else {
                    // If the keyword is not matched, hide the image
                    console.log("Hiding not Drinks");
                    image.style.display = 'none'; // Hide the image
                }
            });
           
        }if (i === 4){
            console.log("Snacks is Showing");
            product_key.forEach(image => {
                // Get the value of the data-keywords attribute and convert it to lowercase
                let keywords = image.getAttribute('data-keywords').toLowerCase();

                // Check if the keywords match the keyword you're interested in
                if (keywords.includes('snack')) {
                    // If the keyword is matched, show the image
                    console.log("Showing Snack");
                    image.style.display = 'block'; // Show the image
                    notfound.style.display = 'none';
                } else {
                    // If the keyword is not matched, hide the image
                    console.log("Hiding not Snack");
                    image.style.display = 'none'; // Hide the image
                }
            });
        }
    }
)}