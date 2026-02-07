function search(){
    // get the value from teh input
    const searchItem = document.getElementById('Product').value.toLowerCase();

    // get all the image element
    images = document.querySelectorAll('.menu_cat_con');

    let found = false;

    // loop through the images and hide those dont match the search
    images.forEach((image) => {
        const keywords = image.getAttribute('data-keywords').toLowerCase();
        if(keywords.includes(searchItem)){
            image.style.display = 'block';
            found = true;
        }else{
            image.style.display = 'none';
        }
        let notfound = document.getElementById('product_not_found');
        if(!found){
            notfound.style.display = 'flex';
        }else{
            notfound.style.display = 'none';
        }
    });
}

// Add active class to the current button (highlight it)
var menuList = document.getElementById("menu_list");
var btns = menuList.getElementsByClassName("menubar_function");
for (var i = 0; i < btns.length; i++) {
  btns[i].addEventListener("click", function() {
    var current = document.getElementsByClassName("active_menu");
    current[0].className = current[0].className.replace(" active_menu", "");
    this.className += " active_menu";
  });
}