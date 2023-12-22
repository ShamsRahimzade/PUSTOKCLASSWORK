const addButtons = document.querySelectorAll(".add-basket-button");
const box = document.querySelector(".basket-books");

addButtons.forEach(btn => {
    btn.addEventListener("click", function (s) {
        s.preventDefault();
        var endpoint = btn.getAttribute("href");

        fetch(endpoint)
            .then(response => response.text())
            .then(data => {
                box.innerHTML = data;
            })
    })
})