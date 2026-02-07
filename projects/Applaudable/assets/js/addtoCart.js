let orders = [];


let totalAmount = JSON.parse(localStorage.getItem('TotalAmount')) || 0;
document.getElementById('totalAmount').textContent = `${totalAmount}`;
    
let totalQuantity = JSON.parse(localStorage.getItem('ItemCount')) || 0;
document.getElementById('Quantity').textContent = `${totalQuantity}`;
document.getElementById('cart_counter').textContent = `${totalQuantity}`;


let product_key = document.querySelectorAll('.menu_cat_con');

// Add event listener to all product
let food_names = document.getElementsByClassName('food_name');
let food_prices = document.getElementsByClassName('food_price');
let food_images = document.getElementsByClassName('food_image');
let food_bars = document.getElementsByClassName('food_bar');
let added_pop = document.getElementById('added_item_pop');
let added_Items_Name = document.getElementById('added_item_name');

for (let i = 0; i < product_key.length; i++) {
    product_key[i].addEventListener('click', () => {
        let food_name = food_names[i].textContent;
        let food_bar = food_bars[i].src;
        let food_price = food_prices[i].textContent;
        let food_image = food_images[i].src;
        // console.log("Product img:" + food_image + "Product Name:" + food_name + " Product Price: " + food_price);
        // document.getElementById('user_choice_img').src = food_image;
        // document.getElementById('user_choice_name').textContent = food_name;
        // document.getElementById('user_choice_price').textContent = food_price;
            console.log("Added: " + food_name +  " on the cart!");
            added_pop.style.display = 'flex';
            // added_Items_Name.textContent = food_name +  " Successfuly added!";
            setTimeout(() => {
                added_pop.style.display = 'none'; 
            },1500)

        addToOrder(food_image, food_name, food_bar, food_price);
    });
}

function addToOrder(food_image, food_name, food_bar, food_price) {
    
    let existingItemIndex = orderedItems.findIndex(item => item.productName === food_name);

    if (existingItemIndex !== -1) {
        // Item already exists, update its properties
        orderedItems[existingItemIndex].quantity += 1;
        orderedItems[existingItemIndex].totalPrice += parseFloat(food_price);
    } else {
        // Item does not exist, add it to the list
        orderedItems.push({
            productImg: food_image,
            productImg2: food_bar,
            productName: food_name,
            productPrice: parseFloat(food_price).toFixed(2),
            quantity: 1,
            totalPrice: parseFloat(food_price)
        });
    }

    // Update the 'orderedItems' array in local storage
    localStorage.setItem('orderedItems', JSON.stringify(orderedItems));
    // Update the 'orders' variable and re-render the order display
    orders = orderedItems;
    showOrder();
}



function showOrder() {
    let ListHolder = document.getElementById('ListHolder');
    ListHolder.innerHTML = "";
    let itemCount = 0;

    orders.forEach(function(items) {
        itemCount++;
        ListHolder.innerHTML +=
            `
        <li>
            <div class="item_img_name">
                <figure>
                    <img id="user_choice_img" src=" ${items.productImg}" alt="${items.productName}" width="70px">
                </figure>
                <label id="user_choice_name">${items.productName}</label>
                <img id="user_barcode_choice" src="${items.productImg2}" alt="${items.productName}" width="70px">
            </div>
            <span id="user_choice_price">₱ ${items.productPrice}</span>
            <span id="item_quantity">${items.quantity} x</span>
            <div class="remove_item" onclick="removeItem(${itemCount - 1})">
                <i class='bx bx-trash'></i>
            </div>
        </li>
        `;
    });
    // Function to total the amount and etc
    let totalAmount = orders.reduce((total, item) => total + item.totalPrice, 0);
    document.getElementById('totalAmount').innerHTML = "₱"  + totalAmount.toFixed(2).toLocaleString();
    document.getElementById('Quantity').innerHTML = "Total Items: " +  itemCount ;
    document.getElementById('cart_counter').innerHTML = itemCount;
    localStorage.setItem('TotalAmount', JSON.stringify(totalAmount));
    localStorage.setItem('ItemCount', JSON.stringify(itemCount));
    document.getElementById('Quantity').innerHTML = "Quantity: " + `${itemCount}`;

}
let orderedItems = JSON.parse(localStorage.getItem("orderedItems")) || [];
orders = orderedItems;
showOrder();




// Remove Item
function removeItem(itemCount) {

    if (itemCount >= 0 && itemCount < orderedItems.length) {
        let removedItem = orderedItems.splice(itemCount, 1)[0]; // Remove the item and get the removed item's details
        localStorage.setItem("orderedItems", JSON.stringify(orderedItems));
        orders = orderedItems;
        showOrder();

        // Check if cart is empty and clear local storage if it is
        if (orderedItems.length === 0) {
            localStorage.removeItem("orderedItems");
            window.location.reload();
        }
    }
}



// Qr Generator
const generateButton = document.getElementById('cart_generate_qr_btn');
const qrImg = document.getElementById('qr_img');
const ListHolder = document.getElementById('ListHolder');
const qr_code_con = document.getElementById('qr_code_con');

 
generateButton.addEventListener('click', () => {


    let listItems = ListHolder.querySelectorAll('li');
    let qrValue = "";

    listItems.forEach(item => {
        let itemName = item.querySelector('.item_img_name label').textContent.trim();
        // let itemPrice = item.querySelector('#user_choice_price').textContent.trim();
        let itemQuantity = item.querySelector('#item_quantity').textContent.trim();
        
        qrValue += `${itemName} , ${itemQuantity}\n`; //- ${itemPrice}
    });
    // let totalAmount = document.getElementById('totalAmount').textContent;
    // qrValue += `Total Amount: ${totalAmount}\n`;

    if (!qrValue) return;

    generateButton.innerHTML = "Generating QR code...";

    qrImg.src = `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(qrValue)}`;
    document.getElementById('loader').style.display = 'flex';
    qr_code_con.style.display="flex";
    // Reset button text after loading QR code
    qrImg.addEventListener("load", () => {
        generateButton.innerHTML = "Generate QR Code";
        document.getElementById('loader').style.display = 'none';
    });
});


const downloadButton = document.getElementById('download_button');
downloadButton.addEventListener('click', () => {
    const qrDataURL = qrImg.src;
    const xhr = new XMLHttpRequest();
    xhr.open('GET', qrDataURL, true);
    xhr.responseType = 'blob';
    xhr.onload = function() {
        const blob = new Blob([xhr.response], { type: 'image/png' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'AffooodabelQR.png';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    };
    xhr.send();
    localStorage.clear("orderedItems");
    localStorage.removeItem("ItemCount");
    localStorage.removeItem("TotalAmount");
});




viewer_close_btn.addEventListener('click' , function(){
    viewer_overlay.style.display = 'none';
})

let orderBotton = document.querySelector('.addto_order');
orderBotton.addEventListener('click', () => {
    window.location.href="/order";
});

let show_cart = document.getElementById('show_cart');
let bodyTag = document.body;
show_cart.addEventListener('click', ()=> {
    cart_popup_con.style.display = "block";
    bodyTag.style.overflow = "hidden";
});

let cart_close_btn = document.getElementById('cart_close_btn');
let cart_popup_con = document.getElementById('cart_popup_con');
cart_close_btn.addEventListener('click', ()=> {
    cart_popup_con.style.display = "none";
    bodyTag.style.overflow = "visible";
});

let close_qr = document.getElementById('close_qr');

close_qr.addEventListener('click', ()=> {
    qr_code_con.style.display="none";
    cart_popup_con.style.display = "none";
    bodyTag.style.overflow = "visible";
    // localStorage.clear("orderedItems");
    // localStorage.removeItem("ItemCount");
    // localStorage.removeItem("TotalAmount");
    window.location.reload();
});

