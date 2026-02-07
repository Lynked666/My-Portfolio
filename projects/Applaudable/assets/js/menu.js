
// kani nga function dri kay siya ang naga handle sa pag show sa modal ug sa mga item
// ang function sa pag show sa item is naka base siya kung unsa nga index or array ang ge pislit ni user
// kay ang mga button sa menu kay ge addan nakug same nga class name then ginatawag nako siya then ge adaan ug evenlister pamagi ug looping for each akung gamit
// array start from [0 1 2 3 4 5 6]
// if si user mo click ni 0 which is LUNCH ang nag una nag add lang kug condition
// if user click 0/lunch iya e show ang item nga naka lista sa arry object
// array ni []
// object ni {}
// if e combine = [{}]  array object - kung asa naka store atung data
// data stored on product.js file

// then after niya makuha ang data iya ni e display sa html nga side wich is naay naka id name nga " menu_category_list"
// kung asa siya mo display

// then every condition is lahi2 ug variable name naka base sa imung ge pislit kung naka bantay ka na naay:
// lunch_category, snacks_catgeory etc
// then if naka bantay pud ka every variable nila sa lahi pud ang item name nila or property name
// for example if ang ge pilsit is lunch so ang tawagon pud niya ang variable name nga lunch_category the sulod ana niya naay object wich naa tay property name didto nga lunch_category_name or food_name
//  e analyze lang ug tarung promise ma sabtan rajud ni nimu then after nimu analayze e remove ni nga message kay paksit jud ka kung mabasahan ni wohahahaha 
// then careful lang pud diay sa pag ilis sa mga name ayaw lang ilisi ang mga property name, sa product name lang, kay kung property name imung ilisan make sure nga maka subay ka sa code kay ginatawag na sila

document.addEventListener("DOMContentLoaded", function(event) {
    // Your JavaScript code here



let menu_btn = document.getElementsByClassName("menu_selection");
let menu_category_list = document.getElementById('menu_category_list');
let modal_overlay = document.getElementById('modal_overlay');
function menuCategory(){
    
//adding event lister on menu catgeory and create th Li element and other classes to render all product that stored on the product list 
    for(let i = 0;i < menu_btn.length; i++){
        menu_btn[i].addEventListener('click', () => {
            if (i === 0){
                document.getElementById('category_name').textContent = 'Lunch';
                document.getElementById('items_count').textContent = lunch_Category.length;
                modal_overlay.style.display = "block";
                lunch_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.food_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.food_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.food_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;

                });
            }if (i === 1){
                document.getElementById('category_name').textContent = 'Snack';
                document.getElementById('items_count').textContent = snacks_Category.length;
                modal_overlay.style.display = "block";
                snacks_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.snacks_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.snacks_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.snacks_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 2){
                document.getElementById('category_name').textContent = 'Dessert';
                document.getElementById('items_count').textContent = dessert_Category.length;
                modal_overlay.style.display = "block";
                dessert_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.dessert_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.dessert_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.dessert_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 3){
                document.getElementById('category_name').textContent = 'Beer';
                document.getElementById('items_count').textContent = beer_Category.length;
                modal_overlay.style.display = "block";
                beer_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.beer_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.beer_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.beer_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 4){
                document.getElementById('category_name').textContent = 'Dessert Drink';
                document.getElementById('items_count').textContent = dessert_drink_Category.length;
                modal_overlay.style.display = "block";
                dessert_drink_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.dessert_drink_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.dessert_drink_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.dessert_drink_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 5){
                document.getElementById('category_name').textContent = 'Egg & Ham';
                document.getElementById('items_count').textContent = egg_ham_Category.length;
                modal_overlay.style.display = "block";
                egg_ham_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.egg_ham_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.egg_ham_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.egg_ham_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 6){
                document.getElementById('category_name').textContent = 'Biscuit';
                document.getElementById('items_count').textContent = biscuit_Category.length;
                modal_overlay.style.display = "block";
                biscuit_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.biscuit_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.biscuit_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.biscuit_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 7){
                document.getElementById('category_name').textContent = 'Fries';
                document.getElementById('items_count').textContent = fries_Category.length;
                modal_overlay.style.display = "block";
                fries_Category.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.fries_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.fries_name}</h3>
                                <p class="food_description">${item.food_description}</p>
                                <span>
                                    ₱
                                   <small id="food_price">${item.fries_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }if (i === 8){
                document.getElementById('category_name').textContent = 'Top Recommendation';
                document.getElementById('items_count').textContent = top_menu.length;
                modal_overlay.style.display = "block";
                top_menu.forEach(item => {
                    menu_category_list.innerHTML +=`
                        <li class="menu_cat_con">
                            <figure>
                                <img src="${item.top_product_img}" alt="">
                            </figure>
                            <div class="food_det">
                                <h3 id="food_name">${item.top_food_name}</h3>
                                <!-- <p class="food_description">${item.food_description}</p> -->
                                <span>
                                    ₱
                                   <small id="food_price">${item.top_product_price}</small> 
                                </span>
                            </div>
                        </li>
                    `;
                });
            }

    hideDescription(); //comment this code if you want to hide the food description

        });
        
    }


}

menuCategory();



// close modal popup and set menu catgeory list into empty
let pop_close_btn = document.getElementById('pop_close_btn');
pop_close_btn.addEventListener('click', function(){
    modal_overlay.style.display = "none";
    menu_category_list.innerHTML = [];
});

// this will add description on your product that stored on product list
// you can run this function by comment the "hideDescription()" above
// optional
function hideDescription() {
    let food_description1 = document.querySelectorAll('.food_description'); // Use .food_description for class selection
    for (let i = 0; i < food_description1.length; i++) { // Corrected loop condition
        food_description1[i].style.display = "none";
    }
}







// ============ menubar_function ==============

let menubar_function = document.getElementsByClassName("menubar_function");
let menu_container_sorted = document.getElementById("menu_container_sorted");
let product_key = document.querySelectorAll('.menu_cat_con');
let notfound = document.getElementById('product_not_found');


// added event listener on the product menu, and show what user click and hide the the other product 
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

// Get the Id and the classname and added event listener on the product
let food_names = document.getElementsByClassName('food_name');
let food_prices = document.getElementsByClassName('food_price');
let food_images = document.getElementsByClassName('food_image');
let viewer_overlay = document.getElementById('viewer_overlay');
let viewer_close_btn = document.getElementById('viewer_close_btn');


for (let i = 0; i < product_key.length; i++) {
    product_key[i].addEventListener('click', () => {
        let food_name = food_names[i].textContent;
        let food_price = food_prices[i].textContent;
        let food_image = food_images[i].getAttribute('src');
        viewer_overlay.style.display = 'flex';

        document.getElementById('product_img_viewer').src = food_image;
        document.getElementById('product_name_viewer').textContent = food_name;
        document.getElementById('product_price_viewer').textContent = food_price;

    });
}

// Close Popup 
viewer_close_btn.addEventListener('click' , function(){
    viewer_overlay.style.display = 'none';
})

});


// Redirect home page to order page
let orderBotton1 = document.querySelector('.addto_order');
orderBotton1.addEventListener('click', () => {
    window.location.href="/order/";
});











    


