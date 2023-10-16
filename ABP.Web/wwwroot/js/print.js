function PrintableContent(printContent) {
    var printButton = document.querySelector("button.btn-primary");

    if (printButton) {
        printButton.style.display = "none"; // Hide the "Print" button
    }

    var headerContent = document.getElementById('header').outerHTML; // Include the header
    var printContents = headerContent + document.getElementById(printContent).innerHTML;
    var originalContent = document.body.innerHTML;
    document.body.innerHTML = printContents;

    window.print();

    document.body.innerHTML = originalContent; // Restore the original content

    if (printButton) {
        printButton.style.display = "block"; // Show the "Print" button again
    }
}
